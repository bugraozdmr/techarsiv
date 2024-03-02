namespace Entities.Models;

public class Notification
{
    public int Id { get; set; }
    public DateTime createdAt { get; set; }
    
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    
    
    public int CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
}