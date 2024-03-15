using System.ComponentModel.DataAnnotations;

namespace GargamelinBurnu.Models.Userpage;

public class EditUserViewModel
{
    public string Userid { get; set; }
    [MaxLength(25)]
    public string? Fullname { get; set; }
    [MaxLength(35)]
    public string? Username { get; set; }
    [MaxLength(300)]
    public string? signature { get; set; }
    [MaxLength(100)]
    public string? githubUrl { get; set; }
    [MaxLength(100)]
    public string? instagramUrl { get; set; }
    [MaxLength(100)]
    public string? youtubeUrl { get; set; }
    public string? Gender { get; set; }
    [MaxLength(100)]
    public string? Place { get; set; }
    [MaxLength(40)]
    public string? Job { get; set; }
    [MaxLength(600)]
    public string? About { get; set; }
    //public DateTime? Birthday { get; set; }


    public bool emailActive { get; set; }
}