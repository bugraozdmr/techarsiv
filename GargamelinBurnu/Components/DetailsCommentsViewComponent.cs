using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class DetailsCommentsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public DetailsCommentsViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke()
    {
        List<CommentViewModel> model = _manager
            .CommentService
            .getAllComments(false)
            .Take(10)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentViewModel()
            {
                CommentUserName = c.User.UserName,
                CommentId = c.CommentId,
                UserCommentCount = c.User.Comments.Count,
                CreatedAt = c.User.CreatedAt,
                CommentDate = c.CreatedAt,
                Content = c.Text,
                CommentUserId = c.User.Id
            }).ToList();

        // comment extras start
            
        foreach (var comment in model)
        {
            comment.likeCount = _manager
                .CommentLikeDService
                .CLikes
                .Where(l => l.CommentId.Equals(comment.CommentId)).Count();
            comment.isLiked = _manager
                    .CommentLikeDService
                    .CLikes
                    .FirstOrDefault(l => l.CommentId.Equals(comment.CommentId)
                                         && l.UserId.Equals(comment.CommentUserId))
                is not null
                ? true
                : false;
                
            comment.dislikeCount = _manager
                .CommentLikeDService
                .CDislikes
                .Where(l => l.CommentId.Equals(comment.CommentId)).Count();
            comment.isdisLiked = _manager
                    .CommentLikeDService
                    .CDislikes
                    .FirstOrDefault(l => l.CommentId.Equals(comment.CommentId)
                                         && l.UserId.Equals(comment.CommentUserId))
                is not null
                ? true
                : false;
        }

            
        // comment extras end
        
        return View(model);
    }
}