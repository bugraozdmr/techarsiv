using Entities.Models;

namespace GargamelinBurnu.Models;

public class CategoryDescViewModel
{
    public int? CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryDesc { get; set; }
    public string? LastSubjectName { get; set; }
    public string? LastSubjectUser { get; set; }
    public DateTime? LastCreatedAt { get; set; }
    public int? catSubCount { get; set; }
    public int? catComCount { get; set; }
}