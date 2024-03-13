using Services.Contracts;

namespace Services;

public class ServiceManager : IServiceManager
{
    private readonly ISubjectService _subjectService;
    private readonly ICategoryService _categoryService;
    private readonly ICommentService _commentService;
    private readonly ILikeDService _likeDService;
    private readonly ICommentLikeDService _commentLikeDService;
    private readonly IBanService _banService;
    private readonly IAwardUserService _awardUserService;
    private readonly IAwardService _awardService;
    private readonly INotificationService _notificationService;
    private readonly IFollowingSubjects _FollowingSubjects;
    private readonly IReportService _reportService;
    private readonly IArticleService _articleService;

    
    public ServiceManager(ISubjectService subjectService
        , ICategoryService categoryService
        , ICommentService commentService
        , ILikeDService likeDService
        , ICommentLikeDService commentLikeDService,
        IBanService banService,
        IAwardUserService awardUserService,
        IAwardService awardService,
        INotificationService notificationService,
        IFollowingSubjects followingSubjects,
        IReportService reportService,
        IArticleService articleService)
    {
        _subjectService = subjectService;
        _categoryService = categoryService;
        _commentService = commentService;
        _likeDService = likeDService;
        _commentLikeDService = commentLikeDService;
        _banService = banService;
        _awardUserService = awardUserService;
        _awardService = awardService;
        _notificationService = notificationService;
        _FollowingSubjects = followingSubjects;
        _reportService = reportService;
        _articleService = articleService;
    }

    public ISubjectService SubjectService => _subjectService;
    public ICategoryService CategoryService => _categoryService;
    public ICommentService CommentService => _commentService;
    public ILikeDService LikeDService => _likeDService;
    public ICommentLikeDService CommentLikeDService => _commentLikeDService;
    public IBanService BanService => _banService;
    public IAwardService AwardService => _awardService;
    public INotificationService NotificationService => _notificationService;
    public IFollowingSubjects FollowingSubjects => _FollowingSubjects;
    public IAwardUserService AwardUserService => _awardUserService;
    public IReportService ReportService => _reportService;
    public IArticleService ArticleService => _articleService;
}