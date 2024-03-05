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
        var groupedCategories = _manager
            .CategoryService
            .GetAllCategories(false)
            .GroupBy(c => c.CommonFilter) // CommonFilter'a göre grupla
            .ToDictionary(
                group => group.Key, // Anahtar olarak CommonFilter kullan
                group => group.Select(c => new CategoryDescViewModel()
                {
                    CategoryName = c.CategoryName,
                    CategoryId = c.CategoryId,
                    CategoryUrl = c.CategoryUrl
                }).ToList()
            );

        var model = groupedCategories.Select(kv => new GetAllSubjectsViewModel
        {
            Key = kv.Key,
            List = kv.Value
        }).ToList();

        
        
        foreach (var cat in model)
        {
            foreach (var category in cat.List)
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
                    .Where(s => s.categoryId.Equals(category.CategoryId) && s.IsActive)
                    .Select(s => new CategoryDescHelper()
                    {
                        LastCreatedAt = s.CreatedAt,
                        LastSubjectName = s.Title,
                        LastSubjectUser = s.User.UserName,
                        LastSubjectUrl = s.Url
                    }).FirstOrDefault();

                if (helperModel?.LastCreatedAt is not null)
                {
                    category.LastCreatedAt = helperModel?.LastCreatedAt;    
                }
            
                category.LastSubjectUser = helperModel?.LastSubjectUser ?? "";
                category.LastSubjectName = helperModel?.LastSubjectName ?? "";
                category.LastSubjectUrl = helperModel?.LastSubjectUrl ?? "";
            }
        }
        
        return View(model);
    }
}