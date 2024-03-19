namespace GargamelinBurnu.Areas.Main.Models.ArticlePage;

public class ArticlePageViewModel
{
    public string Title { get; set; }
    public int TagId { get; set; }
    public int ArticleId { get; set; }
    public string image { get; set; }
    
    public string Username { get; set; }
    public string Url { get; set; }
    public string SubTitle { get; set; }
    public string TagName { get; set; }
    public string TagUrl { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}