using Api.Core.Modules.Categories.Application.Interfaces;

namespace Api.Core.Modules.Categories.Application.UseCases;

public sealed class DeleteCategoryUseCase
{
    private readonly ICategoryRepository _repository;

    public DeleteCategoryUseCase(ICategoryRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);
}
