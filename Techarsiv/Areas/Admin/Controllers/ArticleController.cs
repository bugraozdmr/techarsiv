using Entities.Dtos.Article;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repositories.EF;
using Services.Contracts;
using Services.Helpers;

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
    public async Task<IActionResult> Index(CreateArticleDto dto,IFormFile file)
    {
        if (ModelState.IsValid)
        {
            if (file is null)
            {
                ModelState.AddModelError("","file boş olamaz");
                return View(dto);
            }
            
            var uzanti = Path.GetExtension(file.FileName);
            
            if (uzanti != ".jpg" && uzanti != ".png" && uzanti != ".jpeg")
            {
                ModelState.AddModelError("","uzantı hatalı");
                return View(dto);
            }

            var maxFileLength = 2 * 1024 * 1024;

            if (file.Length > maxFileLength)
            {
                ModelState.AddModelError("","dosya 2 mb'dan büyük olamaz");
                return View(dto);
            }
            
            var name_without_extension = Path.GetFileNameWithoutExtension(file.FileName);

            var name = 
                $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(name_without_extension.Replace(' ', '-').ToLower()))}"+$"-{SlugModifier.GenerateUniqueHash()}";;
             
            var filename = name + uzanti;
            
            // file operation
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/articles/coverImages", filename);

            // garbage collector hemen çalışsın bitsin
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            dto.image = String.Concat("/images/articles/coverImages/", filename);
            
            
            dto.UserId = _userManager
                .Users
                .Where(s => s.UserName.Equals(User.Identity.Name))
                .Select(s => s.Id)
                .FirstOrDefault();

            _manager.ArticleService.CreateArticle(dto);


            return RedirectToAction("Index","Home");
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