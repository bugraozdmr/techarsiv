using AutoMapper;
using Entities.Dtos.Article;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

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
        
        _manager.Article.CreateArticle(articlee);
        await _manager.SaveAsync();
    }

    public async Task UpdateArticle(UpdateArticleDto Article)
    {
        var entity = _mapper.Map<Article>(Article);
        
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