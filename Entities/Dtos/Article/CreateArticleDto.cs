using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Article;

public record CreateArticleDto
{
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [DataType(DataType.Text)]
    [MinLength(10,ErrorMessage = "Başlık 10 karakterden uzun olmalı")]
    public string Title { get; init; }
    
    
    [Required(ErrorMessage = "İçerik alanı doldurulmalı")]
    [MinLength(30,ErrorMessage = "Konu en az 30 karakter olmalıdır")]
    [DataType(DataType.Text)]
    public string Content { get; init; }

    public string? UserId { get; set; }

    
    [Required(ErrorMessage = "Başlık alanı doldurulmalı")]
    [DataType(DataType.Text)]
    [MinLength(10,ErrorMessage = "Başlık 10 karakterden uzun olmalı")]
    public string SubTitle { get; init; }

    public string? image { get; set; }
    
    public int tagId { get; init; }
}