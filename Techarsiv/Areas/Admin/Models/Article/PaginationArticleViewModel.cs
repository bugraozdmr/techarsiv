using GargamelinBurnu.Models;

namespace GargamelinBurnu.Areas.Admin.Models.Article;

public class PaginationArticleViewModel
{
    public Pagination Pagination { get; set; }
    public List<ArticleViewModel> List { get; set; }
    public string? Param { get; set; }
    public string? area { get; set; }
}