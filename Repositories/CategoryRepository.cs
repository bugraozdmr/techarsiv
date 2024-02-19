using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;


namespace Repositories;

public class CategoryRepository : RepositoryBase<Category>,ICategoryRepository
{
    public CategoryRepository(RepositoryContext context) : base(context)
    {
    }

    public IQueryable<Category> GetAllCategories(bool trackchanges) => FindAll(trackchanges);

    public void CreateCategory(Category category) => Create(category);
}