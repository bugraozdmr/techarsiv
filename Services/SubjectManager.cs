using AutoMapper;
using Entities.Dtos.SubjectDtos;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Services.Contracts;
using Services.Helpers;

namespace Services;

public class SubjectManager : ISubjectService
{
    private readonly IRepositoryManager _manager;
    private readonly IMapper _mapper;

    public SubjectManager(IMapper mapper
        , IRepositoryManager manager)
    {
        _mapper = mapper;
        _manager = manager;
    }

    public async Task<IEnumerable<Subject>> GetAllSubjects(bool trackChanges)
    {
        // buralara filtre atılacak
        var subjects = await _manager.Subject.GetAllSubjects(trackChanges).ToListAsync();
        return subjects;
    }

    public async Task<Subject?> getOneSubject(string url, bool trackChanges)
    {
        var subject = await _manager.Subject.GetOneSubject(url, trackChanges);

        if (subject is null)
        {
            return null;
        }

        return subject;
    }

    public async Task CreateSubject(CreateSubjectDto subject)
    {
        var productToGo = _mapper.Map<Subject>(subject);
        
        productToGo.Title = productToGo.Title is not null ? productToGo.Title.Trim() : productToGo.Title;
        productToGo.Content = productToGo.Content is not null ? productToGo.Content.Trim() : productToGo.Content;
        productToGo.prefix = productToGo.prefix is not null ? productToGo.prefix.Trim() : productToGo.prefix;
        
        var url =
            $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(productToGo.Title.Replace(' ', '-').ToLower()))}";

        if ((await getOneSubject(url, false)) is not null)
        {
            url = url+$".{SlugModifier.GenerateUniqueHash()}";
        }
        
        productToGo.CreatedAt = DateTime.UtcNow;
        productToGo.IsActive = false;
        productToGo.Url = url;
        
        _manager.Subject.CreateSubject(productToGo);
        await _manager.SaveAsync();
    }

    public async Task DeleteSubject(string url)
    {
        var subject = await getOneSubject(url, false);

        if (subject is not null)
        {
            _manager.Subject.deleteSubject(subject);
            await _manager.SaveAsync();
        }
    }
}