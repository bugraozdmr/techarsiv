using System.ComponentModel.DataAnnotations;

namespace GargamelinBurnu.Models;

public class LoginModel
{
    [DataType(DataType.Text)]
    [Display(Name = "Kullanıcı Adı")]
    [Required(ErrorMessage = "Kullanıcı adı alanı doldurulmalı")]
    public string? Username { get; set; }
    
    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Parola alanı doldurulmalı")]
    public string? Password { get; set; }
    
    public bool RememberMe { get; set; } = true;
}