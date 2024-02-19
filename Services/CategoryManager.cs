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

    public async Task<IEnumerable<Category>> GetAllCategories(bool trackChanges)
    {
        return await _manager.Categories.GetAllCategories(trackChanges).ToListAsync();
    }
}