using Entities.Models;
using GargamelinBurnu.Models;
using GargamelinBurnu.Models.Userpage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;

    public UserController(UserManager<User> userManager,
        IServiceManager manager)
    {
        _userManager = userManager;
        _manager = manager;
    }

    [HttpGet("/biri/{username}")]
    public IActionResult Index(string username)
    {
        UserPageViewModel model = new UserPageViewModel();

        // tüm her şey geliyor düzeltilebilir belki
        var user = _userManager
            .Users
            .Where(u => u.UserName.Equals(username))
            .FirstOrDefault();

        model.User = user;

        // likecount
        model.LikeCount = _manager
                  .LikeDService
                  .Likes
                  .Count(l => l.UserId.Equals(user.Id)) +
              _manager
                  .CommentLikeDService
                  .CLikes
                  .Count(l => l.UserId.Equals(user.Id));

        // commentcount
        model.CommentCount = _manager
            .CommentService
            .getAllComments(false)
            .Count(c => c.UserId.Equals(user.Id));
        
        // subjectcount
        model.SubjectCount = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Count(s => s.UserId.Equals(user.Id));

        // titleViewModel uyuyor istediğim özelliklere
        model.Comments = _manager
            .CommentService
            .getAllComments(false)
            .Include(c => c.User)
            .Include(c => c.Subject)
            .ThenInclude(s => s.Category)
            .OrderByDescending(c => c.CreatedAt)
            .Where(c => c.UserId.Equals(user.Id))
            .Take(10)
            .Select(c => new TitleViewModel()
            {
                Username = c.User.UserName,
                Url = c.Subject.Url,
                CategoryName = c.Subject.Category.CategoryName,
                createdAt = c.CreatedAt,
                Title = c.Subject.Title,
                Content = c.Text
            }).ToList();

        model.Subjects = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.User)
            .Include(s => s.Comments)
            .Include(s => s.Category)
            .Where(s => s.UserId.Equals(user.Id))
            .Select(s => new TitleViewModel()
            {
                Title = s.Title,
                Username = s.User.UserName,
                createdAt = s.CreatedAt,
                CommentCount = s.Comments.Count(),
                CategoryName = s.Category.CategoryName
            }).ToList();
        
        return View(model);
    }
}