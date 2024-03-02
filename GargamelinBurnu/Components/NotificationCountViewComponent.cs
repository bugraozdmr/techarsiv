using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class NotificationCountViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public NotificationCountViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public string Invoke()
    {
        return (_manager
            .NotificationService
            .GetAllNotification(false)
            .Include(s => s.User)
            .Count(s => s.User.UserName.Equals(User.Identity.Name))).ToString();
    }
}