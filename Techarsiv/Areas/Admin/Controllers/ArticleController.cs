using Entities.Dtos.Article;
using Entities.Models;
using Entities.RequestParameters;
using GargamelinBurnu.Areas.Admin.Models.Article;
using GargamelinBurnu.Models;
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
    [ValidateAntiForgeryToken]
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

            // await !
            await _manager.ArticleService.CreateArticle(dto);


            return RedirectToAction("getAllArticles","Article");
        }
        
        ViewBag.Categories = GetTagsSelectList();
        
        return View(dto);
    }

    public IActionResult getAllArticles(CommonRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;

        List<ArticleViewModel> model = new List<ArticleViewModel>();
        PaginationArticleViewModel RealModel = new PaginationArticleViewModel(); 
        int total_count;

        if (p.SearchTerm == null || p.SearchTerm == "")
        {
            model = _manager
                .ArticleService
                .GetAllArticles(false)
                .Include(s => s.User)
                .Include(s => s.Tag)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new ArticleViewModel()
                {
                    username = s.User.UserName,
                    createdAt = s.CreatedAt,
                    tagName = s.Tag.TagName,
                    Url = s.Url,
                    Title = s.Title
                }).ToList();

            total_count = _manager
                .SubjectService
                .GetAllSubjects(false)
                .Count();
        }
        else
        {
            // hata var gibi
            model = _manager
                .ArticleService
                .GetAllArticles(false)
                .Include(s => s.User)
                .Include(s => s.Tag)
                .Where(s => s.Title.Contains(p.SearchTerm.ToLower()))
                .OrderByDescending(s => s.CreatedAt)
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new ArticleViewModel()
                {
                    username = s.User.UserName,
                    createdAt = s.CreatedAt,
                    tagName = s.Tag.TagName,
                    Url = s.Url,
                    Title = s.Title
                }).ToList();

            total_count = _manager
                .SubjectService
                .GetAllSubjects(false)
                .Count(s => s.Title.Contains(p.SearchTerm.ToLower()));
            
            RealModel.Param = $"SearchTerm={p.SearchTerm}";
            RealModel.area = $"/Admin/Article";
        }
        
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        RealModel.List = model;
        RealModel.Pagination = pagination;
        
        
        return View(RealModel);
    }
    
    [HttpGet("admin/article/edit/{url}")]
    public IActionResult Edit(string url)
    {
        var subject = _manager
            .ArticleService
            .GetAllArticles(false)
            .Include(s => s.Tag)
            .Include(s => s.User)
            .Where(s => s.Url.Equals(url))
            .Select(s => new EditArticleViewModel()
            {
                Content = s.Content,
                TagId = s.TagId,
                Title = s.Title,
                CreatedAt = s.CreatedAt,
                SubTitle = s.SubTitle,
                ArticleId = s.ArticleId,
                Url = s.Url
            })
            .FirstOrDefault();

        ViewBag.Categories = GetTagsSelectList();
        
        return View(subject);
    }
    
    [HttpPost("admin/article/edit/{url}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditArticleViewModel model,IFormFile? file,string url)
    {
        // çok salak saçma bir işlem -- modelden alıp dto doldurmak ...
        if (ModelState.IsValid)
        {
            var dto = new UpdateArticleDto();

            if (model.ArticleId != null)
            {
                dto.ArticleId = model.ArticleId;
                dto.Title = model.Title;
                dto.SubTitle = model.SubTitle;
                dto.Content = model.Content;
                dto.TagId = model.TagId;

                if (file is not null)
                {
                    var uzanti = Path.GetExtension(file.FileName);
            
                    if (uzanti != ".jpg" && uzanti != ".png" && uzanti != ".jpeg")
                    {
                        ModelState.AddModelError("","uzantı hatalı");
                        ViewBag.Categories = GetTagsSelectList();
                        return View(model);
                    }

                    var maxFileLength = 2 * 1024 * 1024;

                    if (file.Length > maxFileLength)
                    {
                        ModelState.AddModelError("","dosya 2 mb'dan büyük olamaz");
                        ViewBag.Categories = GetTagsSelectList();
                        return View(model);
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
                }
                
                
                await _manager.ArticleService.UpdateArticle(dto);

                return RedirectToAction("getAllArticles");
            }
            else
            {
                ModelState.AddModelError("","bir şeyler ters gitti");
            }
        }

        ViewBag.Categories = GetTagsSelectList();
        
        return View(model);
    }
    
    
    private SelectList GetTagsSelectList()
    {
        return new SelectList(_context.Tags.AsNoTracking()
            , "TagId", "TagName", "1");
    }
}