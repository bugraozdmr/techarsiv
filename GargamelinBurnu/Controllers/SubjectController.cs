using Entities.Dtos.SubjectDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class SubjectController : Controller
{
    private readonly IServiceManager _manager;

    public SubjectController(IServiceManager manager)
    {
        _manager = manager;
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await GetCategoriesSelectList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] CreateSubjectDto model)
    {
        if (ModelState.IsValid)
        {
            await _manager.SubjectService.CreateSubject(model);

            return RedirectToAction("Index", "Home");
        }
        
        ViewBag.Categories = await GetCategoriesSelectList();
        
        return View(model);
    }

    private async Task<List<SelectListItem>> GetCategoriesSelectList()
    {
        var categories = await _manager.CategoryService.GetAllCategories(false);
        var selectListItems = categories.Select(c => new SelectListItem
        {
            Value = c.CategoryId.ToString(),
            Text = c.CategoryName
        }).ToList();
    
        return selectListItems;
    }
}