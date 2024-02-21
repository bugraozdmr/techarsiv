using Entities.Models;

namespace GargamelinBurnu.Models;

public class CommentCountViewModel
{
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; }
    public int Count {
        get;
        set;
    } 
}