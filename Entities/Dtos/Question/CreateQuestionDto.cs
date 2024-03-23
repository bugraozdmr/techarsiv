using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Question;

public record CreateQuestionDto
{
    [Required]
    public int RightAnswer { get; init; }
    [Required]
    public string ChoiceA { get; init; }
    [Required]
    public string ChoiceB { get; init; }
    [Required]
    public string ChoiceC { get; init; }
    [Required]
    public string ChoiceD { get; init; }
    public string? Image { get; set; }
    [Required]
    public string QuestionDesc { get; init; }
    public string? UserId { get; set; }
}