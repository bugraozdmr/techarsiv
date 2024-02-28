using Entities.Dtos.Comment;
using Entities.Models;


namespace Services.Contracts;

public interface ICommentService
{
    IQueryable<Comment> getAllComments(bool trackChanges);
    Task CreateComment(CreateCommentDto comment);
    Task UpdateComment(updateCommentDto commentDto);
}