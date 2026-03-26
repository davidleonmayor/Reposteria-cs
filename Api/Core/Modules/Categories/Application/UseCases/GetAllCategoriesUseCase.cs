using Api.Core.Modules.Categories.Application.DTOs;
using Api.Core.Modules.Categories.Application.Interfaces;

namespace Api.Core.Modules.Categories.Application.UseCases;

public sealed class GetAllCategoriesUseCase
{
    private readonly ICategoryRepository _repository;

    public GetAllCategoriesUseCase(ICategoryRepository repository) => _repository = repository;

    public Task<IEnumerable<CategoryResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);
}
