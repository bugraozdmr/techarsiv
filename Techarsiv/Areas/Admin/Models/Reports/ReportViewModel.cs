namespace GargamelinBurnu.Areas.Admin.Models.Reports;

public class ReportViewModel
{
    public string Username { get; set; }
    public int reportId { get; set; }
    public string ReportUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Cause { get; set; }
}