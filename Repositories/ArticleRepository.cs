using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class ArticleRepository : RepositoryBase<Article> , IArticleRepository
{
    public ArticleRepository(RepositoryContext context) : base(context)
    {
    }

    public IQueryable<Article> GetAllArticles(bool trackChanges)
    {
        return FindAll(trackChanges);
    }

    public async Task<Article?> GetOneArticle(string url, bool trackChanges) => FindByCondition(
        p => p.Url.Equals(url)
        , trackChanges);

    public void CreateArticle(Article Article) => Create(Article); 

    public void deleteArticle(Article Article) => Remove(Article);

    public void UpdateArticle(Article Article)
    {
        Update(Article);
        _context.SaveChanges();
    }
}