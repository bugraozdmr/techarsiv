namespace Entities.Models;

public class Commentlike
{
    public int CommentId { get; set; }
    public Comment Comment { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}