using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.SubjectDtos;

public record CreateSubjectDto
{
    // Tğm özellikleri almak gerekmiyor
    
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [DataType(DataType.Text)]
    [MaxLength(60,ErrorMessage = "Başlık 60 karakterden fazla olamaz")]
    [MinLength(10,ErrorMessage = "Başlık 10 karakterden uzun olmalı")]
    public string Title { get; init; }
    public string? prefix { get; set; }
    
    
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [MinLength(30,ErrorMessage = "Konu en az 30 karakter olmalıdır")]
    [DataType(DataType.Text)]
    public string Content { get; set; }

    public int UserId { get; init; }

    public int categoryId { get; init; }
}