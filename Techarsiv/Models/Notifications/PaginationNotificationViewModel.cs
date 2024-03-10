namespace GargamelinBurnu.Models.Notifications;

public class PaginationNotificationViewModel
{
    public List<NotificationViewModel> List { get; set; }
    public Pagination Pagination { get; set; }
    public int TotalCount => List.Count();
}