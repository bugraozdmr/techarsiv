using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class CommentController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;


    public CommentController(UserManager<User> userManager
        , IServiceManager manager)
    {
        _userManager = userManager;
        _manager = manager;
    }

    [Authorize]
    public IActionResult LikeComment(int CommentId)
    {
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserName = s.UserName, UserId = s.Id })
            .FirstOrDefault();

        
        var userId = user.UserId;
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }
        
        var searched = _manager.CommentLikeDService.CLikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
        if (searched is null)
        {
            var search1 = _manager.CommentLikeDService.CDislikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
            if (search1 is not null)
            {
                dislikeCommentRemove(CommentId, userId);
                
            }
            _manager.CommentLikeDService.Like(new Commentlike()
            {
                CommentId = CommentId,
                UserId = userId
            });
            
            return Json(new
            {
                success = 1,
                likecount = _manager
                    .CommentLikeDService
                    .CLikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count(),
                dislikecount = _manager
                    .CommentLikeDService
                    .CDislikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count()
            });
        }
        else
        {
            return likeCommentRemove(CommentId, userId);
        }
    }


    [Authorize]
    public IActionResult likeCommentRemove(int CommentId,string userId)
    {
        _manager.CommentLikeDService.LikeRemove(CommentId, userId);

        return Json(new
        {
            success = 2,
            likecount = _manager
                .CommentLikeDService
                .CLikes
                .Where(l => l.CommentId.Equals(CommentId)).Count(),
            dislikecount = _manager
                .CommentLikeDService
                .CDislikes
                .Where(l => l.CommentId.Equals(CommentId)).Count()
        });
    }

    [Authorize]
    public async Task<IActionResult> dislikeComment(int CommentId)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var userId = user.Id;
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }
        
        var searched = _manager.CommentLikeDService.CDislikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
        if (searched is null)
        {
            var search1 = _manager.CommentLikeDService.CLikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
            if (search1 is not null)
            {
                likeCommentRemove(CommentId, userId);
            }
            _manager.CommentLikeDService.disLike(new CommentDislike()
            {
                CommentId = CommentId,
                UserId = userId
            });
            
            return Json(new
            {
                success = 1,
                likecount = _manager
                    .CommentLikeDService
                    .CLikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count(),
                dislikecount = _manager
                    .CommentLikeDService
                    .CDislikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count()
            });
        }
        else
        {
            return dislikeCommentRemove(CommentId, userId);
        }
    }

    [Authorize]
    public IActionResult dislikeCommentRemove(int CommentId,string userId)
    {
        _manager.CommentLikeDService.disLikeRemove(CommentId, userId);

        return Json(new
        {
            success = 2,
            likecount = _manager
                .CommentLikeDService
                .CLikes
                .Where(l => l.CommentId.Equals(CommentId)).Count(),
            dislikecount = _manager
                .CommentLikeDService
                .CDislikes
                .Where(l => l.CommentId.Equals(CommentId)).Count()
        });
    }
}