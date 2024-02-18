using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos;

public class RegisterDto
{
    [Display(Name = "Full isim")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Full isim doldurulmalı")]
    public string FullName { get; init; }

    [Display(Name = "Kullanıcı adı")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Kullanıcı adı doldurulmalı")]
    public string Username { get; init; }
    
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email alanı doldurulmalı")]
    public string Email { get; init; }
    
    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Parola alanı doldurulmalı")]
    public string Password { get; init; }
    
    [Display(Name = "Parola Tekrar")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Parola tekrar alanı doldurulmalı")]
    public string ConfirmPassword { get; init; }
}