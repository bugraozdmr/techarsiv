namespace Entities.Models;

public class Category
{
     
    public int CommonFilter { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; } = string.Empty;
    public string? CategoryUrl { get; set; } = string.Empty;
    public List<Subject> Subjects { get; set; }
    public string? Description { get; set; }
}