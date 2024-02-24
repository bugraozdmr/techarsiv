using Entities.Models;

namespace GargamelinBurnu.Models;

public class CommentDetailsViewModel
{
    public string Username { get; set; }
    public string? userSignature { get; set; }
    public string? userImage { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public User User { get; set; }
    public int Count { get; set; } 
}