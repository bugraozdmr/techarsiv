using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Report;

public record CreateReportDto
{
    // bu ikisi sonra tanımlanır
    public string? UserId { get; set; }
    // raporlayan kisi
    public String? username { get; set; }
    
    
    [Required(ErrorMessage = "Bu alan zorunlu şikayet sebebini seçin")]
    public string Cause { get; init; }
    public DateTime CreatedAt { get; set; }
}