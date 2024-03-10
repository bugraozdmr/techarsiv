using GargamelinBurnu.Models;

namespace GargamelinBurnu.Areas.Admin.Models.Tables;

public class TableSubjectViewModel
{
    public string Title { get; set; }
    public string Username { get; set; }
    public string SubjectUrl { get; set; }
    public string categoryName { get; set; }
    public DateTime CreatedAt { get; set; }

    public Pagination Pagination { get; set; }
}