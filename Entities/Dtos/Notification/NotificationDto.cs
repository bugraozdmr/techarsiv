namespace Entities.Dtos.Notification;

public class NotificationDto
{
    // kullanıcıdan alınmadığı için sorun yok recod değil
    public string UserId { get; set; }
    public int SubjectId { get; set; }
    public int CommentId { get; set; }
}