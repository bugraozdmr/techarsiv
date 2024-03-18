using GargamelinBurnu.Areas.Admin.Models.Article;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Main.Components;

public class SimilarViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public SimilarViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke(int tagId,int articleId)
    {
        var similars = _manager
            .ArticleService
            .GetAllArticles(false)
            .Include(s => s.Tag)
            .Include(s => s.User)
            .Where(s => s.Tag.TagId.Equals(tagId) && s.ArticleId != articleId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(6)
            .Select(s => new ArticleViewModel()
            {
                imageUrl = s.image,
                Url = s.Url,
                Title = s.Title,
                createdAt = s.CreatedAt
            })
            .ToList();
        
        
        return View(similars);
    }
}