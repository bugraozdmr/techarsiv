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
        await _manager.SaveAsync();
    }

    public IQueryable<Notification> GetAllNotification(bool trackChanges)
    {
        return _manager.Notification.getAllNotifications(trackChanges);
    }
}