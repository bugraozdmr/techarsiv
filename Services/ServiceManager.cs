using Services.Contracts;

namespace Services;

public class ServiceManager : IServiceManager
{
    private readonly ISubjectService _subjectService;
    private readonly ICategoryService _categoryService;
    private readonly ICommentService _commentService;

    public ServiceManager(ISubjectService subjectService
        , ICategoryService categoryService
        , ICommentService commentService)
    {
        _subjectService = subjectService;
        _categoryService = categoryService;
        _commentService = commentService;
    }

    public ISubjectService SubjectService => _subjectService;
    public ICategoryService CategoryService => _categoryService;
    public ICommentService CommentService => _commentService;
}