using Entities.Models;

namespace Services.Contracts;

public interface ICategoryService
{
    void CreateCategory(Category category);
    IQueryable<Category> GetAllCategories(bool trackChanges);
}