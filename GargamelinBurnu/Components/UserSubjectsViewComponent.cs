using Entities.Models;
using Entities.RequestParameters;
using GargamelinBurnu.Infrastructure;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class UserSubjectsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;
    private readonly ILikeDService _likeDService;

    public UserSubjectsViewComponent(IServiceManager manager, 
        UserManager<User> userManager, ILikeDService likeDService)
    {
        _manager = manager;
        _userManager = userManager;
        _likeDService = likeDService;
    }
    
    public IViewComponentResult Invoke(CommonRequestParameters? p)
    {
        List<TitleViewModel> model;
        int total_count;

        // model to use
        var realModel = new SubjectListViewModel();

        
        if (User.Identity.IsAuthenticated)
        {
            var userId = _userManager
                .Users
                .Where(u => u.UserName.Equals(User.Identity.Name))
                .Select(u => u.Id).FirstOrDefault();
            
            
            model = _manager
                .SubjectService
                .GetAllSubjects(false)
                .OrderByDescending(s => s.CreatedAt)
                .Include(s => s.Comments)
                .Include(s => s.User)
                .Include(s => s.Category)
                .Where(s => s.UserId.Equals(userId) && s.IsActive.Equals(true))
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
                .Count();
            
    
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
        }

        realModel = realModel ?? new SubjectListViewModel();
        
        return View(realModel);
    }
}