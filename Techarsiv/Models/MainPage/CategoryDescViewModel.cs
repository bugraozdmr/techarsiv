
namespace GargamelinBurnu.Models;

public class CategoryDescViewModel
{
    public int? CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryUrl { get; set; }
    public string Icon { get; set; }
    public string? LastSubjectName { get; set; }
    public string? LastSubjectUser { get; set; }
    public string? LastSubjectUrl { get; set; }
    public DateTime? LastCreatedAt { get; set; }
    public int? catSubCount { get; set; }
    public int? catComCount { get; set; }
}