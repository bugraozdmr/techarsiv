namespace GargamelinBurnu.Models;

public class MostHeartViewModel
{
    public string owner { get; set; }
    public string OwnerImage { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int MessageCount { get; set; }
    public int HeartCount { get; set; }
    public DateTime CreatedAt { get; set; }
}