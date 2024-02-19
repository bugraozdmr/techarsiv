using Entities.Models;

namespace Repositories.Contracts;

public interface ISubjectRepository : IRepositoryBase<Subject>
{
    IQueryable<Subject> GetAllSubjects(bool trackChanges);
    Task<Subject?> GetOneSubject(string url,bool trackChanges);
    
    void CreateSubject(Subject subject);

    void deleteSubject(Subject subject);
    void UpdateSubject(Subject subject);
}