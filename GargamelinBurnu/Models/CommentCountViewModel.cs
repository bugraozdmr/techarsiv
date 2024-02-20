using Entities.Models;

namespace GargamelinBurnu.Models;

public class CommentCountViewModel
{
    public User User { get; set; }
    public int Count {
        get;
        set;
    } 
}