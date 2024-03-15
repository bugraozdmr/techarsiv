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

    public async Task<string> InvokeAsync()
    {
        int count = _manager
            .NotificationService
            .GetAllNotification(false)
            .Include(s => s.User)
            .Count(s => s.User.UserName.Equals(User.Identity.Name) && s.read.Equals(false));
        
        return count.ToString();
    }
}