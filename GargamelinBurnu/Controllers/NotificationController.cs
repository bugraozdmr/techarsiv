using Entities.Models;
using Entities.RequestParameters;
using GargamelinBurnu.Models;
using GargamelinBurnu.Models.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> notifications(CommonRequestParameters p)
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
            .Skip((p.PageNumber-1)*(p.Pagesize))
            .Take(p.Pagesize)
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

        
        // pagination
        var realModel = new PaginationNotificationViewModel();
        realModel.List = model;
        
        int total_count = _manager
            .NotificationService
            .GetAllNotification(false)
            .Include(s => s.User)
            .Count(s => s.User.UserName.Equals(User.Identity.Name));
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };
        
        realModel.Pagination = pagination;
        
        return View(realModel);
    }

    [Authorize]
    public async Task<IActionResult> removeAllNotifications()
    {
        string userid = _userManager
            .Users
            .Where(u => u.UserName.Equals(User.Identity.Name))
            .Select(s => s.Id)
            .FirstOrDefault();
        
        
        await _manager.NotificationService.deleteAllNotification(userid);

        return RedirectToAction("notifications");
    }
}