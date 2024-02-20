using Entities.Models;

namespace Services.Contracts;

public interface ILikeDService
{
    IQueryable<SubjectLike> Likes { get; }
    IQueryable<SubjectDislike> Dislikes { get; }
    IQueryable<SubjectHeart> Hearts { get; }
    void Like(SubjectLike model);
    void LikeRemove(int subjectId,string userId);
    void disLike(SubjectDislike model);
    void disLikeRemove(int subjectId,string userId);
    void Heart(SubjectHeart model);
    void HeartRemove(int subjectId,string userId);
}