using Entities.Models;
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
            // tek bir context çalışsın 2 kere save gereksiz
            _context.SaveChanges();
            var subject = _context.Subjects
                .Where(s => s.SubjectId == model.SubjectId)
                .FirstOrDefault();

            if (subject != null)
            {
                subject.LikeCount = _context.SubjectLikes
                    .Count(s => s.SubjectId.Equals(subject.SubjectId));
                _context.SaveChanges();
            }
            
        }
    }

    public void LikeRemove(int subjectId,string userId)
    {
        var subjectLike = _context.SubjectLikes.FirstOrDefault(l => l.SubjectId == subjectId && l.UserId == userId);

        if (subjectLike != null)
        {
            _context.SubjectLikes.Remove(subjectLike);
            
            var subject = _context.Subjects
                .Where(s => s.SubjectId == subjectId)
                .FirstOrDefault();
            _context.SaveChanges();
            if (subject != null)
            {
                subject.LikeCount = _context.SubjectLikes
                    .Count(s => s.SubjectId.Equals(subject.SubjectId));
                _context.SaveChanges();
            }
            
            

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
            var subject = _context.Subjects
                .Where(s => s.SubjectId == model.SubjectId)
                .FirstOrDefault();

            if (subject != null)
            {
                subject.DislikeCount = _context.SubjectDislikes
                    .Count(s => s.SubjectId.Equals(subject.SubjectId));
                _context.SaveChanges();
            }
            
            
        }
    }

    public void disLikeRemove(int subjectId,string userId)
    {
        var subjectDisLike = _context.SubjectDislikes.FirstOrDefault(l => l.SubjectId == subjectId && l.UserId == userId);

        if (subjectDisLike != null)
        {
            _context.SubjectDislikes.Remove(subjectDisLike);
            
            var subject = _context.Subjects
                .Where(s => s.SubjectId == subjectId)
                .FirstOrDefault();
            _context.SaveChanges();
            if (subject != null)
            {
                subject.DislikeCount = _context.SubjectDislikes
                    .Count(s => s.SubjectId.Equals(subject.SubjectId));
                _context.SaveChanges();
            }
            
            

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
            
            var subject = _context.Subjects
                .Where(s => s.SubjectId == model.SubjectId)
                .FirstOrDefault();

            _context.SaveChanges();
            if (subject != null)
            {
                subject.HeartCount = _context.SubjectHearts
                    .Count(s => s.SubjectId.Equals(subject.SubjectId));
                _context.SaveChanges();
            }
            
        }
    }

    public void HeartRemove(int subjectId, string userId)
    {
        var subjectHeart = _context.SubjectHearts.FirstOrDefault(l => l.SubjectId == subjectId && l.UserId == userId);

        if (subjectHeart != null)
        {
            _context.SubjectHearts.Remove(subjectHeart);
            
            var subject = _context.Subjects
                .Where(s => s.SubjectId == subjectId)
                .FirstOrDefault();

            _context.SaveChanges();
            if (subject != null)
            {
                subject.HeartCount = _context.SubjectHearts
                    .Count(s => s.SubjectId.Equals(subject.SubjectId));
                _context.SaveChanges();
            }
            
            

        }
    }
}