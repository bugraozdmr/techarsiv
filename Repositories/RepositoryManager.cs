using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _context;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IBanRepository _banRepository;

    public RepositoryManager(RepositoryContext context
        , ISubjectRepository subjectRepository
        , ICategoryRepository categoryRepository
        , ICommentRepository commentRepository, 
        IBanRepository banRepository)
    {
        _context = context;
        _subjectRepository = subjectRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _banRepository = banRepository;
    }


    public ISubjectRepository Subject => _subjectRepository;
    public ICategoryRepository Categories => _categoryRepository;
    public ICommentRepository Comments => _commentRepository;
    public IBanRepository Bans => _banRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}