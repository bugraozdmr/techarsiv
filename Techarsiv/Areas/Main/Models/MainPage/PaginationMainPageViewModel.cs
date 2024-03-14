using GargamelinBurnu.Models;

namespace GargamelinBurnu.Areas.Main.Models.MainPage;

public class PaginationMainPageViewModel
{
    public Pagination Pagination { get; set; }
    public List<MainPageViewModel> List { get; set; }
    public string? Param { get; set; }
    public string? area { get; set; }
}