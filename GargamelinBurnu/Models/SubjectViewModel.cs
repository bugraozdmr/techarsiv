using Entities.Models;

namespace GargamelinBurnu.Models;

public class SubjectViewModel
{
    public int CommentCount { get; set; }
    public Subject Subject { get; set; }
    public Category Category { get; set; }
    public string UserName { get; set; }
    public int UserCommentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CommentViewModel> Comments { get; set; }
}