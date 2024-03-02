using Entities.Models;
using GargamelinBurnu.Models.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class NotificationController : Controller
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public NotificationController(IServiceManager manager,
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    [Authorize]
    public IActionResult notifications()
    {
        // zaten giriş yapmazsa erişemez

        List<NotificationViewModel> model;
        
        model = _manager
            .NotificationService
            .GetAllNotification(false)
            .Include(s => s.User)
            .Include(s => s.Subject)
            .ThenInclude(s => s.User)
            .Include(s => s.Comment)
            .ThenInclude(s => s.User)
            .Where(s => s.User.UserName.Equals(User.Identity.Name))
            .Select(s => new NotificationViewModel()
            {
                CreatedAt = s.createdAt,
                titleUrl = s.Subject.Url,
                title = s.Subject.Title,
                userImage = s.Comment.User.Image,
                username = s.Comment.User.UserName
            })
            .ToList();

        

        return View(model);
    }
}