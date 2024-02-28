using AutoMapper;
using Entities.Dtos.SubjectDtos;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Repositories.Contracts;
using Services.Contracts;
using Services.Helpers;

namespace Services;

public class SubjectManager : ISubjectService
{
    private readonly IRepositoryManager _manager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public SubjectManager(IMapper mapper
        , IRepositoryManager manager,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager)
    {
        _mapper = mapper;
        _manager = manager;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public IQueryable<Subject> GetAllSubjects(bool trackChanges)
    {
        var subjects = _manager.Subject.GetAllSubjects(trackChanges);
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
        productToGo.Prefix = productToGo.Prefix is not null ? productToGo.Prefix.Trim() : productToGo.Prefix;
        
        var url =
            $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(productToGo.Title.Replace(' ', '-').ToLower()))}";

        if ((await getOneSubject(url, false)) is not null)
        {
            url = url+$".{SlugModifier.GenerateUniqueHash()}";
        }
        
        productToGo.CreatedAt = DateTime.Now;

        // role check
        var roles = await GetRolesForUserAsync(subject.UserId);
        if (roles.Contains("Admin"))
        {
            productToGo.IsActive = true;
        }
        else
        {
            productToGo.IsActive = false;
        }
        productToGo.Url = url;

        
        
        _manager.Subject.CreateSubject(productToGo);
        await _manager.SaveAsync();
    }

    // dto olduğuna bakma la -- record değil
    public async Task UpdateSubject(UpdateSubjectDto subject)
    {
        var entity = _mapper.Map<Subject>(subject);
        
        // boş gidemeyecek değerler var
        entity.Url = _manager
            .Subject
            .GetAllSubjects(false)
            .Where(s => s.SubjectId.Equals(subject.SubjectId))
            .Select(s => s.Url)
            .FirstOrDefault();
        
        entity.UserId = _manager
            .Subject
            .GetAllSubjects(false)
            .Where(s => s.SubjectId.Equals(subject.SubjectId))
            .Select(s => s.UserId)
            .FirstOrDefault();
        
        entity.categoryId = _manager
            .Subject
            .GetAllSubjects(false)
            .Where(s => s.SubjectId.Equals(subject.SubjectId))
            .Select(s => s.categoryId)
            .FirstOrDefault();
        
        entity.CreatedAt = _manager
            .Subject
            .GetAllSubjects(false)
            .Where(s => s.SubjectId.Equals(subject.SubjectId))
            .Select(s => s.CreatedAt)
            .FirstOrDefault();
        
        _manager.Subject.UpdateSubject(entity);
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
    
    
    protected async Task<IEnumerable<string>> GetRolesForUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Enumerable.Empty<string>();

        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

}