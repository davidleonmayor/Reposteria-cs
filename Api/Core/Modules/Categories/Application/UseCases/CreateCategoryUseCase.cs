using Api.Core.Modules.Categories.Application.DTOs;
using Api.Core.Modules.Categories.Application.Interfaces;

namespace Api.Core.Modules.Categories.Application.UseCases;

public sealed class CreateCategoryUseCase
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryUseCase(ICategoryRepository repository) => _repository = repository;

    public async Task<CategoryResponse> ExecuteAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        var category = new global::Category
        {
            Name        = StringNormalization.Clean(request.Name),
            Description = StringNormalization.Clean(request.Description)
        };

        return await _repository.AddAsync(category, cancellationToken);
    }
}
