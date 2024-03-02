using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Subject
{
    public int SubjectId { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public string? Prefix { get; set; }
    
    public string? Title { get; set; }
    
    public string? Content { get; set; }


    public string UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int? categoryId { get; set; }
    public Category? Category { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();
    // comments ve tags eklenecek

    public List<SubjectLike> Likes { get; set; } = new List<SubjectLike>();

    public int LikeCount { get; set; } = 0;
    public int HeartCount { get; set; } = 0;
    public int DislikeCount { get; set; } = 0;
    
    public List<Notification> Notifications { get; set; } = new List<Notification>();
}