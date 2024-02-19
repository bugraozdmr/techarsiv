using Services.Contracts;

namespace Services;

public class ServiceManager : IServiceManager
{
    private readonly ISubjectService _subjectService;
    private readonly ICategoryService _categoryService;

    public ServiceManager(ISubjectService subjectService
        , ICategoryService categoryService)
    {
        _subjectService = subjectService;
        _categoryService = categoryService;
    }

    public ISubjectService SubjectService => _subjectService;
    public ICategoryService CategoryService => _categoryService;
}