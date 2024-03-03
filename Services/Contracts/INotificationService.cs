using Entities.Dtos.Notification;
using Entities.Models;

namespace Services.Contracts;

public interface INotificationService
{
    Task CreateNotification(NotificationDto dto);
    Task deleteAllNotification(string userId);
    void read(Notification not);
    IQueryable<Notification> GetAllNotification(bool trackChanges);
}