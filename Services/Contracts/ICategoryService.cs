using Entities.Models;

namespace Services.Contracts;

public interface ICategoryService
{
    // create Subject lazım
    void CreateCategory(Category category);
    IQueryable<Category> GetAllCategories(bool trackChanges);
}