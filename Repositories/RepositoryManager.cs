using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _context;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICategoryRepository _categoryRepository;

    public RepositoryManager(RepositoryContext context
        , ISubjectRepository subjectRepository
        , ICategoryRepository categoryRepository)
    {
        _context = context;
        _subjectRepository = subjectRepository;
        _categoryRepository = categoryRepository;
    }


    public ISubjectRepository Subject => _subjectRepository;
    public ICategoryRepository Categories => _categoryRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}