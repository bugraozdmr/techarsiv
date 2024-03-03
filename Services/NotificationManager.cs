using AutoMapper;
using Entities.Dtos.Notification;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services;

public class NotificationManager : INotificationService
{
    private readonly IRepositoryManager _manager;
    private readonly IMapper _mapper;

    public NotificationManager(IRepositoryManager manager, 
        IMapper mapper)
    {
        _manager = manager;
        _mapper = mapper;
    }

    public async Task CreateNotification(NotificationDto dto)
    {
        var not = _mapper.Map<Notification>(dto);

        not.createdAt = DateTime.Now;
        
        _manager.Notification.CreateNotification(not);
        
        // notification save işlemi en sona kalsın diye controllere gidiyor sistem yoruluyor.
        
    }

    public async Task deleteAllNotification(string UserId)
    {
        var not = _manager
            .Notification
            .getAllNotifications(false)
            .Where(s => s.UserId.Equals(UserId))
            .ToList();


        foreach (var item in not)
        {
            _manager.Notification.deleteNotification(item);
        }

        // en son save
        await _manager.SaveAsync();
    }

    public void read(Notification not)
    {
        // bu kadar
        not.read = true;
        _manager.Notification.UpdateNotification(not);
    }

    public IQueryable<Notification> GetAllNotification(bool trackChanges)
    {
        return _manager.Notification.getAllNotifications(trackChanges);
    }
}