namespace Entities.Models;

public class Awards
{
    public int AwardsId { get; set; }
    public string Title { get; set; }

    public int point { get; set; }

    public List<User> Users { get; set; }
}