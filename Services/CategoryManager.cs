using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Services.Contracts;

namespace Services;

public class CategoryManager : ICategoryService
{
    private readonly IRepositoryManager _manager;

    public CategoryManager(IRepositoryManager manager)
    {
        _manager = manager;
    }

    public void CreateCategory(Category category)
    {
        
    }

    public IQueryable<Category> GetAllCategories(bool trackChanges)
    {
        return _manager.Categories.GetAllCategories(trackChanges);
    }
}