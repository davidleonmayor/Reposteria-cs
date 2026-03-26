using Api.Core.Modules.Categories.Application.DTOs;
using Api.Core.Modules.Categories.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.Categories.Infrastructure.Persistence;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _db.Category.ToListAsync(cancellationToken);
        return categories.Select(CategoryResponse.FromEntity);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _db.Category.FindAsync([id], cancellationToken);
        return category is null ? null : CategoryResponse.FromEntity(category);
    }

    public async Task<CategoryResponse> AddAsync(global::Category category, CancellationToken cancellationToken = default)
    {
        _db.Category.Add(category);
        await _db.SaveChangesAsync(cancellationToken);
        return CategoryResponse.FromEntity(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(int id, global::Category updated, CancellationToken cancellationToken = default)
    {
        var category = await _db.Category.FindAsync([id], cancellationToken);
        if (category is null) return null;

        category.Name        = updated.Name;
        category.Description = updated.Description;

        await _db.SaveChangesAsync(cancellationToken);
        return CategoryResponse.FromEntity(category);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _db.Category.FindAsync([id], cancellationToken);
        if (category is null) return false;

        _db.Category.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
