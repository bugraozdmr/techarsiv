namespace GargamelinBurnu.Areas.Admin.Models.Users;

public class UserRoleUpdateViewModel
{
    public List<string> Roles { get; set; } = new List<string>();
    public List<string> UserRoles { get; set; } = new List<string>();
}