namespace Entities.Models;

public class Tag
{
    public int TagId { get; set; }
    public string? TagName { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}