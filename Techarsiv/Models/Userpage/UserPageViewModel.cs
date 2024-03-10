using Entities.Models;

namespace GargamelinBurnu.Models.Userpage;

public class UserPageViewModel
{
    public int CommentCount { get; set; }
    public int SubjectCount { get; set; }
    public int LikeCount { get; set; }
    public int BanCount { get; set; }
    public bool hasBan { get; set; }
    public User User { get; set; }
    public List<TitleViewModel> Comments { get; set; }
    public List<UserAwardsTab> AwardsTab { get; set; }
    public List<TitleViewModel> Subjects { get; set; }
    public List<string> Roles { get; set; }
}