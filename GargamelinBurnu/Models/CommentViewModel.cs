using Entities.Models;

namespace GargamelinBurnu.Models;

public class CommentViewModel
{
    public Comment Comment { get; set; }
    public string CommentUserName { get; set; }
    public string Content { get; set; }
    public int UserCommentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime CommentDate { get; set; }
}