namespace Entities.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; } = string.Empty;
    public string? Icon { get; set; } = string.Empty;
    public List<Subject> Subjects { get; set; }
    public string? Description { get; set; }
}