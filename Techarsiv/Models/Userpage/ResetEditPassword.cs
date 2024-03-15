using System.ComponentModel.DataAnnotations;

namespace GargamelinBurnu.Models.Userpage;

public class ResetEditPassword
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),ErrorMessage = "Parolalar eşleşmiyor")]
    public string ConfirmPassword { get; set; } = string.Empty;
}