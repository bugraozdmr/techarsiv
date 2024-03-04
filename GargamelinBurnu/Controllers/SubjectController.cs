using System.Text.RegularExpressions;
using Entities.Dtos.Comment;
using Entities.Dtos.SubjectDtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;
using Repositories.EF;
using Services.Helpers;


namespace GargamelinBurnu.Controllers;

public class SubjectController : Controller
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;
    private readonly RepositoryContext _context;

    public SubjectController(IServiceManager manager
        , UserManager<User> userManager, 
        IWebHostEnvironment env,
        RepositoryContext context)
    {
        _manager = manager;
        _userManager = userManager;
        _env = env;
        _context = context;
    }

    [Authorize]
    public IActionResult Create()
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
        
        ViewBag.Categories = GetCategoriesSelectList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] CreateSubjectDto model,string Name)
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
        
        if (ModelState.IsValid)
        {
            if (model.prefix != null)
            {
                if (model.prefix.Length > 20)
                {
                    ModelState.AddModelError("","Ön ek 20 karakterden uzun olamaz");
                    return View(model);
                }
            }
            
            var user = await _userManager.FindByNameAsync(Name);

            if (user is not null)
            {
                model.UserId = user.Id;
                
                await _manager.SubjectService.CreateSubject(model);


                // en son uretilen Id'yi al
                int subjectId = _manager
                    .SubjectService
                    .GetAllSubjects(false)
                    .Include(s => s.User)
                    .Where(s => s.User.Id.Equals(user.Id))
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => s.SubjectId)
                    .FirstOrDefault();
                
                // follow created
                _manager.FollowingSubjects.Follow(new FollowingSubjects()
                {
                    SubjectId = subjectId,
                    UserId = model.UserId
                });
                

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


    [HttpPost]
    public async Task<IActionResult> UploadImage(List<IFormFile> files)
    {
        var filepath = "";
        foreach (IFormFile photo in Request.Form.Files)
        {
            var uzanti = Path.GetExtension(photo.FileName);
            if (uzanti != ".jpg" && uzanti != ".png" && uzanti != ".jpeg")
            {
                TempData["create_message"] = "sadece .jpg ve .png uzantılı dosyalar yüklenebilir";
                return Json(new { Url = "fail" });
            }
            
            var maxFileLength = 1 * 1024 * 1024;

            if (photo.Length > maxFileLength)
            {
                TempData["create_message"] = "Dosya 1 MB'dan büyük olamaz";
                return Json(new { Url = "fail" });
            }
            
            var name_without_extension = Path.GetFileNameWithoutExtension(photo.FileName);
            
            var name = 
                $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(name_without_extension.Replace(' ', '-').ToLower()))}"+$"-{SlugModifier.GenerateUniqueHash()}";;
             
            var filename = name + uzanti;
            
            
            string serverMapPath = Path.Combine(_env.WebRootPath, "images/subjects", filename);
            using (var stream = new FileStream(serverMapPath,FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            filepath = "https://localhost:7056/" + "images/subjects/" + filename;
        }

        return Json(new { Url = filepath });
    }
    

    private SelectList GetCategoriesSelectList()
    {
        return new SelectList(_manager.CategoryService.GetAllCategories(false)
            , "CategoryId", "CategoryName", "1");
    }

    // Preview -- dummy not usable
    [HttpPost]
    [Authorize]
    public IActionResult Preview(string? Title,string? Content)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return NotFound();
        }

        var model = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new SubjectViewModel()
            {
                UserName = s.UserName,
                UserImage = s.Image,
                CreatedAt = s.CreatedAt,
                UserSignature = s.signature
            }).FirstOrDefault();

        var userId = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => s.Id)
            .FirstOrDefault();

        model.UserCommentCount = _manager
            .CommentService
            .getAllComments(false)
            .Count(s => s.UserId.Equals(userId));
        
        model.Title = Title;
        model.Content = Content;
        
        
        return View(model);
    }

    [HttpGet("/{url}")]
    public async Task<IActionResult> Details([FromRoute] string url,CommonRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 10 ? 10 : p.Pagesize;

        
        if (url.ToLower().Equals("index"))
        {
            return RedirectToAction("Index", "Home");
        }

        if (User.Identity.IsAuthenticated)
        {
            await _manager.BanService.checkBan(User.Identity.Name);

            var userFF = await _userManager.FindByNameAsync(User.Identity.Name);
            userFF.canTakeEmail = true;
            // true islemi -- mail alamaz tekrar konu okuyana kadar
            await _context.SaveChangesAsync();
        }
        
        // model started
        var model = new SubjectViewModel();
        
        // global user
        // taking user
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserName = s.UserName, UserId = s.Id ,banuntil=s.BanUntill})
            .FirstOrDefault();
        
        
        if (url == "son_mesajlar")
        {
            return RedirectToAction("LastComments", "Home");
        }
        else if (url == "takip")
        {
            return RedirectToAction("followedSubjects", "Home");
        }
        else
        {
            var topic = await _manager
                .SubjectService
                .getOneSubject(url, false);
        
            if (topic is null)
            {
                return NotFound();
            }

            if (!topic.IsActive)
            {
                return NotFound();
            }
            
            // burası login olsanda olmasanda gelecek bilgiler
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
                    UserImage = s.User.Image,
                    CreatedAt = s.User.CreatedAt,
                    UserCommentCount = s.User.Comments.Count,
                    UserSignature = s.User.signature
                }).AsEnumerable()
                .FirstOrDefault(s => s.Subject.Url.Equals(topic.Url));
            
            
            // user yukarı taşındı
            
            
            if (user is not null)
            {
                // Banuntill atama yapildi
                model.BanUntill = user.banuntil ?? DateTime.MinValue;

                
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
        }

        model.p = p;
        model.isMain = p.PageNumber == 1 || p.PageNumber == null ? true : false;

        if (user is not null)
        {
            // isFollowing
            var checking = _manager
                .FollowingSubjects
                .FSubjects
                .Where(s => s.UserId.Equals(user.UserId) &&
                            s.SubjectId.Equals(model.Subject.SubjectId))
                .FirstOrDefault();
            
            model.IsFollowing = checking is null ? false : true;
        }
        
        
        return View(model);
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
    public async Task<IActionResult> EditComment(string Text,int commentId)
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
        
        // çok kod kirliliği var
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
                Count = u.Comments.Count,
                userSignature = u.signature,
                userImage = u.Image
            })
            .FirstOrDefaultAsync();
        
        var result = await _manager.CommentService.UpdateComment(new updateCommentDto()
        {
            CommentId = commentId,
            Text = Text
        });

        if (result == -1)
        {
            return Json(new
            {
                success = -1
            });
        }
        else
        {
            return Json(new
            {
                success = 1,
                text = Text,
                username = model.Username,
                messageCount = model.Count,
                userSignature = model.userSignature,
                userImage = model.userImage
            });
        }
        
    }

    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> deleteComment(int commentId)
    {
        // admin olup olmadığını kontrole gerek yok çünkü authorized yapar

        var result = await _manager.CommentService.DeleteComment(commentId);

        if (result == -1)
        {
            return Json(new
            {
                success = -1
            });
        }
        else
        {
            return Json(new
            {
                success = 1
            });
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


    [Authorize]
    [HttpPost]
    public IActionResult fallowSubject(string username,int subjectId,string url)
    {
        var userId = _userManager
            .Users
            .Where(s => s.UserName.Equals(username))
            .Select(s => s.Id)
            .FirstOrDefault();
        
        
        var check = _manager
            .FollowingSubjects
            .FSubjects
            .Where(s => s.UserId.Equals(userId) && s.SubjectId.Equals(subjectId))
            .FirstOrDefault();

        if (check == null)
        {
            _manager.FollowingSubjects.Follow(new FollowingSubjects()
            {
                SubjectId = subjectId,
                UserId = userId
            });    
        }
        else
        {
            unfallowSubject(userId, subjectId);
        }
        

        return Redirect($"/{url}");
    }
    
    [Authorize]
    [HttpPost]
    public void unfallowSubject(string userId,int subjectId)
    {
        _manager.FollowingSubjects.unFollow(userId, subjectId);
    }
}