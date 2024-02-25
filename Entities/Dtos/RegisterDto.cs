using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos;

public record RegisterDto
{
    [Display(Name = "Full isim")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Kayıt full isim doldurulmalı")]
    public string FullName { get; init; }

    [Display(Name = "Kullanıcı adı")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Kayıt kullanıcı adı doldurulmalı")]
    public string Username { get; init; }
    
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Kayıt email alanı doldurulmalı")]
    public string Email { get; init; }
    
    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Kayıt parola alanı doldurulmalı")]
    public string Password { get; init; }
    
    [Display(Name = "Parola Tekrar")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Kayıt parola tekrar alanı doldurulmalı")]
    [Compare("Password", ErrorMessage = "Parola ve Parola Tekrar alanları eşleşmiyor.")]
    public string ConfirmPassword { get; init; }
}