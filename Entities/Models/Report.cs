namespace Entities.Models;

public class Report
{
    public int ReportId { get; set; }
    public string UserId { get; set; }
    public User User { get; set; } = null!;

    // raporlayan kisi
    public String username { get; set; }
    
    public string Cause { get; set; }
    public DateTime CreatedAt { get; set; }
}