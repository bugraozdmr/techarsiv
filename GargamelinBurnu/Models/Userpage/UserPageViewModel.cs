using Entities.Models;

namespace GargamelinBurnu.Models.Userpage;

public class UserPageViewModel
{
    public int CommentCount { get; set; }
    public int SubjectCount { get; set; }
    public int LikeCount { get; set; }
    public User User { get; set; }
    public List<TitleViewModel> Comments { get; set; }
    public List<TitleViewModel> Subjects { get; set; }
}