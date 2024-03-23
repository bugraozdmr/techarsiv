namespace GargamelinBurnu.Models.Question;

public class PaginationQuestionViewModel
{
    public List<QuestionViewModel> List { get; set; }
    public Pagination Pagination { get; set; }
    public int TotalCount => List.Count();
}