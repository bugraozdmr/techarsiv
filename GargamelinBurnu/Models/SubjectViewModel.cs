using Entities.Models;

namespace GargamelinBurnu.Models;

public class SubjectViewModel
{
    public UserSubjectLdhViewModel UserSubjectLdh { get; set; }
    public int CommentCount { get; set; }
    public Subject Subject { get; set; }
    public Category Category { get; set; }
    public string UserName { get; set; }
    public int UserCommentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CommentViewModel> Comments { get; set; }

    public int likeCount { get; set; }
    public bool isLiked { get; set; }
    
    public int dislikeCount { get; set; }
    public bool isdisLiked { get; set; }
    public int heartCount { get; set; }
    public bool isheart { get; set; }
}