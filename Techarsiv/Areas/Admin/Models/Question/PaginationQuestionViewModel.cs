using GargamelinBurnu.Models;

namespace GargamelinBurnu.Areas.Admin.Models.Question;

public class PaginationQuestionViewModel
{
    public Pagination Pagination { get; set; }
    public List<QuestionViewModel> List { get; set; }
    public string? Param { get; set; }
    public string? area { get; set; }
}