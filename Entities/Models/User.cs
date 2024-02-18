using Microsoft.AspNetCore.Identity;

namespace Entities.Models;

public class User : IdentityUser
{
    public string? FullName { get; set; }
    public string? signature { get; set; }
    public string? Gender { get; set; }
    public string? Place { get; set; }
    public string? Job { get; set; }
    public DateTime? Birthday { get; set; }
    
}