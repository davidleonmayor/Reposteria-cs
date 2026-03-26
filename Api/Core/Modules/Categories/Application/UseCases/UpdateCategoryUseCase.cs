using Api.Core.Modules.Categories.Application.DTOs;
using Api.Core.Modules.Categories.Application.Interfaces;

namespace Api.Core.Modules.Categories.Application.UseCases;

public sealed class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _repository;

    public UpdateCategoryUseCase(ICategoryRepository repository) => _repository = repository;

    public async Task<CategoryResponse?> ExecuteAsync(int id, CategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var category = new global::Category
        {
            Name        = StringNormalization.Clean(request.Name),
            Description = StringNormalization.Clean(request.Description)
        };

        return await _repository.UpdateAsync(id, category, cancellationToken);
    }
}
