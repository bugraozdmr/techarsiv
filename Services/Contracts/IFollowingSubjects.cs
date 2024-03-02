using Entities.Models;

namespace Services.Contracts;

public interface IFollowingSubjects
{
    IQueryable<FollowingSubjects> FSubjects { get; }
    void Follow(FollowingSubjects model);
    void unFollow(string userId,int subjectId);
}