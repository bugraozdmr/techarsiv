using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class MostHeartViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    

    public MostHeartViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke()
    {
        List<MostHeartViewModel> model; 
        model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.User)
            .Include(s => s.Comments)
            .OrderByDescending(s => s.HeartCount)
            .Take(6)
            .Select(s => new MostHeartViewModel
            {
                owner = s.User.UserName,
                Title = s.Title,
                Url = s.Url,
                CreatedAt = s.CreatedAt,
                MessageCount = s.Comments.Count,
                HeartCount = s.HeartCount
            }).ToList();
        
        return View(model);
    }
}