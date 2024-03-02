using System.Text.RegularExpressions;
using Entities.Dtos.Comment;
using Entities.Dtos.Notification;
using Entities.Models;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.EF;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class CommentController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;
    private readonly RepositoryContext _context;    // save icin


    public CommentController(UserManager<User> userManager
        , IServiceManager manager, 
        RepositoryContext context)
    {
        _userManager = userManager;
        _manager = manager;
        _context = context;
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
    
    
    [Authorize]
    public async Task<IActionResult> addComment(int SubjectId,string Text)
    {
        var userBan = _userManager
            .Users
            .Where(u => u.UserName.Equals(User.Identity.Name))
            .Select(u => u.BanUntill)
            .FirstOrDefault();
        
        if (userBan != null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new {  UserId = s.Id })
            .FirstOrDefault();
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }

        string[] yasakliKelimeler = { "amına koyayım", "siktiğim", "sikik","dalyarak","dalyarrak","yarrak","piç","siktirgit","siktir"};
        string pattern = string.Join("|", yasakliKelimeler);

        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        MatchCollection matches = regex.Matches(Text.ToLower());

        if (matches.Count > 0)
        {
            return Json(new
            {
                success = -1
            });
        }
        
        CommentDetailsViewModel model = new CommentDetailsViewModel();
        
        model = await _userManager.Users
            .Where(u => u.UserName == User.Identity.Name)
            .Include(u => u.Comments)
            .Select(u => new  CommentDetailsViewModel()
            {
                Username = u.UserName,
                CreatedAt = u.CreatedAt,
                Count = u.Comments.Count,
                userSignature = u.signature,
                userImage = u.Image
            })
            .FirstOrDefaultAsync();
        
        var result = await _manager.CommentService.CreateComment(new CreateCommentDto()
        {
            SubjectId = SubjectId,
            Text = Text,
            UserId = user.UserId
        });

        if (result == 1)
        {
            // Notification islemleri
            // sadece ilgilenleri alcak onlara göndercek... baya yavaşlatcak
            
            var subjectnot = _manager
                .FollowingSubjects
                .FSubjects
                .Where(s => s.SubjectId.Equals(SubjectId))
                .ToList();

            if (subjectnot != null)
            {
                foreach (var item in subjectnot)
                {
                    if (!(item.UserId.Equals(user.UserId)))
                    {
                        NotificationDto dto = new NotificationDto()
                        {
                            UserId = item.UserId,
                            SubjectId = SubjectId
                        };

                        _manager.NotificationService.CreateNotification(dto);
                    }
                }
                // burda olmazsa tüm hepsi için kayıt almıyor atlıyor /*/*/
                await _context.SaveChangesAsync();
            }
            
            
            
            return Json(new
            {
                success = 1,
                text = Text,
                createdAt = model.CreatedAt,
                username = model.Username,
                messageCount = model.Count,
                userSignature = model.userSignature,
                userImage = model.userImage
            });    
        }
        else
        {
            return Json(new
            {
                success = -1
            });
        }
        
    }
}