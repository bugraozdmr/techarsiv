using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.SubjectDtos;

public record CreateSubjectDto
{
    // Tğm özellikleri almak gerekmiyor
    
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [DataType(DataType.Text)]
    [MaxLength(140,ErrorMessage = "Başlık 140 karakterden fazla olamaz")]
    [MinLength(10,ErrorMessage = "Başlık 10 karakterden uzun olmalı")]
    public string Title { get; init; }
    
    
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [MinLength(30,ErrorMessage = "Konu en az 30 karakter olmalıdır")]
    [DataType(DataType.Text)]
    public string Content { get; set; }

    public string? UserId { get; set; }

    public int categoryId { get; init; }
}