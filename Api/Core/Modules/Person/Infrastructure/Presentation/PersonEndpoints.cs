using Api.Core.Modules.Persons.Application.DTOs;
using Api.Core.Modules.Persons.Application.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Persons.Infrastructure.Presentation;

public static class PersonEndpoints
{
    public static IEndpointRouteBuilder MapPersonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/person")
            .WithTags("Person");

        group.MapGet("", async (GetAllPersonsUseCase getAll, CancellationToken cancellationToken)
                => Results.Ok(await getAll.ExecuteAsync(cancellationToken)))
            .AllowAnonymous()
            .Produces<IEnumerable<PersonResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, GetPersonByIdUseCase getById, CancellationToken cancellationToken) =>
            {
                var person = await getById.ExecuteAsync(id, cancellationToken);
                return person is null ? Results.NotFound() : Results.Ok(person);
            })
            .AllowAnonymous()
            .Produces<PersonResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("", async ([FromBody] PersonCreateRequest request, CreatePersonUseCase create, CancellationToken cancellationToken) =>
            {
                if (!TryValidate(request, out var errors))
                    return Results.ValidationProblem(errors);

                var person = await create.ExecuteAsync(request, cancellationToken);
                return Results.Created($"/api/person/{person.Id}", person);
            })
            .RequireAuthorization("PeopleWrite")
            .Produces<PersonResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPut("/{id:int}", async (int id, [FromBody] PersonUpdateRequest request, UpdatePersonUseCase update, CancellationToken cancellationToken) =>
            {
                if (!TryValidate(request, out var errors))
                    return Results.ValidationProblem(errors);

                var person = await update.ExecuteAsync(id, request, cancellationToken);
                return person is null ? Results.NotFound() : Results.NoContent();
            })
            .RequireAuthorization("PeopleWrite")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapDelete("/{id:int}", async (int id, DeletePersonUseCase delete, CancellationToken cancellationToken) =>
            {
                var deleted = await delete.ExecuteAsync(id, cancellationToken);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization("PeopleWrite")
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
