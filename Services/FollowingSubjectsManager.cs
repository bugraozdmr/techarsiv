using Entities.Models;
using Repositories.EF;
using Services.Contracts;

namespace Services;

public class FollowingSubjectsManager : IFollowingSubjects
{
    private readonly RepositoryContext _context;

    public FollowingSubjectsManager(RepositoryContext context)
    {
        _context = context;
    }

    public IQueryable<FollowingSubjects> FSubjects => _context.FollowingSubjects.Select(s => new FollowingSubjects()
    {
        SubjectId = s.SubjectId,
        UserId = s.UserId
    });
    public void Follow(FollowingSubjects model)
    {
        if (_context
                .FollowingSubjects
                .FirstOrDefault(l =>
                    l.SubjectId == model.SubjectId && l.UserId == model.UserId) is not null)
        {
            unFollow(model.UserId,model.SubjectId);
        }
        else
        {
            _context.FollowingSubjects.Add(model);
            
            _context.SaveChanges();
        }
    }

    public void unFollow(string userId,int subjectId)
    {
        var fSubject = _context.
            FollowingSubjects.FirstOrDefault(l => l.SubjectId == subjectId && l.UserId == userId);

        if (fSubject != null)
        {
            _context.FollowingSubjects.Remove(fSubject);
            _context.SaveChanges();
        }
    }
}