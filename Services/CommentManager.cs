using AutoMapper;
using Entities.Dtos.Comment;
using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;
using Services.Contracts;

namespace Services;

public class CommentManager : ICommentService
{
    private readonly IRepositoryManager _manager;
    private readonly IMapper _mapper;
    private readonly RepositoryContext _context;
    
    public CommentManager(IRepositoryManager manager
        , IMapper mapper, RepositoryContext context)
    {
        _manager = manager;
        _mapper = mapper;
        _context = context;
    }

    public IQueryable<Comment> getAllComments(bool trackChanges)
    {
        return _manager.Comments.GetAllComments(trackChanges);
    }

    public async Task<int> CreateComment(CreateCommentDto comment)
    {
        var commentToGo = _mapper.Map<Comment>(comment);
        
        commentToGo.CreatedAt = DateTime.Now;
        
        _manager.Comments.CreateComment(commentToGo);
        
        // hata var ama biz geçiyoruz...
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public async Task<int> UpdateComment(updateCommentDto commentDto)
    {
        var commentToGo = _mapper.Map<Comment>(commentDto);

        var oldComment = _manager
            .Comments
            .GetAllComments(false)
            .Where(c => c.CommentId.Equals(commentToGo.CommentId))
            .FirstOrDefault();

        commentToGo.CreatedAt = oldComment.CreatedAt;
        commentToGo.UserId = oldComment.UserId;
        commentToGo.SubjectId = oldComment.SubjectId;
        
        _manager.Comments.UpdateComment(commentToGo);
        // await _manager.SaveAsync();  -- bunu daha basitlerinde kullanıcam -- ajaxsız
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}