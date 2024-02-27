using Entities.Models;
using Entities.RequestParameters;

namespace GargamelinBurnu.Models;

public class SubjectViewModel
{
    // for preview page -- do not use for others
    public string Title { get; set; }
    public string Content { get; set; }
    
    public SubjectRequestParameters p { get; set; }
    
    public UserSubjectLdhViewModel UserSubjectLdh { get; set; }
    public bool isMain { get; set; }
    public int CommentCount { get; set; }
    public string UserSignature { get; set; }
    public Subject Subject { get; set; }
    public Category Category { get; set; }
    public string UserName { get; set; }
    public string UserImage { get; set; }
    public int UserCommentCount { get; set; }
    public DateTime CreatedAt { get; set; }

    public int likeCount { get; set; }
    public bool isLiked { get; set; }
    
    public int dislikeCount { get; set; }
    public bool isdisLiked { get; set; }
    public int heartCount { get; set; }
    public bool isheart { get; set; }
}