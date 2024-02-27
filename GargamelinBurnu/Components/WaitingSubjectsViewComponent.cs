using Entities.Models;
using GargamelinBurnu.Views.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class WaitingSubjectsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public WaitingSubjectsViewComponent(IServiceManager manager,
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    public IViewComponentResult Invoke()
    {
        var userid = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => s.Id)
            .FirstOrDefault();

        // 5'ten fazla olmasın yine
        List<WaitingSubjectsViewModel> waitingSubjects = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Where(s => s.UserId.Equals(userid) && s.IsActive.Equals(false))
            .OrderByDescending(s => s.CreatedAt)
            .Include(s => s.User)
            .Include(s => s.Category)
            .Take(5)
            .Select(s => new WaitingSubjectsViewModel()
            {
                Image = s.User.Image,
                Username = s.User.UserName,
                Title = s.Title,
                CreatedAt = s.CreatedAt,
                categoryName = s.Category.CategoryName
            })
            .ToList();
        
        return View(waitingSubjects);
    }
}