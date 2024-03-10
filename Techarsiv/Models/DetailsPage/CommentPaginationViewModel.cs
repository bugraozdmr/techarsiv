namespace GargamelinBurnu.Models;

public class CommentPaginationViewModel
{
    public List<CommentViewModel> List { get; set; }
    public Pagination Pagination { get; set; }
    public string Action { get; set; }
}