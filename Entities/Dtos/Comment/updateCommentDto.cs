namespace Entities.Dtos.Comment;

public record updateCommentDto
{
    public string? Text { get; init; }
    
    public int? CommentId { get; init; }

}