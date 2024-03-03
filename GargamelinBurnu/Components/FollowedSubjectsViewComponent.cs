using Entities.Models;
using Entities.RequestParameters;
using GargamelinBurnu.Infrastructure;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class FollowedSubjectsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public FollowedSubjectsViewComponent(IServiceManager manager, 
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    public IViewComponentResult Invoke(CommonRequestParameters? p)
    {
        List<TitleViewModel> model = new List<TitleViewModel>();
        int total_count;

        // model to use
        var realModel = new SubjectListViewModel();

        
        if (User.Identity.IsAuthenticated)
        {
            var userId = _userManager
                .Users
                .Where(u => u.UserName.Equals(User.Identity.Name))
                .Select(u => u.Id).FirstOrDefault();


            List<int> ids = _manager
                .FollowingSubjects
                .FSubjects
                .Where(s => s.UserId.Equals(userId))
                .Skip(p.Pagesize*(p.PageNumber-1))
                .Take(p.Pagesize)
                .Select(s => s.SubjectId)
                .ToList();

            foreach (var id in ids)
            {
                var subject = _manager
                    .SubjectService
                    .GetAllSubjects(false)
                    .Include(s => s.Comments)
                    .Include(s => s.User)
                    .Include(s => s.Category)
                    .Where(s => s.SubjectId.Equals(id) && s.IsActive.Equals(true))
                    .OrderByDescending(s => s.CreatedAt)
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
                    }).FirstOrDefault();

                model.Add(subject);
            }

            model = model.OrderByDescending(s => s.createdAt).ToList();
            
            
            total_count = _manager
                .FollowingSubjects
                .FSubjects
                .Count(s => s.UserId.Equals(userId));
            
    
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