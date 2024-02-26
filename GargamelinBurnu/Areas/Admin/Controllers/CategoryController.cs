using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Panel");
    }
}