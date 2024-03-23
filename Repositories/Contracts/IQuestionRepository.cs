using Entities.Models;

namespace Repositories.Contracts;

public interface IQuestionRepository
{
    IQueryable<Question> GetAllQuestion(bool trackChanges);
    Task<Question?> GetOneQuestion(string url,bool trackChanges);

    void CreateQuestion(Question Question);

    void deleteQuestion(Question Question);
    void UpdateQuestion(Question Question);
}