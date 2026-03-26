using Api.Core.Modules.Categories.Application.DTOs;
using Api.Core.Modules.Categories.Application.Interfaces;

namespace Api.Core.Modules.Categories.Application.UseCases;

public sealed class GetCategoryByIdUseCase
{
    private readonly ICategoryRepository _repository;

    public GetCategoryByIdUseCase(ICategoryRepository repository) => _repository = repository;

    public Task<CategoryResponse?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);
}
