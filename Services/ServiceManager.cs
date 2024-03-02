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

    public ServiceManager(ISubjectService subjectService
        , ICategoryService categoryService
        , ICommentService commentService
        , ILikeDService likeDService
        , ICommentLikeDService commentLikeDService,
        IBanService banService,
        IAwardUserService awardUserService,
        IAwardService awardService)
    {
        _subjectService = subjectService;
        _categoryService = categoryService;
        _commentService = commentService;
        _likeDService = likeDService;
        _commentLikeDService = commentLikeDService;
        _banService = banService;
        _awardUserService = awardUserService;
        _awardService = awardService;
    }

    public ISubjectService SubjectService => _subjectService;
    public ICategoryService CategoryService => _categoryService;
    public ICommentService CommentService => _commentService;
    public ILikeDService LikeDService => _likeDService;
    public ICommentLikeDService CommentLikeDService => _commentLikeDService;
    public IBanService BanService => _banService;
    public IAwardService AwardService => _awardService;
    public IAwardUserService AwardUserService => _awardUserService;
}