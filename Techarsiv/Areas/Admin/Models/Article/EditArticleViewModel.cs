namespace GargamelinBurnu.Areas.Admin.Models.Article;

public class EditArticleViewModel
{
    public int ArticleId { get; set; }
    public int TagId { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
}