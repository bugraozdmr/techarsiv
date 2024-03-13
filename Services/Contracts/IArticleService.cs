using Entities.Dtos.Article;
using Entities.Models;

namespace Services.Contracts;

public interface IArticleService
{
    IQueryable<Article> GetAllArticles(bool trackChanges);
    Task<Article?> getOneArticle(string url,bool trackChanges);

    Task CreateArticle(CreateArticleDto Article);
    Task UpdateArticle(UpdateArticleDto Article);
    Task DeleteArticle(string url);
}