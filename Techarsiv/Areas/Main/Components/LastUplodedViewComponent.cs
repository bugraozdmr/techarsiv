using GargamelinBurnu.Areas.Admin.Models.Article;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Main.Components;

public class LastUplodedViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public LastUplodedViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke(int articleId)
    {
        // sıra önemli önce sırala sonra al
        var last = _manager
            .ArticleService
            .GetAllArticles(false)
            .Include(s => s.User)
            .Include(s => s.Tag)
            .Where(s => s.ArticleId != articleId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(6)
            .Select(s => new ArticleViewModel()
            {
                imageUrl = s.image,
                Url = s.Url,
                Title = s.Title,
                createdAt = s.CreatedAt
            }).ToList();

        return View(last);
    }
}