namespace Entities.Models;

public class SubjectLike
{
    //public int SubjectLikeId { get; set; } // Birincil anahtar

    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}
