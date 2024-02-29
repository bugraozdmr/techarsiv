using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Ban;

public record BanCauseDto
{
    [Required(ErrorMessage = "Sebep verilmeli")]
    public string Cause { get; init; }

    public string? username { get; set; }
}