using AutoMapper;
using Entities.Dtos.Question;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using Services.Helpers;

namespace Services;

public class QuestionManager : IQuestionService
{
    private readonly IRepositoryManager _manager;
    private readonly IMapper _mapper;

    public QuestionManager(IRepositoryManager manager,
        IMapper mapper)
    {
        _manager = manager;
        _mapper = mapper;
    }

    public IQueryable<Question> GetAllQuestion(bool trackChanges)
    {
        var questions = _manager.Question.GetAllQuestion(trackChanges);
        return questions;
    }

    public async Task<Question?> getOneQuestion(string url, bool trackChanges)
    {
        var question = await _manager.Question.GetOneQuestion(url, trackChanges);

        if (question is null)
        {
            return null;
        }

        return question;
    }

    public async Task CreateQuestion(CreateQuestionDto question)
    {
        var questionc = _mapper.Map<Question>(question);
        
        questionc.CreatedAt = DateTime.Now;

        var urlName = questionc.QuestionDesc.Length > 40
            ? question.QuestionDesc.Substring(0, 39)
            : question.QuestionDesc;
        
        var url = $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(urlName.Replace(' ', '-').ToLower()))}";

        var check = _manager
            .Question
            .GetAllQuestion(false)
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

        questionc.Url = url;
        
        _manager.Question.CreateQuestion(questionc);
        await _manager.SaveAsync();
    }

    public async Task UpdateQuestion(EditQuestionDto question)
    {
        var entity1 = _mapper.Map<Question>(question);

        var entity = _manager
            .Question
            .GetAllQuestion(false)
            .Where(s => s.QuestionId.Equals(entity1.QuestionId))
            .FirstOrDefault();

        // hata durumu
        if (entity is null)
        {
            return;   
        }

        if (entity1.QuestionDesc != entity.QuestionDesc)
        {
            var urlName = entity1.QuestionDesc.Length > 40
                ? entity1.QuestionDesc.Substring(0, 39)
                : entity1.QuestionDesc;
            
            var url = $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(urlName.Replace(' ', '-').ToLower()))}";

            var check = _manager
                .Question
                .GetAllQuestion(false)
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
        
        entity.QuestionDesc = entity1.QuestionDesc;
        entity.ChoiceA = entity1.ChoiceA;
        entity.ChoiceB = entity1.ChoiceB;
        entity.ChoiceC = entity1.ChoiceC;
        entity.ChoiceD = entity1.ChoiceD;
        entity.RightAnswer = entity1.RightAnswer;
        entity.Image = entity1.Image ?? entity.Image;
        
        _manager.Question.UpdateQuestion(entity);
        await _manager.SaveAsync();
    }

    public async Task DeleteQuestion(string url)
    {
        var question = await getOneQuestion(url, false);

        if (question is not null)
        {
            _manager.Question.deleteQuestion(question);
            await _manager.SaveAsync();
        }
    }
}