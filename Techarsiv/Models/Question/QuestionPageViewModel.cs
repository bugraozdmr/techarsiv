namespace GargamelinBurnu.Models.Question;

public class QuestionPageViewModel
{
    public string Image { get; set; }
    public string QuestionDesc { get; set; }
    public string ChoiceA { get; set; }
    public string ChoiceB { get; set; }
    public string ChoiceC { get; set; }
    public string ChoiceD { get; set; }
    public int rightAnswer { get; set; }
}