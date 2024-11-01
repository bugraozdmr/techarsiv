using Entities.Models;

namespace Entities.Dtos.Article;

public class UpdateArticleDto
{
    public int ArticleId { get; set; }
    public string Url { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }

    public string image { get; set; }
}