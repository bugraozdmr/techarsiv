using Entities.Dtos.Comment;
using Entities.Dtos.SubjectDtos;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System.Linq;
using GargamelinBurnu.Models;


namespace GargamelinBurnu.Controllers;

public class SubjectController : Controller
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public SubjectController(IServiceManager manager
        , UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await GetCategoriesSelectList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] CreateSubjectDto model,string Name)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(Name);

            if (user is not null)
            {
                model.UserId = user.Id;
                
                await _manager.SubjectService.CreateSubject(model);

                return RedirectToAction("Index", "Home");    
            }
            else
            {
                ModelState.AddModelError("","Hata oluştu");
            }
            
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

    [HttpGet("/{url}")]
    public async Task<IActionResult> Details([FromRoute] string url)
    {
        if (url.ToLower().Equals("index") || url.ToLower().Equals("style1.css"))
        {
            return RedirectToAction("Index", "Home");
        }

        var topic = await _manager.SubjectService.getOneSubject(url, false);
        
        if (topic is null)
        {
            return NotFound();
        }

        var model = new SubjectViewModel();
        
        
        model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.Category)
            .Include(s => s.User)
            .Include(s => s.Comments)   // çok yorar sunucuyu
            .ThenInclude(c => c.User)
            .Select(s => new SubjectViewModel()
            {
                Subject = s,
                CommentCount = s.Comments.Count,
                Category = s.Category,
                UserName = s.User.UserName,
                CreatedAt = s.User.CreatedAt,
                UserCommentCount = s.User.Comments.Count,
                Comments = s.Comments.Select(c => new CommentViewModel()
                {
                    CommentUserName = c.User.UserName,
                    UserCommentCount = c.User.Comments.Count,
                    CreatedAt = c.User.CreatedAt,
                    CommentDate = c.CreatedAt,
                    Content = c.Text
                }).OrderBy(c => c.CommentDate).ToList()
            }).AsEnumerable()
            .FirstOrDefault(s => s.Subject.Url.Equals(topic.Url));
            
        // take değeri fazla değer almaya çalışırsa patlar
        
        return View(model);
    }

    public async Task<IActionResult> addComment(int SubjectId,string Text)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var UserId = user.Id;
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }

        CommentCountViewModel model = new CommentCountViewModel();
        
        model = await _userManager.Users
            .Where(u => u.UserName == User.Identity.Name)
            .Include(u => u.Comments)
            .Select(u => new  CommentCountViewModel()
            {
                User = u,
                Count = u.Comments.Count
            })
            .FirstOrDefaultAsync();
        
        _manager.CommentService.CreateComment(new CreateCommentDto()
        {
            SubjectId = SubjectId,
            Text = Text,
            UserId = UserId
        });
        
        return Json(new
        {
            success = 1,
            text = Text,
            createdAt = user.CreatedAt,
            username = user.UserName,
            messageCount = model.Count
        });
    }
}