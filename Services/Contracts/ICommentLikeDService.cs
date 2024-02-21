using Entities.Models;

namespace Services.Contracts;

public interface ICommentLikeDService
{
    IQueryable<Commentlike> CLikes { get; }
    IQueryable<CommentDislike> CDislikes { get; }
    void Like(Commentlike model);
    void LikeRemove(int commentId,string userId);
    void disLike(CommentDislike model);
    void disLikeRemove(int commentId,string userId);
}