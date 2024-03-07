using GargamelinBurnu.Models;

namespace GargamelinBurnu.Areas.Admin.Models.Users;

public class PaginationUserViewModel
{
    public List<UserViewModel> List { get; set; }
    public Pagination Pagination { get; set; }
    public string? Param { get; set; }
    public string? area { get; set; }
}