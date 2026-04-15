using Api.Core.Modules.Categories.Application.DTOs;
using Api.Core.Modules.Categories.Application.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Categories.Infrastructure.Presentation;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/category")
            .WithTags("Category");

        group.MapGet("", async (GetAllCategoriesUseCase getAll, CancellationToken cancellationToken)
                => Results.Ok(await getAll.ExecuteAsync(cancellationToken)))
            .AllowAnonymous()
            .Produces<IEnumerable<CategoryResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, GetCategoryByIdUseCase getById, CancellationToken cancellationToken) =>
            {
                var category = await getById.ExecuteAsync(id, cancellationToken);
                return category is null ? Results.NotFound() : Results.Ok(category);
            })
            .AllowAnonymous()
            .Produces<CategoryResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("", async ([FromBody] CategoryCreateRequest request, CreateCategoryUseCase create, CancellationToken cancellationToken) =>
            {
                if (!TryValidate(request, out var errors))
                    return Results.ValidationProblem(errors);

                var category = await create.ExecuteAsync(request, cancellationToken);
                return Results.Created($"/api/category/{category.Id}", category);
            })
            .RequireAuthorization("CatalogWrite")
            .Produces<CategoryResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPut("/{id:int}", async (int id, [FromBody] CategoryUpdateRequest request, UpdateCategoryUseCase update, CancellationToken cancellationToken) =>
            {
                if (!TryValidate(request, out var errors))
                    return Results.ValidationProblem(errors);

                var category = await update.ExecuteAsync(id, request, cancellationToken);
                return category is null ? Results.NotFound() : Results.NoContent();
            })
            .RequireAuthorization("CatalogWrite")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapDelete("/{id:int}", async (int id, DeleteCategoryUseCase delete, CancellationToken cancellationToken) =>
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
