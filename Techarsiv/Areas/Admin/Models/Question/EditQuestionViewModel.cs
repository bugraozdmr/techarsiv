namespace GargamelinBurnu.Areas.Admin.Models.Question;

public class EditQuestionViewModel
{
    public int QuestionId { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public string questionDesc { get; set; }
    public int RightAnswer { get; set; }
    public string ChoiceA { get; set; }
    public string ChoiceB { get; set; }
    public string ChoiceC { get; set; }
    public string ChoiceD { get; set; }
}