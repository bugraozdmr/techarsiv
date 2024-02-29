using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class CommentRepository : RepositoryBase<Comment>,ICommentRepository
{
    public CommentRepository(RepositoryContext context) : base(context)
    {
    }

    public IQueryable<Comment> GetAllComments(bool trackChanges)
    {
        return FindAll(trackChanges);
    }

    public Comment? getOneComment(int id, bool trackChanges) => FindByCondition(p => p.CommentId.Equals(id), trackChanges);

    public void CreateComment(Comment comment)
    {
        Create(comment);
    }

    public void UpdateComment(Comment comment)
    {
        Update(comment);
    }

    public void DeleteComment(Comment comment)
    {
        Remove(comment);   
    }
}