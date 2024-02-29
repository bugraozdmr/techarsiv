using Entities.Models;

namespace Repositories.Contracts;

public interface ICommentRepository : IRepositoryBase<Comment>
{
    IQueryable<Comment> GetAllComments(bool trackChanges);
    Comment? getOneComment(int id, bool trackChanges);
    void CreateComment(Comment comment);
    void UpdateComment(Comment comment);
    void DeleteComment(Comment comment);
}