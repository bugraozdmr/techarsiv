using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area("Admin")]
public class PanelController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}