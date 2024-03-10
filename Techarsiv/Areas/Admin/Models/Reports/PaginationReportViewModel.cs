using GargamelinBurnu.Models;

namespace GargamelinBurnu.Areas.Admin.Models.Reports;

public class PaginationReportViewModel
{
    public Pagination Pagination { get; set; }
    public List<ReportViewModel> List { get; set; }
}