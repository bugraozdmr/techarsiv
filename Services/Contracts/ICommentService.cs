using Entities.Dtos.Comment;
using Entities.Models;


namespace Services.Contracts;

public interface ICommentService
{
    IQueryable<Comment> getAllComments(bool trackChanges);
    Task<int> CreateComment(CreateCommentDto comment);
    Task<int> UpdateComment(updateCommentDto commentDto);
}