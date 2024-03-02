namespace Entities.Models;

public class Comment
{
    public int CommentId { get; set; }
    public string? Text { get; set; }
    
    public DateTime CreatedAt { get; set; }

    
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public string UserId { get; set; }
    public User User { get; set; } = null!;
    
    public List<Notification> Notifications { get; set; } = new List<Notification>();
}