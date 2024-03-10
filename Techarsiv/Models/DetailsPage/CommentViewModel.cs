using Entities.Models;

namespace GargamelinBurnu.Models;

public class CommentViewModel
{
    public Comment Comment { get; set; }
    public int CommentId { get; set; }
    
    public string CommentUserName { get; set; }
    public string CommentUserSignature { get; set; }
    public string CommentUserImage { get; set; }
    public string Username { get; set; }
    
    public string Content { get; set; }
    public int UserCommentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime CommentDate { get; set; }
    
    
    public int likeCount { get; set; }
    public bool isLiked { get; set; }
    
    public int dislikeCount { get; set; }
    public bool isdisLiked { get; set; }

}