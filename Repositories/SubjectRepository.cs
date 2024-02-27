using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class SubjectRepository : RepositoryBase<Subject> , ISubjectRepository
{
    public SubjectRepository(RepositoryContext context) : base(context)
    {
    }

    

    public IQueryable<Subject> GetAllSubjects(bool trackChanges)
    {
        return FindAll(trackChanges);
    }

    public async Task<Subject?> GetOneSubject(string url, bool trackChanges) => FindByCondition(
        p => p.Url.Equals(url)
        , trackChanges);

    public void CreateSubject(Subject subject) => Create(subject);

    public void deleteSubject(Subject subject) => Remove(subject);

    public void UpdateSubject(Subject subject)
    {
        Update(subject);
        _context.SaveChanges();
    }
}