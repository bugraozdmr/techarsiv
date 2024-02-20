namespace Entities.Models;

public class SubjectDislike
{

    
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}