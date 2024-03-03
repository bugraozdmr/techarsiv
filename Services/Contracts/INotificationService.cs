using Entities.Dtos.Notification;
using Entities.Models;

namespace Services.Contracts;

public interface INotificationService
{
    Task CreateNotification(NotificationDto dto);
    void read(Notification not);
    IQueryable<Notification> GetAllNotification(bool trackChanges);
}