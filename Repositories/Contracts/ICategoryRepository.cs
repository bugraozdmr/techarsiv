using Entities.Models;

namespace Repositories.Contracts;

public interface ICategoryRepository : IRepositoryBase<Category>
{
    // diğer metodlar sonra eklenebilirler
    IQueryable<Category> GetAllCategories(bool trackchanges);
    void CreateCategory(Category category);
}