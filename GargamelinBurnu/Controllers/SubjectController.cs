using Entities.Dtos.Comment;
using Entities.Dtos.SubjectDtos;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;



namespace GargamelinBurnu.Controllers;

public class SubjectController : Controller
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public SubjectController(IServiceManager manager
        , UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    [Authorize]
    public IActionResult Create()
    {
        ViewBag.Categories = GetCategoriesSelectList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] CreateSubjectDto model,string Name)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(Name);

            if (user is not null)
            {
                model.UserId = user.Id;
                
                await _manager.SubjectService.CreateSubject(model);

                return RedirectToAction("Index", "Home");    
            }
            else
            {
                ModelState.AddModelError("","Hata oluştu");
            }
            
        }
        
        ViewBag.Categories = GetCategoriesSelectList();
        
        return View(model);
    }

   
    

    private SelectList GetCategoriesSelectList()
    {
        return new SelectList(_manager.CategoryService.GetAllCategories(false)
            , "CategoryId", "CategoryName", "1");
    }

    [HttpGet("/{url}")]
    public async Task<IActionResult> Details([FromRoute] string url)
    {
        if (url.ToLower().Equals("index") || url.ToLower().Equals("style1.css"))
        {
            return RedirectToAction("Index", "Home");
        }

        var topic = await _manager.SubjectService.getOneSubject(url, false);
        
        if (topic is null)
        {
            return NotFound();
        }

        var model = new SubjectViewModel();
        
        
        model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.Category)
            .Include(s => s.User)
            .Include(s => s.Comments)   // çok yorar sunucuyu
            .ThenInclude(c => c.User)
            .Select(s => new SubjectViewModel()
            {
                Subject = s,
                CommentCount = s.Comments.Count,
                Category = s.Category,
                UserName = s.User.UserName,
                CreatedAt = s.User.CreatedAt,
                UserCommentCount = s.User.Comments.Count,
            }).AsEnumerable()
            .FirstOrDefault(s => s.Subject.Url.Equals(topic.Url));
        
        
        
        
        
        
        // taking user
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserName = s.UserName, UserId = s.Id })
            .FirstOrDefault();

        if (user is not null)
        {
            var userId = user.UserId;
        
            
            // like count
            int likecount = _manager
                .LikeDService
                .Likes
                .Where(l => l.SubjectId.Equals(topic.SubjectId)).Count();
            // like count
            int dislikecount = _manager
                .LikeDService
                .Dislikes
                .Where(l => l.SubjectId.Equals(topic.SubjectId)).Count();
            // like count
            int heartcount = _manager
                .LikeDService
                .Hearts
                .Where(l => l.SubjectId.Equals(topic.SubjectId)).Count();
        
        
        
            // subject extras start
        
            bool isLiked = _manager
                    .LikeDService
                    .Likes
                    .FirstOrDefault(l => l.SubjectId.Equals(topic.SubjectId)
                                         && l.UserId.Equals(userId))
                is not null
                ? true
                : false;
            
            bool isdisLiked = _manager
                    .LikeDService
                    .Dislikes
                    .FirstOrDefault(l => l.SubjectId.Equals(topic.SubjectId)
                                         && l.UserId.Equals(userId))
                is not null
                ? true
                : false;
            
            bool isheart = _manager
                    .LikeDService
                    .Hearts
                    .FirstOrDefault(l => l.SubjectId.Equals(topic.SubjectId)
                                         && l.UserId.Equals(userId))
                is not null
                ? true
                : false;
        
            model.likeCount = likecount;
            model.dislikeCount = dislikecount;
            model.heartCount = heartcount;
        
            model.isLiked = isLiked;
            model.isdisLiked = isdisLiked;
            model.isheart = isheart;

            // subject extras end
        }
        
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> addComment(int SubjectId,string Text)
    {
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

        CommentCountViewModel model = new CommentCountViewModel();
        
        model = await _userManager.Users
            .Where(u => u.UserName == User.Identity.Name)
            .Include(u => u.Comments)
            .Select(u => new  CommentCountViewModel()
            {
                Username = u.UserName,
                CreatedAt = u.CreatedAt,
                Count = u.Comments.Count
            })
            .FirstOrDefaultAsync();
        
        _manager.CommentService.CreateComment(new CreateCommentDto()
        {
            SubjectId = SubjectId,
            Text = Text,
            UserId = user.UserId
        });
        
        return Json(new
        {
            success = 1,
            text = Text,
            createdAt = model.CreatedAt,
            username = model.Username,
            messageCount = model.Count
        });
    }
    
    [Authorize]
    public async Task<IActionResult> LikeSubject(int SubjectId)
    {
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserId = s.Id })
            .FirstOrDefault();

        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }
        
        
        
        // continue
        var searched = _manager.LikeDService.Likes.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));

        if (searched is null)
        {
            var search1 = _manager.LikeDService.Dislikes.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));
            if (search1 is not null)
            {
                dislikeSubjectRemove(SubjectId, user.UserId);
                _manager.LikeDService.Like(new SubjectLike()
                {
                    SubjectId = SubjectId,
                    UserId = user.UserId
                });
            }
            else
            {
                var search2 = _manager.LikeDService.Hearts.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));

                if (search2 is not null)
                {
                    heartSubjectRemove(SubjectId, user.UserId);
                }
                
                _manager.LikeDService.Like(new SubjectLike()
                {
                    SubjectId = SubjectId,
                    UserId = user.UserId
                });
            }
            
            return Json(new
            {
                success = 1,
                heartcount =  _manager
                    .LikeDService
                    .Hearts
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
                likecount = _manager
                    .LikeDService
                    .Likes
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
                dislikecount = _manager
                    .LikeDService
                    .Dislikes
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count()

            });
        }
        else
        {
            return likeSubjectRemove(SubjectId, user.UserId);
        }
    }

    [Authorize]
    public IActionResult likeSubjectRemove(int SubjectId,string UserId)
    {
        _manager.LikeDService.LikeRemove(SubjectId, UserId);
        
        
        return Json(new
        {
            success = 2,
            heartcount =  _manager
                .LikeDService
                .Hearts
                .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
            likecount = _manager
                .LikeDService
                .Likes
                .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
            dislikecount = _manager
                .LikeDService
                .Dislikes
                .Where(l => l.SubjectId.Equals(SubjectId)).Count()

        });
    }
    
    [Authorize]
    public async Task<IActionResult> dislikeSubject(int SubjectId)
    {
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserId = s.Id })
            .FirstOrDefault();

        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }

        var searched = _manager.LikeDService.Dislikes.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));

        if (searched is null)
        {
            var search1 = _manager.LikeDService.Likes.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));
            if (search1 is not null)
            {
                likeSubjectRemove(SubjectId, user.UserId);
                _manager.LikeDService.disLike(new SubjectDislike()
                {
                    SubjectId = SubjectId,
                    UserId = user.UserId
                });
            }
            else
            {
                var search2 = _manager.LikeDService.Hearts.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));

                if (search2 is not null)
                {
                    heartSubjectRemove(SubjectId, user.UserId);
                }
                
                _manager.LikeDService.disLike(new SubjectDislike()
                {
                    SubjectId = SubjectId,
                    UserId = user.UserId
                });
            }
            
            
            
            
            
            return Json(new
            {
                success = 1,
                heartcount =  _manager
                    .LikeDService
                    .Hearts
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
                likecount = _manager
                    .LikeDService
                    .Likes
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
                dislikecount = _manager
                    .LikeDService
                    .Dislikes
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count()

            });
        }
        else
        {
            return dislikeSubjectRemove(SubjectId, user.UserId);
        }
    }
    
    [Authorize]
    public IActionResult dislikeSubjectRemove(int SubjectId,string UserId)
    {
        _manager.LikeDService.disLikeRemove(SubjectId, UserId);

        
            
        return Json(new
        {
            success = 2,
            heartcount =  _manager
                .LikeDService
                .Hearts
                .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
            likecount = _manager
                .LikeDService
                .Likes
                .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
            dislikecount = _manager
                .LikeDService
                .Dislikes
                .Where(l => l.SubjectId.Equals(SubjectId)).Count()

        });
    }
    
    
    [Authorize]
    public async Task<IActionResult> heartSubject(int SubjectId)
    {
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserId = s.Id })
            .FirstOrDefault();
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }

        var searched = _manager.LikeDService.Hearts.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));

        if (searched is null)
        {
            var search1 = _manager.LikeDService.Likes.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));
            if (search1 is not null)
            {
                likeSubjectRemove(SubjectId, user.UserId);
                _manager.LikeDService.Heart(new SubjectHeart()
                {
                    SubjectId = SubjectId,
                    UserId = user.UserId
                });
            }
            else
            {
                var search2 = _manager.LikeDService.Dislikes.FirstOrDefault(s => s.UserId.Equals(user.UserId) && s.SubjectId.Equals(SubjectId));

                if (search2 is not null)
                {
                    dislikeSubjectRemove(SubjectId, user.UserId);
                }
                
                _manager.LikeDService.Heart(new SubjectHeart()
                {
                    SubjectId = SubjectId,
                    UserId = user.UserId
                });
            }
            
            
            
            return Json(new
            {
                success = 1,
                heartcount =  _manager
                    .LikeDService
                    .Hearts
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
                likecount = _manager
                    .LikeDService
                    .Likes
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
                dislikecount = _manager
                    .LikeDService
                    .Dislikes
                    .Where(l => l.SubjectId.Equals(SubjectId)).Count()

            });
        }
        else
        {
            return heartSubjectRemove(SubjectId, user.UserId);
        }
    }
    
    
    [Authorize]
    public IActionResult heartSubjectRemove(int SubjectId,string UserId)
    {
        _manager.LikeDService.HeartRemove(SubjectId, UserId);

        
        
        return Json(new
        {
            success = 2,
            heartcount =  _manager
                .LikeDService
                .Hearts
                .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
            likecount = _manager
                .LikeDService
                .Likes
                .Where(l => l.SubjectId.Equals(SubjectId)).Count(),
            dislikecount = _manager
                .LikeDService
                .Dislikes
                .Where(l => l.SubjectId.Equals(SubjectId)).Count()

        });
    }
}