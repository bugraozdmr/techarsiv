using System.ComponentModel.DataAnnotations.Schema;
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
    public DateTime CreatedAt { get; set; }
    
    public List<Subject> Subjects { get; set; } = new List<Subject>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
    

    public List<SubjectLike> LikedSubjects { get; set; } = new List<SubjectLike>();

}