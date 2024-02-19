using Entities.Dtos.SubjectDtos;
using Entities.Models;

namespace Services.Contracts;

public interface ISubjectService
{
    Task<IEnumerable<Subject>> GetAllSubjects(bool trackChanges);
    Task<Subject?> getOneSubject(string url,bool trackChanges);

    Task CreateSubject(CreateSubjectDto subject);
    //void UpdateSubject(ProductDtoForUpdate product);
    Task DeleteSubject(string url);
}