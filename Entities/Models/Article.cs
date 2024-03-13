namespace Entities.Models;

public class Article
{
    public int ArticleId { get; set; }
    public string Url { get; set; }
    public int TagId { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string UserId { get; set; }
    public User User { get; set; } = null!;
}