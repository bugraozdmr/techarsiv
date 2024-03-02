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
    private readonly INotificationRepository _notificationRepository;

    public RepositoryManager(RepositoryContext context
        , ISubjectRepository subjectRepository
        , ICategoryRepository categoryRepository
        , ICommentRepository commentRepository, 
        IBanRepository banRepository,
        INotificationRepository notificationRepository)
    {
        _context = context;
        _subjectRepository = subjectRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _banRepository = banRepository;
        _notificationRepository = notificationRepository;
    }


    public ISubjectRepository Subject => _subjectRepository;
    public ICategoryRepository Categories => _categoryRepository;
    public ICommentRepository Comments => _commentRepository;
    public IBanRepository Bans => _banRepository;
    public INotificationRepository Notification => _notificationRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}