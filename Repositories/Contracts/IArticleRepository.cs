using Entities.Models;

namespace Repositories.Contracts;

public interface IArticleRepository
{
    IQueryable<Article> GetAllArticles(bool trackChanges);
    Task<Article?> GetOneArticle(string url,bool trackChanges);
    void CreateArticle(Article Article);
    void deleteArticle(Article Article);
    void UpdateArticle(Article Article);
}