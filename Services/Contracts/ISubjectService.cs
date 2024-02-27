using Entities.Dtos.SubjectDtos;
using Entities.Models;

namespace Services.Contracts;

public interface ISubjectService
{
    IQueryable<Subject> GetAllSubjects(bool trackChanges);
    Task<Subject?> getOneSubject(string url,bool trackChanges);

    Task CreateSubject(CreateSubjectDto subject);
    Task UpdateSubject(UpdateSubjectDto subject);
    Task DeleteSubject(string url);
}