using Entities.Models;
using GargamelinBurnu.Models.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EF;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class NotificationController : Controller
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;
    private readonly RepositoryContext _context;

    public NotificationController(IServiceManager manager,
        UserManager<User> userManager, 
        RepositoryContext context)
    {
        _manager = manager;
        _userManager = userManager;
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> notifications()
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
            .OrderByDescending(s => s.createdAt)
            .Select(s => new NotificationViewModel()
            {
                CreatedAt = s.createdAt,
                titleUrl = s.Subject.Url,
                title = s.Subject.Title,
                userImage = s.Comment.User.Image,
                username = s.Comment.User.UserName
            })
            .ToList();

        List<Notification> changes = _manager
            .NotificationService
            .GetAllNotification(false)
            .Where(s => s.User.UserName.Equals(User.Identity.Name))
            .ToList();

        foreach (Notification item in changes)
        {
            _manager.NotificationService.read(item);
        }


        // save lazim burda
        await _context.SaveChangesAsync();

        return View(model);
    }
}