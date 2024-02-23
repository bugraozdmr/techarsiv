namespace GargamelinBurnu.Models;

public class TitleViewModel
{
    public string Username { get; set; }
    public string Content { get; set; }
    public string CategoryName { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int CommentCount { get; set; }
    public int HeartCount { get; set; }
    public int SubjectId { get; set; }
    public DateTime createdAt { get; set; }
}