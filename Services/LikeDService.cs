using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.EF;
using Services.Contracts;

namespace Services;

public class LikeDService : ILikeDService
{
    private readonly RepositoryContext _context;

    public LikeDService(RepositoryContext context)
    {
        _context = context;
    }


    public IQueryable<SubjectHeart> Hearts => _context.SubjectHearts.Select(s => new SubjectHeart()
    {
        SubjectId = s.SubjectId,
        UserId = s.UserId
    });

    public IQueryable<SubjectLike> Likes => _context.SubjectLikes.Select(s => new SubjectLike()
    {
        SubjectId = s.SubjectId,
        UserId = s.UserId
    });

    public IQueryable<SubjectDislike> Dislikes => _context.SubjectDislikes.Select(s => new SubjectDislike()
    {
        SubjectId = s.SubjectId,
        UserId = s.UserId
    });

    public void Like(SubjectLike model)
    {
        // just in case
        if (_context.SubjectLikes.FirstOrDefault(l => l.SubjectId == model.SubjectId && l.UserId == model.UserId) is not null)
        {
            LikeRemove(model.SubjectId,model.UserId);
        }
        else
        {
            _context.SubjectLikes.Add(model);
            _context.SaveChanges();
        }
    }

    public void LikeRemove(int subjectId,string userId)
    {
        var subjectLike = _context.SubjectLikes.FirstOrDefault(l => l.SubjectId == subjectId && l.UserId == userId);

        if (subjectLike != null)
        {
            _context.SubjectLikes.Remove(subjectLike);
            _context.SaveChanges(); // Değişiklikleri kaydet
        }

    }
    


    public void disLike(SubjectDislike model)
    {
        if (_context.SubjectDislikes.FirstOrDefault(l => l.SubjectId == model.SubjectId && l.UserId == model.UserId) is not null)
        {
            disLikeRemove(model.SubjectId,model.UserId);
        }
        else
        {
            _context.SubjectDislikes.Add(model);
            _context.SaveChanges();
        }
    }

    public void disLikeRemove(int subjectId,string userId)
    {
        var subjectDisLike = _context.SubjectDislikes.FirstOrDefault(l => l.SubjectId == subjectId && l.UserId == userId);

        if (subjectDisLike != null)
        {
            _context.SubjectDislikes.Remove(subjectDisLike);
            _context.SaveChanges();
        }
    }

    public void Heart(SubjectHeart model)
    {
        if (_context.SubjectHearts.FirstOrDefault(l => l.SubjectId == model.SubjectId && l.UserId == model.UserId) is not null)
        {
            HeartRemove(model.SubjectId,model.UserId);
        }
        else
        {
            _context.SubjectHearts.Add(model);
            _context.SaveChanges();
        }
    }

    public void HeartRemove(int subjectId, string userId)
    {
        var subjectHeart = _context.SubjectHearts.FirstOrDefault(l => l.SubjectId == subjectId && l.UserId == userId);

        if (subjectHeart != null)
        {
            _context.SubjectHearts.Remove(subjectHeart);
            _context.SaveChanges();
        }
    }
}