using Entities.Dtos.Article;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repositories.EF;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ArticleController : Controller
{
    private readonly RepositoryContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;

    public ArticleController(RepositoryContext context,
        UserManager<User> userManager,
        IServiceManager manager)
    {
        _context = context;
        _userManager = userManager;
        _manager = manager;
    }

    public IActionResult Index()
    {
        ViewBag.Categories = GetTagsSelectList();
        
        return View();
    }
    
    [HttpPost]
    public IActionResult Index([FromForm] CreateArticleDto dto)
    {
        if (ModelState.IsValid)
        {
            dto.UserId = _userManager
                .Users
                .Where(s => s.UserName.Equals(User.Identity.Name))
                .Select(s => s.Id)
                .FirstOrDefault();

            _manager.ArticleService.CreateArticle(dto);
        }
        
        ViewBag.Categories = GetTagsSelectList();
        
        return View(dto);
    }

    
    private SelectList GetTagsSelectList()
    {
        return new SelectList(_context.Tags.AsNoTracking()
            , "TagId", "TagName", "1");
    }
}