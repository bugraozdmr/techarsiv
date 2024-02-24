using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class LastCommentsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public LastCommentsViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke()
    {
        List<CommentCardViewModel> model = _manager
            .CommentService
            .getAllComments(false)
            .Include(c => c.User)
            .Include(c => c.Subject)
            .ThenInclude(s => s.Category)
            .OrderByDescending(c => c.CreatedAt)
            .Take(15)
            .Select(s => new CommentCardViewModel()
            {
                SubjectTitle = s.Subject.Title,
                CreatedAt = s.CreatedAt,
                Url = s.Subject.Url,
                CategoryName = s.Subject.Category.CategoryName,
                categoryUrl = s.Subject.Category.CategoryUrl,
                username = s.User.UserName,
                UserImage = s.User.Image
            }).ToList();
        
        return View(model);
    }
}