using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _context;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICommentRepository _commentRepository;

    public RepositoryManager(RepositoryContext context
        , ISubjectRepository subjectRepository
        , ICategoryRepository categoryRepository
        , ICommentRepository commentRepository)
    {
        _context = context;
        _subjectRepository = subjectRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
    }


    public ISubjectRepository Subject => _subjectRepository;
    public ICategoryRepository Categories => _categoryRepository;
    public ICommentRepository Comments => _commentRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}