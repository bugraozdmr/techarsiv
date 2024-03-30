using Entities.RequestParameters;
using GargamelinBurnu.Areas.Main.Models.ArticlePage;
using GargamelinBurnu.Areas.Main.Models.MainPage;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Main.Controllers;

[Area("Main")]
public class HomeController : Controller
{
    private readonly IServiceManager _manager;

    public HomeController(IServiceManager manager)
    {
        _manager = manager;
    }

    [HttpGet("/")]
    public IActionResult Home(CommonRequestParameters p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 10 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 10 ? 10 : p.Pagesize;

        PaginationMainPageViewModel realModel = new PaginationMainPageViewModel();
        
        
        List<MainPageViewModel> model = new List<MainPageViewModel>();

        int total_count;

        if (p.SearchTerm == null || p.SearchTerm == "")
        {
            model = _manager
                .ArticleService
                .GetAllArticles(false)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new MainPageViewModel()
                {
                    Image = s.image,
                    Title = s.Title,
                    SubTitle = s.SubTitle,
                    TagName = s.Tag.TagName,
                    TagUrl= s.Tag.Url,
                    CreatedAt = s.CreatedAt,
                    Username = s.User.UserName,
                    url = s.Url
                }).ToList();
            
            
            total_count = _manager
                .ArticleService
                .GetAllArticles(false)
                .Count();
        }
        else
        {
            model = _manager
                .ArticleService
                .GetAllArticles(false)
                .Include(s => s.Tag)
                .Include(s => s.User)
                .Where(s => s.Title.Contains(p.SearchTerm.ToLower()))
                .OrderByDescending(s => s.CreatedAt)
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new MainPageViewModel()
                {
                    Image = s.image,
                    Title = s.Title,
                    SubTitle = s.SubTitle,
                    TagName = s.Tag.TagName,
                    TagUrl= s.Tag.Url,
                    CreatedAt = s.CreatedAt,
                    Username = s.User.UserName,
                    url = s.Url
                }).ToList();
            
            realModel.Param = $"SearchTerm={p.SearchTerm}";
            
            total_count = _manager
                .ArticleService
                .GetAllArticles(false)
                .Count(s => s.Title.Contains(p.SearchTerm.ToLower()));
        }
        
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        realModel.List = model;
        realModel.Pagination = pagination;
        
        // ana sayfa mı
        ViewData["IsHomePage"] = true;

        
        return View(realModel);
    }

    
    [HttpGet("/t/{tagUrl}")]
    public IActionResult tag(CommonRequestParameters p,string tagUrl)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;

        PaginationMainPageViewModel realModel = new PaginationMainPageViewModel();
        
        
        List<MainPageViewModel> model = new List<MainPageViewModel>();

        int total_count;

        model = _manager
            .ArticleService
            .GetAllArticles(false)
            .Include(s => s.Tag)
            .Include(s => s.User)
            .Where(s => s.Tag.Url.Equals(tagUrl))
            .OrderByDescending(s => s.CreatedAt)
            .Skip((p.PageNumber-1)*p.Pagesize)
            .Take(p.Pagesize)
            .Select(s => new MainPageViewModel()
            {
                Image = s.image,
                Title = s.Title,
                SubTitle = s.SubTitle,
                TagName = s.Tag.TagName,
                CreatedAt = s.CreatedAt,
                Username = s.User.UserName,
                url = s.Url
            }).ToList();
            
        total_count = _manager
            .ArticleService
            .GetAllArticles(false)
            .Count(s => s.Tag.Url.Equals(tagUrl));
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        realModel.List = model;
        realModel.Pagination = pagination;
        
        // ana sayfa mı
        ViewData["IsHomePage"] = true;

        
        return View("Home",realModel);
    }


    [HttpGet("/{url}")]
    public IActionResult ArticlePage(string url)
    {
        var article = _manager
            .ArticleService
            .GetAllArticles(false)
            .Include(s => s.User)
            .Include(s => s.Tag)
            .Where(s => s.Url.Equals(url))
            .Select(s => new ArticlePageViewModel()
            {
                Content = s.Content,
                Title = s.Title,
                SubTitle = s.SubTitle,
                CreatedAt = s.CreatedAt,
                Username = s.User.UserName,
                TagName = s.Tag.TagName,
                TagUrl = s.Tag.Url,
                TagId = s.TagId,
                Url = s.Url,
                image = s.image,
                ArticleId = s.ArticleId
            })
            .FirstOrDefault();

        if (article is null)
        {
            return NotFound();
        }
        
        
        return View(article);
    }
    
    
    
    public IActionResult sitemap()
    {
        // Sitemap dosyasının fiziksel yolunu alın
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "sitemap.xml");

        // Sitemap dosyasını döndürün
        return PhysicalFile(filePath, "application/xml");
    }
    
    public IActionResult robots()
    {
        // Sitemap dosyasının fiziksel yolunu alın
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "robots.txt");

        // Sitemap dosyasını döndürün
        return PhysicalFile(filePath, "text/plain");
    }

    [HttpGet("/404")]
    public IActionResult error()
    {
        return View();
    }
}