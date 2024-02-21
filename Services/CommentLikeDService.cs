using Entities.Models;
using Repositories.EF;
using Services.Contracts;

namespace Services;

public class CommentLikeDService : ICommentLikeDService
{
    private readonly RepositoryContext _context;

    public CommentLikeDService(RepositoryContext context)
    {
        _context = context;
    }

    public IQueryable<Commentlike> CLikes => _context.Commentlikes.Select(s => new Commentlike()
    {
        CommentId = s.CommentId,
        UserId = s.UserId
    });
    
    public IQueryable<CommentDislike> CDislikes => _context.CommentDislikes.Select(s => new CommentDislike()
    {
        CommentId = s.CommentId,
        UserId = s.UserId
    });
    
    public void Like(Commentlike model)
    {
        // just in case
        if (_context.Commentlikes.FirstOrDefault(l => l.CommentId == model.CommentId && l.UserId == model.UserId) is not null)
        {
            LikeRemove(model.CommentId,model.UserId);
        }
        else
        {
            _context.Commentlikes.Add(model);
            _context.SaveChanges();
        }
    }

    public void LikeRemove(int commentId, string userId)
    {
        var commentLike = _context.Commentlikes.FirstOrDefault(l => l.CommentId == commentId && l.UserId == userId);

        if (commentLike != null)
        {
            _context.Commentlikes.Remove(commentLike);
            _context.SaveChanges(); // Değişiklikleri kaydet
        }

    }

    public void disLike(CommentDislike model)
    {
        if (_context.SubjectDislikes.FirstOrDefault(l => l.SubjectId == model.CommentId && l.UserId == model.UserId) is not null)
        {
            disLikeRemove(model.CommentId,model.UserId);
        }
        else
        {
            _context.CommentDislikes.Add(model);
            _context.SaveChanges();
        }
    }

    public void disLikeRemove(int commentId, string userId)
    {
        var commentDislike = _context.CommentDislikes.FirstOrDefault(l => l.CommentId == commentId && l.UserId == userId);

        if (commentDislike != null)
        {
            _context.CommentDislikes.Remove(commentDislike);
            _context.SaveChanges();
        }
    }
}