using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class GetSubjectsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public GetSubjectsViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke()
    {
        List<CategoryDescViewModel> model;
        
        model = _manager
            .CategoryService
            .GetAllCategories(false)
            .Select(s => new CategoryDescViewModel()
            {
                CategoryName = s.CategoryName,
                CategoryId = s.CategoryId,
                Icon = s.Icon
            }).ToList();

        foreach (var category in model)
        {
            category.catSubCount = _manager
                .SubjectService
                .GetAllSubjects(false)
                .Count(s => s.categoryId.Equals(category.CategoryId));

            category.catComCount = _manager
                .CommentService
                .getAllComments(false)
                .Include(c => c.Subject)
                .Count(c => c.Subject.categoryId.Equals(category.CategoryId));

            CategoryDescHelper helperModel;

            helperModel = _manager
                .SubjectService
                .GetAllSubjects(false)
                .OrderByDescending(s => s.CreatedAt)
                .Include(s => s.User)
                .Where(s => s.categoryId.Equals(category.CategoryId))
                .Select(s => new CategoryDescHelper()
                {
                    LastCreatedAt = s.CreatedAt,
                    LastSubjectName = s.Title,
                    LastSubjectUser = s.User.UserName
                }).FirstOrDefault();

            category.LastCreatedAt = helperModel.LastCreatedAt;
            category.LastSubjectUser = helperModel.LastSubjectUser;
            category.LastSubjectName = helperModel.LastSubjectName;
        }
        
        return View(model);
    }
}