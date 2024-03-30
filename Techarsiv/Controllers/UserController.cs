using Entities.Dtos.Ban;
using Entities.Dtos.Report;
using Entities.Models;
using GargamelinBurnu.Models;
using GargamelinBurnu.Models.Userpage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using Services.Helpers;


namespace GargamelinBurnu.Controllers;

public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;
    private readonly SignInManager<User> _signInManager;
    

    public UserController(UserManager<User> userManager,
        IServiceManager manager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _manager = manager;
        _signInManager = signInManager;
    }

    [HttpGet("/forum/biri/{username}")]
    public async Task<IActionResult> Index(string username)
    {
        UserPageViewModel model = new UserPageViewModel();

        // tüm her şey geliyor düzeltilebilir belki
        var user = _userManager
            .Users
            .Where(u => u.UserName.Equals(username))
            .FirstOrDefault();

        if (user is null)
        {
            return NotFound();
        }
        
        model.User = user;

        // likecount
        model.LikeCount = _manager
                  .LikeDService
                  .Likes
                  .Count(l => l.UserId.Equals(user.Id)) +
              _manager
                  .CommentLikeDService
                  .CLikes
                  .Count(l => l.UserId.Equals(user.Id));

        // commentcount
        model.CommentCount = _manager
            .CommentService
            .getAllComments(false)
            .Count(c => c.UserId.Equals(user.Id));
        
        // subjectcount
        model.SubjectCount = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Count(s => s.UserId.Equals(user.Id) && s.IsActive.Equals(true));

        // titleViewModel uyuyor istediğim özelliklere
        model.Comments = _manager
            .CommentService
            .getAllComments(false)
            .Include(c => c.User)
            .Include(c => c.Subject)
            .ThenInclude(s => s.Category)
            .OrderByDescending(c => c.CreatedAt)
            .Where(c => c.UserId.Equals(user.Id))
            .Take(10)
            .Select(c => new TitleViewModel()
            {
                Username = c.User.UserName,
                Url = c.Subject.Url,
                CategoryName = c.Subject.Category.CategoryName,
                createdAt = c.CreatedAt,
                Title = c.Subject.Title,
                Content = c.Text
            }).ToList();

        model.Subjects = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.User)
            .Include(s => s.Comments)
            .Include(s => s.Category)
            .Where(s => s.UserId.Equals(user.Id) && s.IsActive.Equals(true))
            .OrderByDescending(s => s.CreatedAt)
            .Take(10)
            .Select(s => new TitleViewModel()
            {
                Title = s.Title,
                Username = s.User.UserName,
                createdAt = s.CreatedAt,
                CommentCount = s.Comments.Count(),
                CategoryName = s.Category.CategoryName,
                Url = s.Url
            }).ToList();

        // ban count
        model.BanCount = _manager
            .BanService
            .getAllBans(false)
            .Count(s => s.UserId.Equals(model.User.Id));
        
        
        
        // awardUser -- illa kullanıcının kendisi girsin dedim fazla sorgu olmasın diye
        // bool değerler ile kontroller geçilebilir
        if (User.Identity.IsAuthenticated)
        {
            if (User.Identity.Name.Equals(username))
            {
                var commentCount = model.CommentCount;
                var subjectCount = model.SubjectCount;
                var year = model.User.CreatedAt;

                // commentCount award check
                if (commentCount >= 10)
                {
                    awardCheck(model.User.Id, 1);
                }
                
                // subjectcount award check
                if (subjectCount >= 10)
                {
                    awardCheck(model.User.Id, 2);
                }
                
                // commentCount award check
                if (DateTime.Now >= year.AddYears(1))
                {
                    awardCheck(model.User.Id, 3);
                }

                if (DateTime.Now >= year.AddDays(7))
                {
                    awardCheck(model.User.Id, 4);
                }
                
                if (DateTime.Now >= year.AddDays(30))
                {
                    awardCheck(model.User.Id, 5);
                }

                if (commentCount >= 50)
                {
                    awardCheck(model.User.Id, 6);
                }

                if (commentCount >= 100)
                {
                    awardCheck(model.User.Id, 7);
                }
                
                if (commentCount >= 500)
                {
                    awardCheck(model.User.Id, 8);
                }

                if (subjectCount >= 50)
                {
                    awardCheck(model.User.Id, 9);
                }
            }
        }

        
        // awards tab -- include çalışmadı iki işlem yapıldı
        List<UserAwardsTab> model2 = _manager
            .AwardUserService
            .AwardUsers
            .Where(s => s.UserId.Equals(model.User.Id))
            .Select(s => new UserAwardsTab()
            {
                createdAt = s.CreatedAt,
                awardId = s.AwardsId
            }).ToList();

        foreach (var item in model2)
        {
            item.percentage =(int)
                ((double)_manager
                     .AwardUserService
                     .AwardUsers
                     .Count(s => s.AwardsId.Equals(item.awardId)) /
                 (double)_userManager.Users.Count()*100);

            item.awardPoint = _manager
                .AwardService
                .Awards
                .Where(s => s.AwardsId.Equals(item.awardId))
                .Select(s => s.point)
                .FirstOrDefault();
            
            item.awardName = _manager
                .AwardService
                .Awards
                .Where(s => s.AwardsId.Equals(item.awardId))
                .Select(s => s.Title)
                .FirstOrDefault();
        }

        
        // user points -- user kendi sayfasına bakıyor olmalı
        if (User.Identity.IsAuthenticated)
        {
            if (User.Identity.Name.Equals(username))
            {
                int totalPoint = 0;
                var awards = _manager
                    .AwardUserService
                    .AwardUsers
                    .Where(aw => aw.UserId.Equals(model.User.Id))
                    .Select(aw => aw.AwardsId)
                    .ToList(); // * tolist

                foreach (var ids in awards)
                {
                    var point = _manager
                        .AwardService
                        .Awards
                        .Where(s => s.AwardsId.Equals(ids))
                        .Select(s => s.point)
                        .FirstOrDefault();

                    totalPoint += point;
                }
                
                model.User.Points = totalPoint +
                    _manager.CommentService.getAllComments(false)
                        .Count(s => s.UserId.Equals(model.User.Id)) +
                    _manager.SubjectService.GetAllSubjects(false)
                        .Count(s => s.UserId.Equals(model.User.Id)) * 6;

                // user point update
                var userChange = await _userManager.FindByNameAsync(model.User.UserName);
                userChange.Points = model.User.Points;
                await _userManager.UpdateAsync(userChange);
            }
        }
        
        model.AwardsTab = model2;
        
        // tekrar aliyor useri
        var usr = await _userManager.FindByNameAsync(model.User.UserName);
        // roles
        var roles = await _userManager.GetRolesAsync(usr);
        model.Roles = roles.ToList();

        var checkData = _userManager
            .Users
            .Where(s => s.UserName.Equals(username) && s.BanUntill.Equals(null))
            .FirstOrDefault();

        if (checkData is not null)
            model.hasBan = false;

        else model.hasBan = true;
        
        return View(model);
    }

    [Authorize]
    [HttpPost("/forum/biri/{username}")]
    public async Task<IActionResult> UploadImage(string username,IFormFile file)
    {
        if (username is null)
        {
            return NotFound();
        }

        if (!username.Equals(User.Identity.Name))
        {
            return Unauthorized();
        }
        
        // controls
        if (file is null)
        {
            TempData["profile_message"] = "resim boş olamaz";
            return Redirect($"/forum/biri/{username}");
        }
        
        var user = _userManager.Users.Where(s => s.UserName.Equals(username)).FirstOrDefault();

        if (user is null)
        {
            TempData["profile_message"] = "bir şeyler ters gitti";
            return Redirect($"/forum/biri/{username}");
        }
        
        
        // file
        var uzanti = Path.GetExtension(file.FileName);

        if (uzanti != ".jpg" && uzanti != ".png" && uzanti != ".jpeg" && uzanti != ".webp")
        {
            TempData["profile_message"] = "sadece .jpg , .jpeg ve .png uzantılı dosyalar yüklenebilir";
            return Redirect($"/forum/biri/{username}");
        }

        var maxFileLength = 1 * 1024 * 1024;

        if (file.Length > maxFileLength)
        {
            TempData["profile_message"] = "Dosya 1 MB'dan büyük olamaz";
            return Redirect($"/forum/biri/{username}");
        }
        
        
        var name_without_extension = Path.GetFileNameWithoutExtension(file.FileName);
            
        var name = 
            $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(name_without_extension.Replace(' ', '-').ToLower()))}"+$"-{SlugModifier.GenerateUniqueHash()}";;
             
        var filename = name + uzanti;
            
        // file operation
        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/user", filename);
        
        // garbage collector hemen çalışsın bitsin
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        
        

        
        
        // user -- önceki resim silinsin / done
        user.Image = String.Concat("/images/user/", filename);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            TempData["profile_message"] = "bir şeyler ters gitti";
            return Redirect($"/forum/biri/{username}");
        }
        

        return Redirect($"/forum/biri/{username}");
    }
    
    [Authorize]
    public IActionResult Edit()
    {
        return View();
    }

    [Authorize]
    [HttpGet("editProfile/{username}")]
    public IActionResult editProfile(string? username)
    {
        if (username is null)
        {
            return NotFound();
        }
        
        // silinebilir
        if (!User.Identity.Name.Equals(username))
        {
            return NotFound();
        }
        
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new EditUserViewModel
            {
                Fullname = s.FullName,
                Place = s.Place,
                About = s.About,
                signature = s.signature,
                Gender = s.Gender,
                githubUrl = s.githubUrl,
                youtubeUrl = s.youtubeUrl,
                Job = s.Job,
                Userid = s.Id,
                emailActive = s.emailActive
            })
            .FirstOrDefault();
        
        return View(user);
    }

    [Authorize]
    [HttpPost("editProfile/{username}")]
    public async Task<IActionResult> editProfile(EditUserViewModel model)
    {
        // maxlenght için yapılcak tek şey burda doğrulamadır ancak gerek yok sorun olursa yapılır

        if (!(model.emailActive.Equals(false) || model.emailActive.Equals(true)))
        {
            // hatali durum
            return RedirectToAction("Index","Home");
        }
        
        var gender = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => s.Gender)
            .FirstOrDefault();

        if (model.Gender != gender && gender != null)
        {
            model.Gender = gender;
        }

        // user update
        var user = await _userManager.FindByIdAsync(model.Userid);

        if (user is not null)
        {
            if (User.Identity.Name == user.UserName)
            {
                var usercheck = user.UserName;
                
                // dto olsa buna gerek kalmazdı
                user.FullName = model.Fullname;
                user.Gender = model.Gender == "Cinsiyet" ? null : user.Gender;
                user.signature = model.signature;
                user.About = model.About;
                user.instagramUrl = model.instagramUrl;
                user.githubUrl = model.githubUrl;
                user.youtubeUrl = model.youtubeUrl;
                user.Place = model.Place;
                user.Job = model.Job;
                user.emailActive = model.emailActive;
                user.UserName = model.Username;
            
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (!usercheck.Equals(model.Username))
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        return RedirectToAction("Edit");    
                    }
                    
                }
                else
                {
                    ModelState.AddModelError("","güncelleme sırasında bir hata meydana geldi.Bu kullanıcı adı alınmış.");
                }
            }
            else
            {
                ModelState.AddModelError("","hata !");
            }
        }
        else
        {
            ModelState.AddModelError("","bir hata oluştu ...");
        }
        
        
        
        return View(model);
    }

    [Authorize]
    public IActionResult resetPassword()
    {
        return View();
    }
    
    // edit password
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> resetPassword([FromForm] ResetEditPassword model,string currentPass)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user is null)
            {
                return NotFound();
            }
            
            var result = await _userManager.ChangePasswordAsync(user,currentPass,model.Password);

            if (result.Succeeded)
            {
                return Redirect($"/editProfile/{User.Identity.Name}");
            }
            else
            {
                ModelState.AddModelError("","Mevcut şifre hatalı");
            }
        }
        
        
        return View(model);
    }

    // boşta bu
    [Authorize]
    public async Task<IActionResult> Fallow()
    {
        return Json(new
        {
            success = 1
        });
    }

    [Authorize]
    [HttpGet("/forum/report/{username}")]
    public IActionResult report(string username)
    {
        CreateReportDto dto = new CreateReportDto()
        {
            username = username
        };
        
        // burda giden username sayfada çıkar kim reporlancak
        return View("report",dto);
    }
    
    [Authorize]
    [HttpPost("/forum/report/{username}")]
    public async Task<IActionResult> report([FromForm] CreateReportDto dto,string username)
    {
        // burda ise username rapor eden kişi
        if (ModelState.IsValid)
        {
            dto.CreatedAt = DateTime.Now;
            dto.username = User.Identity.Name;
            dto.UserId = _userManager
                .Users
                .Where(s => s.UserName.Equals(username))
                .Select(s => s.Id)
                .FirstOrDefault();
            
            // birtek cause init edildi -- amk bu nasıl iştiki
            await _manager.ReportService.CreateReport(dto);

            return RedirectToAction("Index","Home");
        }
        
        return View(dto);
    }

    
    
    private void awardCheck(string userId, int awardId)
    {
        var check = _manager
            .AwardUserService
            .AwardUsers
            .Where(s => s.UserId.Equals(userId) && s.AwardsId.Equals(awardId))
            .FirstOrDefault();
                    
        if (check == null)
        {
            _manager.AwardUserService
                .GiveAward(new AwardUser()
                {
                    AwardsId = awardId,
                    UserId = userId,
                    CreatedAt = DateTime.Now
                });
        }
    }
}