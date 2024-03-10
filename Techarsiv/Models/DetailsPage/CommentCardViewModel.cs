namespace GargamelinBurnu.Models;

public class CommentCardViewModel
{
    public string username { get; set; }
    public string UserImage { get; set; }
    public string Url { get; set; }
    public string SubjectTitle { get; set; }
    public string CategoryName { get; set; }
    public string categoryUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}