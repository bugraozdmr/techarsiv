namespace Entities.Models;

public class AwardUser
{
    public int AwardsId { get; set; }
    public Awards Awards { get; set; }
    
    public string UserId { get; set; }
    public User User { get; set; }
}