using GargamelinBurnu.Models;

namespace GargamelinBurnu.Infrastructure;

public class SubjectListViewModel
{
    public List<TitleViewModel> List { get; set; }
    public string Action { get; set; }
    public string Param { get; set; }
    public string CategoryName { get; set; }
    public Pagination Pagination { get; set; }
    public int TotalCount => List.Count();
}