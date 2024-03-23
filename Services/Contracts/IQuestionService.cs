using Entities.Dtos.Question;
using Entities.Models;

namespace Services.Contracts;

public interface IQuestionService
{
    IQueryable<Question> GetAllQuestion(bool trackChanges);
    Task<Question?> getOneQuestion(string url,bool trackChanges);

    Task CreateQuestion(CreateQuestionDto question);
    Task UpdateQuestion(EditQuestionDto question);
    Task DeleteQuestion(string url);
}