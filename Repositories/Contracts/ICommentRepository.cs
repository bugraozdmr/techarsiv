using Entities.Models;

namespace Repositories.Contracts;

public interface ICommentRepository : IRepositoryBase<Comment>
{
    IQueryable<Comment> GetAllComments(bool trackChanges);
    void CreateComment(Comment comment);
}