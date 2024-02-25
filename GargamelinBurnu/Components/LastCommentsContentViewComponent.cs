using Entities.RequestParameters;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class LastCommentsContentViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public LastCommentsContentViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }
    
    public IViewComponentResult Invoke(SubjectRequestParameters? p)
    {
        List<TitleViewModel> model;
        int total_count;
        
        model = _manager
            .CommentService
            .getAllComments(false)
            .Include(c => c.Subject)
            .ThenInclude(s => s.Category)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(p.Pagesize*(p.PageNumber-1))
            .Take(p.Pagesize)
            .Select(c => new TitleViewModel()
            {
                createdAt = c.CreatedAt,
                CategoryName = c.Subject.Category.CategoryName,
                categoryUrl = c.Subject.Category.CategoryUrl,
                Title = c.Subject.Title,
                Url = c.Subject.Url,
                Username = c.User.UserName,
                Content = c.Text,
                ImageUrl = c.User.Image
            }).ToList();

        total_count = _manager.CommentService.getAllComments(false).Count();
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };


        var realModel = new IndexCommentsViewModel()
        {
            List = model,
            Pagination = pagination
        };
        
        return View(realModel);
    }
}