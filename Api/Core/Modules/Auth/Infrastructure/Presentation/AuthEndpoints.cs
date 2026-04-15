using Api.Core.Modules.Auth.Application.DTOs;
using Api.Core.Modules.Auth.Application.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace Api.Core.Modules.Auth.Infrastructure.Presentation;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("/login", async (LoginRequest request, LoginUseCase loginUseCase, CancellationToken cancellationToken) =>
        {
            var response = await loginUseCase.ExecuteAsync(request, cancellationToken);
            if (response is null)
                return Results.Unauthorized();

            return Results.Ok(response);
        })
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/register", async (RegisterRequest request, RegisterUseCase registerUseCase, CancellationToken cancellationToken) =>
        {
            var response = await registerUseCase.ExecuteAsync(request, cancellationToken);
            if (response is null)
                return Results.Conflict(new { message = "Email already in use" });

            return Results.Created($"/api/auth/login?email={request.Email}", response);
        })
        .Produces<LoginResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        return app;
    }
}
