namespace Entities.Models;

public class Question
{
    public int QuestionId { get; set; }
    public int RightAnswer { get; set; }
    public string ChoiceA { get; set; }
    public string ChoiceB { get; set; }
    public string ChoiceC { get; set; }
    public string ChoiceD { get; set; }
    public string Image { get; set; }
    public string QuestionDesc { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string UserId { get; set; }
    public User User { get; set; }
}