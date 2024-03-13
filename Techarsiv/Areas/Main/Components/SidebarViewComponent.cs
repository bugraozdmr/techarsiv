using GargamelinBurnu.Areas.Main.Models.MainPage;
using Microsoft.AspNetCore.Mvc;
using Repositories.EF;

namespace GargamelinBurnu.Areas.Main.Components;

public class SidebarViewComponent : ViewComponent
{
    private readonly RepositoryContext _context;

    public SidebarViewComponent(RepositoryContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var tags = _context
            .Tags.Select(s => new SideBarViewModel()
            {
                Tagurl = s.Url,
                Tagname = s.TagName
            }).ToList();
        
        return View(tags);
    }
}