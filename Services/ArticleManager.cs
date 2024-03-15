using AutoMapper;
using Entities.Dtos.Article;
using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;
using Services.Contracts;
using Services.Helpers;

namespace Services;

public class ArticleManager : IArticleService
{
    private readonly IRepositoryManager _manager;
    private readonly IMapper _mapper;
    
    
    public ArticleManager(IRepositoryManager manager, 
        IMapper mapper)
    {
        _manager = manager;
        _mapper = mapper;
    }

    public IQueryable<Article> GetAllArticles(bool trackChanges)
    {
        var articles = _manager.Article.GetAllArticles(trackChanges);
        return articles;
    }

    public async Task<Article?> getOneArticle(string url, bool trackChanges)
    {
        var article = await _manager.Article.GetOneArticle(url, trackChanges);

        if (article is null)
        {
            return null;
        }

        return article;
    }

    public async Task CreateArticle(CreateArticleDto Article)
    {
        var articlee = _mapper.Map<Article>(Article);
        
        articlee.CreatedAt = DateTime.Now;
        var url = $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(articlee.Title.Replace(' ', '-').ToLower()))}";

        var check = _manager
            .Article
            .GetAllArticles(false)
            .Where(s => s.Url.Equals(url))
            .FirstOrDefault();

        
        if (url[url.Length - 1] == '-')
        {
            url = url.Substring(0, url.Length - 1);
        }
        
        if (check is not null)
        {
            url = url + SlugModifier.GenerateUniqueHash();
        }

        articlee.Url = url;
        
        _manager.Article.CreateArticle(articlee);
        await _manager.SaveAsync();
    }

    public async Task UpdateArticle(UpdateArticleDto Article)
    {
        var entity1 = _mapper.Map<Article>(Article);

        var entity = _manager
            .Article
            .GetAllArticles(false)
            .Where(s => s.ArticleId.Equals(entity1.ArticleId))
            .FirstOrDefault();

        // hata durumu
        if (entity is null)
        {
            return;   
        }

        if (entity1.Title != entity.Title)
        {
            var url = $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(entity1.Title.Replace(' ', '-').ToLower()))}";

            var check = _manager
                .Article
                .GetAllArticles(false)
                .Where(s => s.Url.Equals(url))
                .FirstOrDefault();

        
            if (url[url.Length - 1] == '-')
            {
                url = url.Substring(0, url.Length - 1);
            }
        
            if (check is not null)
            {
                url = url + SlugModifier.GenerateUniqueHash();
            }

            entity.Url = url;
        }
        
        entity.Title = entity1.Title;
        entity.SubTitle = entity1.SubTitle;
        entity.Content = entity1.Content;
        entity.image = entity1.image ?? entity.image;
        entity.TagId = entity1.TagId;
        
        _manager.Article.UpdateArticle(entity);
        await _manager.SaveAsync();
    }

    public async Task DeleteArticle(string url)
    {
        var article = await getOneArticle(url, false);

        if (article is not null)
        {
            _manager.Article.deleteArticle(article);
            await _manager.SaveAsync();
        }
    }
}