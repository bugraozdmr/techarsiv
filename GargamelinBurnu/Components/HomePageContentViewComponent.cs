using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class HomePageContentViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly ILikeDService _likeDService;

    public HomePageContentViewComponent(IServiceManager manager,
        ILikeDService likeDService)
    {
        _manager = manager;
        _likeDService = likeDService;
    }

    public IViewComponentResult Invoke(string? CategoryUrl)
    {
        List<TitleViewModel> model;

        if (CategoryUrl is not null)
        {
            model = _manager
                .SubjectService
                .GetAllSubjects(false)
                .OrderByDescending(s => s.CreatedAt)
                .Include(s => s.Comments)
                .Include(s => s.User)
                .Include(s => s.Category)
                .Where(s => s.Category.CategoryUrl.Equals(CategoryUrl))
                .Take(15)
                .Select(s => new TitleViewModel()
                {
                    Username = s.User.UserName,
                    CategoryName = s.Category.CategoryName,
                    Title = s.Title,
                    categoryUrl = s.Category.CategoryUrl,
                    createdAt = s.CreatedAt,
                    SubjectId = s.SubjectId,
                    Url = s.Url,
                    ImageUrl = s.User.Image,
                    CommentCount = s.Comments.Count,
                    Content = s.Content
                }).ToList();            
        }
        else
        {
            model = _manager
                .SubjectService
                .GetAllSubjects(false)
                .OrderByDescending(s => s.CreatedAt)
                .Include(s => s.Comments)
                .Include(s => s.User)
                .Include(s => s.Category)
                .Take(15)
                .Select(s => new TitleViewModel()
                {
                    Username = s.User.UserName,
                    CategoryName = s.Category.CategoryName,
                    categoryUrl = s.Category.CategoryUrl,
                    Title = s.Title,
                    createdAt = s.CreatedAt,
                    SubjectId = s.SubjectId,
                    Url = s.Url,
                    ImageUrl = s.User.Image,
                    CommentCount = s.Comments.Count,
                    Content = s.Content
                }).ToList();
        }
        

        foreach (var title in model)
        {
            title.HeartCount = _likeDService
                .Hearts
                .Count(s => s.SubjectId.Equals(title.SubjectId));
        }
        
        return View(model);
    }
}