using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Article;

public record CreateArticleDto
{
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [DataType(DataType.Text)]
    [MaxLength(60,ErrorMessage = "Başlık 60 karakterden fazla olamaz")]
    [MinLength(10,ErrorMessage = "Başlık 10 karakterden uzun olmalı")]
    public string Title { get; init; }
    
    
    [Required(ErrorMessage = "İçerik alanı doldurulmalı")]
    [MinLength(30,ErrorMessage = "Konu en az 30 karakter olmalıdır")]
    [DataType(DataType.Text)]
    public string Content { get; init; }

    public string? UserId { get; set; }

    
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [DataType(DataType.Text)]
    [MaxLength(150,ErrorMessage = "Başlık 60 karakterden fazla olamaz")]
    [MinLength(10,ErrorMessage = "Başlık 10 karakterden uzun olmalı")]
    public string SubTitle { get; init; }
    
    
    public int tagId { get; init; }
}