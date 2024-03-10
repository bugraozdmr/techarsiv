using GargamelinBurnu.Models;

namespace GargamelinBurnu.Areas.Admin.Models.Tables;

public class PaginationSubjectListViewModel
{
    public Pagination Pagination { get; set; }
    public List<TableSubjectViewModel> List { get; set; }
    public string? Param { get; set; }
    public string? area { get; set; }
}