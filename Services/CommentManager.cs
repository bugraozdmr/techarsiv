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

    public async Task CreateComment(CreateCommentDto comment)
    {
        var commentToGo = _mapper.Map<Comment>(comment);
        
        commentToGo.CreatedAt = DateTime.Now;
        
        _manager.Comments.CreateComment(commentToGo);
        
        // hata var ama biz geçiyoruz...
        await _manager.SaveAsync();
        //_context.SaveChanges();
    }
}