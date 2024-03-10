using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class CheckSubjectUserViewComponent : ViewComponent
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;

    public CheckSubjectUserViewComponent(UserManager<User> userManager, 
        IServiceManager manager)
    {
        _userManager = userManager;
        _manager = manager;
    }

    public string Invoke(int subjectId)
    {
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserId = s.Id })
            .FirstOrDefault();

        if (user is not null)
        {
            var subject = _manager.SubjectService
                .GetAllSubjects(false)
                .Where(s => s.SubjectId.Equals(subjectId) && s.UserId.Equals(user.UserId))
                .Select(s => new
                {
                    SubjectId = s.SubjectId
                })
                .FirstOrDefault();

            if (subject is not null)
            {
                return "true";
            }
        }
        
        return "false";
    }
}