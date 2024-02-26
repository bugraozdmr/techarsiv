using Entities.RequestParameters;
using GargamelinBurnu.Infrastructure;
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

    public IViewComponentResult Invoke(string? CategoryUrl,SubjectRequestParameters? p,string action,string category)
    {
        List<TitleViewModel> model;
        int total_count;

        // model to use
        var realModel = new SubjectListViewModel();
        
        
        if (CategoryUrl is not null && CategoryUrl != "")
        {
            model = _manager
                .SubjectService
                .GetAllSubjects(false)
                .OrderByDescending(s => s.CreatedAt)
                .Include(s => s.Comments)
                .Include(s => s.User)
                .Include(s => s.Category)
                .Where(s => s.Category.CategoryUrl.Equals(CategoryUrl) && s.IsActive.Equals(true))
                .Skip(p.Pagesize*(p.PageNumber-1))
                .Take(p.Pagesize)
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

            total_count = _manager
                .SubjectService
                .GetAllSubjects(false)
                .Include(s => s.Category)
                .Count(s => s.Category.CategoryUrl.Equals(CategoryUrl) && s.IsActive.Equals(true));
        }
        else
        {
            if (p.SearchTerm is not null && p.SearchTerm != "")
            {
                model = _manager
                    .SubjectService
                    .GetAllSubjects(false)
                    .OrderByDescending(s => s.CreatedAt)
                    .Include(s => s.Comments)
                    .Include(s => s.User)
                    .Include(s => s.Category)
                    .Where(s => s.Title.Contains(p.SearchTerm.ToLower())  && s.IsActive.Equals(true))
                    .Skip(p.Pagesize*(p.PageNumber-1))
                    .Take(p.Pagesize)
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

                total_count = _manager
                    .SubjectService
                    .GetAllSubjects(false)
                    .Count(s => s.Title.Contains(p.SearchTerm.ToLower()) && s.IsActive.Equals(true));
                realModel.Param = $"SearchTerm={p.SearchTerm}";
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
                    .Where(s => s.IsActive.Equals(true))
                    .Skip(p.Pagesize*(p.PageNumber-1))
                    .Take(p.Pagesize)
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

                total_count = _manager
                    .SubjectService
                    .GetAllSubjects(false)
                    .Count(s => s.IsActive.Equals(true));
            }
        }
        

        foreach (var title in model)
        {
            title.HeartCount = _likeDService
                .Hearts
                .Count(s => s.SubjectId.Equals(title.SubjectId));
        }

        
        realModel.List = model;

        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        realModel.Pagination = pagination;
        realModel.CategoryName = category;
        realModel.Action = action;
        
        return View(realModel);
    }
}