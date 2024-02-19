using System.ComponentModel.DataAnnotations;

namespace Entities.Models;

public class Subject
{
    public int SubjectId { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public string? prefix { get; set; }
    
    public string? Title { get; set; }
    
    public string? Content { get; set; }


    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int? categoryId { get; set; }
    public Category? Category { get; set; }

    // comments ve tags eklenecek
    //ön ek eklenecek -- ama oraya options menu lazım
}