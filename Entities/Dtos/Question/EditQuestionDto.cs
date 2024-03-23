using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Question;

public record EditQuestionDto
{
    public int QuestionId { get; set; }
    [Required]
    public string ChoiceA { get; set; }
    [Required]
    public string ChoiceB { get; set; }
    [Required]
    public string ChoiceC { get; set; }
    [Required]
    public string ChoiceD { get; set; }
    public string Image { get; set; }
    [Required]
    public string QuestionDesc { get; set; }
    [Required] 
    public int RightAnswer { get; set; }
}