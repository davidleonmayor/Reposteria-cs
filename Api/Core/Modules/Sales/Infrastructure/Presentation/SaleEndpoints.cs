using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Api.Core.Modules.Sales.Infrastructure.Presentation;

public static class SaleEndpoints
{
    public static IEndpointRouteBuilder MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sales").WithTags("Sales");

        group.MapGet("/", async (GetAllSalesUseCase useCase, CancellationToken ct) =>
            Results.Ok(await useCase.ExecuteAsync(ct)))
            .AllowAnonymous()
            .Produces<IEnumerable<SaleResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, GetSaleByIdUseCase useCase, CancellationToken ct) =>
        {
            var sale = await useCase.ExecuteAsync(id, ct);
            return sale is null ? Results.NotFound() : Results.Ok(sale);
        })
        .AllowAnonymous()
        .Produces<SaleResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (SaleCreateRequest request, CreateSaleUseCase useCase, ILoggerFactory loggerFactory, CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("SaleEndpoints");
            logger.LogInformation("[SaleEndpoints] POST /api/sales — State={State}, Details={Count}", request.State, request.Details?.Count ?? 0);
            var sale = await useCase.ExecuteAsync(request, ct);
            logger.LogInformation("[SaleEndpoints] Sale created — Id={Id}, Total={Total}", sale.Id, sale.Total);
            return Results.Created($"/api/sales/{sale.Id}", sale);
        })
        .AllowAnonymous()
        .Produces<SaleResponse>(StatusCodes.Status201Created);

        group.MapPut("/{id:int}", async (int id, SaleUpdateRequest request, UpdateSaleUseCase useCase, CancellationToken ct) =>
        {
            var updated = await useCase.ExecuteAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization("SalesWrite")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (int id, DeleteSaleUseCase useCase, CancellationToken ct) =>
        {
            var deleted = await useCase.ExecuteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .RequireAuthorization("SalesWrite")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
