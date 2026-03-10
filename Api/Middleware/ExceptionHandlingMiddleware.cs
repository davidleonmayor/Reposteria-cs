using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

            if (context.Response.HasStarted)
                throw;

            var (status, title) = MapException(ex);
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = _env.IsDevelopment() ? ex.Message : null,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
        }
    }

    private static (int status, string title) MapException(Exception ex)
    {
        if (ex is BadHttpRequestException)
            return (StatusCodes.Status400BadRequest, "Bad request");

        if (ex is ArgumentException)
            return (StatusCodes.Status400BadRequest, "Invalid argument");

        if (ex is InvalidOperationException)
            return (StatusCodes.Status400BadRequest, "Invalid operation");

        if (ex is DbUpdateException dbUpdateEx)
        {
            if (TryMapSqliteConstraint(dbUpdateEx, out var mapped))
                return mapped;

            return (StatusCodes.Status400BadRequest, "Database update failed");
        }

        if (ex is SqliteException sqliteEx)
        {
            if (TryMapSqliteConstraint(sqliteEx, out var mapped))
                return mapped;

            return (StatusCodes.Status400BadRequest, "Database error");
        }

        return (StatusCodes.Status500InternalServerError, "Internal server error");
    }

    private static bool TryMapSqliteConstraint(DbUpdateException ex, out (int status, string title) mapped)
    {
        mapped = default;

        var sqliteEx = ex.InnerException as SqliteException;
        if (sqliteEx is null)
            return false;

        return TryMapSqliteConstraint(sqliteEx, out mapped);
    }

    private static bool TryMapSqliteConstraint(SqliteException sqliteEx, out (int status, string title) mapped)
    {
        mapped = default;

        // SQLITE_CONSTRAINT = 19
        if (sqliteEx.SqliteErrorCode != 19)
            return false;

        if (sqliteEx.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
        {
            mapped = (StatusCodes.Status409Conflict, "Conflict");
            return true;
        }

        if (sqliteEx.Message.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase))
        {
            mapped = (StatusCodes.Status400BadRequest, "Invalid reference");
            return true;
        }

        mapped = (StatusCodes.Status400BadRequest, "Constraint violation");
        return true;
    }
}
