using Entities.Models;

namespace Services.Contracts;

public interface ICategoryService
{
    // create Subject lazım
    void CreateCategory(Category category);
    Task<IEnumerable<Category>> GetAllCategories(bool trackChanges);
}