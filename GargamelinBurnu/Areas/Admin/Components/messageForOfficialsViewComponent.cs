using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Areas.Admin.Components;

public class messageForOfficialsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}