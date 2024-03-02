namespace GargamelinBurnu.Models.Notifications;

public class NotificationViewModel
{
    public int SubjectId { get; set; }
    public string title { get; set; }
    public string titleUrl { get; set; }
    public string username { get; set; }
    public string userImage { get; set; }
    public DateTime CreatedAt { get; set; }
}