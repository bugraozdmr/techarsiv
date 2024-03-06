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

    public RepositoryManager(RepositoryContext context
        , ISubjectRepository subjectRepository
        , ICategoryRepository categoryRepository
        , ICommentRepository commentRepository, 
        IBanRepository banRepository,
        INotificationRepository notificationRepository, 
        IReportRepository reportRepository)
    {
        _context = context;
        _subjectRepository = subjectRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _banRepository = banRepository;
        _notificationRepository = notificationRepository;
        _reportRepository = reportRepository;
    }


    public ISubjectRepository Subject => _subjectRepository;
    public ICategoryRepository Categories => _categoryRepository;
    public ICommentRepository Comments => _commentRepository;
    public IBanRepository Bans => _banRepository;
    public INotificationRepository Notification => _notificationRepository;
    public IReportRepository ReportRepository => _reportRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}