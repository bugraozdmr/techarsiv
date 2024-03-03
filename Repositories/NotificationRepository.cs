using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class NotificationRepository : RepositoryBase<Notification>,INotificationRepository
{
    public NotificationRepository(RepositoryContext context) : base(context)
    {
    }

    public IQueryable<Notification> getAllNotifications(bool trackchanges) => FindAll(trackchanges);

    public void CreateNotification(Notification notification) => Create(notification);
    public void UpdateNotification(Notification notification) => Update(notification);
}