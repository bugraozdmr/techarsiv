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
    private readonly IReportRepository _reportRepository;
    private readonly IArticleRepository _articleRepository;

    public RepositoryManager(RepositoryContext context
        , ISubjectRepository subjectRepository
        , ICategoryRepository categoryRepository
        , ICommentRepository commentRepository, 
        IBanRepository banRepository,
        INotificationRepository notificationRepository, 
        IReportRepository reportRepository,
        IArticleRepository articleRepository)
    {
        _context = context;
        _subjectRepository = subjectRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _banRepository = banRepository;
        _notificationRepository = notificationRepository;
        _reportRepository = reportRepository;
        _articleRepository = articleRepository;
    }


    public ISubjectRepository Subject => _subjectRepository;
    public ICategoryRepository Categories => _categoryRepository;
    public ICommentRepository Comments => _commentRepository;
    public IBanRepository Bans => _banRepository;
    public INotificationRepository Notification => _notificationRepository;
    public IReportRepository ReportRepository => _reportRepository;
    public IArticleRepository Article => _articleRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}