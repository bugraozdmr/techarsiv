namespace Entities.Models;

public class Ban
{
    public int BanId { get; set; }
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Cause { get; set; }
    public DateTime CreatedAt { get; set; }
}