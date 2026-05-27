using Api.Core.Modules.Products.Application.DTOs;
using Api.Core.Modules.Products.Application.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Products.Infrastructure.Presentation;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product")
            .WithTags("Product");

        group.MapGet("", async (GetAllProductsUseCase getAll, CancellationToken cancellationToken)
                => Results.Ok(await getAll.ExecuteAsync(cancellationToken)))
            .AllowAnonymous()
            .Produces<IEnumerable<ProductResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, GetProductByIdUseCase getById, CancellationToken cancellationToken) =>
            {
                var product = await getById.ExecuteAsync(id, cancellationToken);
                return product is null ? Results.NotFound() : Results.Ok(product);
            })
            .AllowAnonymous()
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("", async ([FromBody] ProductCreateRequest request, CreateProductUseCase create, CancellationToken cancellationToken) =>
            {
                if (!TryValidate(request, out var errors))
                    return Results.ValidationProblem(errors);

                var product = await create.ExecuteAsync(request, cancellationToken);
                return Results.Created($"/api/product/{product.Id}", product);
            })
            .RequireAuthorization("CatalogWrite")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPut("/{id:int}", async (int id, [FromBody] ProductUpdateRequest request, UpdateProductUseCase update, CancellationToken cancellationToken) =>
            {
                if (!TryValidate(request, out var errors))
                    return Results.ValidationProblem(errors);

                var product = await update.ExecuteAsync(id, request, cancellationToken);
                return product is null ? Results.NotFound() : Results.NoContent();
            })
            .RequireAuthorization("CatalogWrite")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapDelete("/{id:int}", async (int id, DeleteProductUseCase delete, CancellationToken cancellationToken) =>
            {
                var deleted = await delete.ExecuteAsync(id, cancellationToken);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization("CatalogWrite")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        return app;
    }

    private static bool TryValidate<T>(T model, out Dictionary<string, string[]> errors)
    {
        var context = new ValidationContext(model!);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(model!, context, results, validateAllProperties: true);

        errors = new Dictionary<string, string[]>();
        if (isValid)
            return true;

        foreach (var result in results)
        {
            foreach (var memberName in result.MemberNames)
            {
                if (!errors.TryGetValue(memberName, out var messages))
                {
                    errors[memberName] = [result.ErrorMessage ?? "Validation error."];
                    continue;
                }

                var list = new List<string>(messages) { result.ErrorMessage ?? "Validation error." };
                errors[memberName] = [.. list];
            }
        }

        return false;
    }
}
