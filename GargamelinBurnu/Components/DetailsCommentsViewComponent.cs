using Entities.Models;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class DetailsCommentsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public DetailsCommentsViewComponent(IServiceManager manager,
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    public IViewComponentResult Invoke(int subjectId)
    {
        // Take'in yeri sonlarda olmalı yoksa önce alır sonra sorgular sorun çıkar
        List<CommentViewModel> model = _manager
            .CommentService
            .getAllComments(false)
            .OrderBy(c => c.CreatedAt)
            .Where(c => c.SubjectId.Equals(subjectId))
            .Take(10)
            .Select(c => new CommentViewModel()
            {
                CommentUserName = c.User.UserName,
                CommentUserImage = c.User.Image,
                CommentUserSignature = c.User.signature,
                CommentId = c.CommentId,
                UserCommentCount = c.User.Comments.Count,
                CreatedAt = c.User.CreatedAt,
                CommentDate = c.CreatedAt,
                Content = c.Text
            }).ToList();
        
        // userid
        string userid = _userManager
            .Users
            .Where(u => u.UserName.Equals(User.Identity.Name))
            .Select(u => u.Id)
            .FirstOrDefault();

        // comment extras start
        if (model is not null && userid is not null)
        {
            foreach (var comment in model)
            {
                comment.likeCount = _manager
                    .CommentLikeDService
                    .CLikes
                    .Count(l => l.CommentId.Equals(comment.CommentId));
                comment.isLiked = _manager
                        .CommentLikeDService
                        .CLikes
                        .FirstOrDefault(l => l.CommentId.Equals(comment.CommentId)
                                             && l.UserId.Equals(userid))
                    is not null
                    ? true
                    : false;
            
                comment.dislikeCount = _manager
                    .CommentLikeDService
                    .CDislikes
                    .Count(l => l.CommentId.Equals(comment.CommentId));
                comment.isdisLiked = _manager
                        .CommentLikeDService
                        .CDislikes
                        .FirstOrDefault(l => l.CommentId.Equals(comment.CommentId)
                                             && l.UserId.Equals(userid))
                    is not null
                    ? true
                    : false;
            }
        }
        // comment extras end
        
        return View(model);
    }
}