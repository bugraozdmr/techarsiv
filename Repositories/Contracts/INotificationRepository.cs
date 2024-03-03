using Entities.Models;

namespace Repositories.Contracts;

public interface INotificationRepository
{
    IQueryable<Notification> getAllNotifications(bool trackchanges);
    void CreateNotification(Notification notification);
    void UpdateNotification(Notification notification);
}