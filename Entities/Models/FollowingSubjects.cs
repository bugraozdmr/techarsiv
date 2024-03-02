namespace Entities.Models;

public class FollowingSubjects
{
    public string UserId { get; set; }
    public User User { get; set; }
    
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

}