using Entities.Models;

namespace GargamelinBurnu.Models;

public class UserSubjectLdhViewModel
{
    public List<string> LikedUsers { get; set; }
    public List<string> disLikedUsers { get; set; }
    public List<string> HeartUsers { get; set; }
}