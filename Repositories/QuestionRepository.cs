using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class QuestionRepository : RepositoryBase<Question> , IQuestionRepository
{
    public QuestionRepository(RepositoryContext context) : base(context)
    {
    }

    public IQueryable<Question> GetAllQuestion(bool trackChanges)
    {
        return FindAll(trackChanges);
    }

    public async Task<Question?> GetOneQuestion(string url, bool trackChanges)=> FindByCondition(
        p => p.Url.Equals(url)
        , trackChanges);
    
    public void CreateQuestion(Question Question) => Create(Question);

    public void deleteQuestion(Question Question) => Remove(Question);

    public void UpdateQuestion(Question Question)
    {
        Update(Question);
        _context.SaveChanges();
    }
}