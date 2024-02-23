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
    
    public IViewComponentResult Invoke()
    {
        List<TitleViewModel> model;
        
        model = _manager
            .CommentService
            .getAllComments(false)
            .Include(c => c.Subject)
            .ThenInclude(s => s.Category)
            .OrderByDescending(c => c.CreatedAt)
            .Take(15)
            .Select(c => new TitleViewModel()
            {
                createdAt = c.CreatedAt,
                CategoryName = c.Subject.Category.CategoryName,
                Title = c.Subject.Title,
                Url = c.Subject.Url,
                Username = c.User.UserName,
                Content = c.Text
            }).ToList();


        return View(model);
    }
}