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

    public UserController(UserManager<User> userManager,
        IServiceManager manager)
    {
        _userManager = userManager;
        _manager = manager;
    }

    [HttpGet("/biri/{username}")]
    public IActionResult Index(string username)
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
            .Select(s => new TitleViewModel()
            {
                Title = s.Title,
                Username = s.User.UserName,
                createdAt = s.CreatedAt,
                CommentCount = s.Comments.Count(),
                CategoryName = s.Category.CategoryName,
                Url = s.Url
            }).ToList();
        
        return View(model);
    }

    [Authorize]
    [HttpPost("/biri/{username}")]
    [ValidateAntiForgeryToken]
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
            return Redirect($"/biri/{username}");
        }
        
        var user = _userManager.Users.Where(s => s.UserName.Equals(username)).FirstOrDefault();

        if (user is null)
        {
            TempData["profile_message"] = "bir şeyler ters gitti";
            return Redirect($"/biri/{username}");
        }
        
        
        // file
        var uzanti = Path.GetExtension(file.FileName);

        if (uzanti != ".jpg" && uzanti != ".png")
        {
            TempData["profile_message"] = "sadece .jpg ve .png uzantılı dosyalar yüklenebilir";
            return Redirect($"/biri/{username}");
        }

        var maxFileLength = 1 * 1024 * 1024;

        if (file.Length > maxFileLength)
        {
            TempData["profile_message"] = "Dosya 1 MB'dan büyük olamaz";
            return Redirect($"/biri/{username}");
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

            string yol = $"/Users/bugra/Desktop/GargamelinBurnu/GargamelinBurnu/wwwroot{user.Image}";
            
            if (System.IO.File.Exists(yol) && !yol.Contains("/samples/"))
            {
                System.IO.File.Delete(yol);
            }
        }
        
        
        

        
        
        // user -- önceki resim silinsin / done
        user.Image = String.Concat("/images/user/", filename);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            TempData["profile_message"] = "bir şeyler ters gitti";
            return Redirect($"/biri/{username}");
        }
        

        return Redirect($"/biri/{username}");
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
                Userid = s.Id
            })
            .FirstOrDefault();
        
        return View(user);
    }

    [Authorize]
    [HttpPost("editProfile/{username}")]
    public async Task<IActionResult> editProfile(EditUserViewModel model)
    {
        // maxlenght için yapılcak tek şey burda doğrulamadır ancak gerek yok sorun olursa yapılır
        
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
                user.FullName = model.Fullname;
                user.Gender = model.Gender;
                user.signature = model.signature;
                user.About = model.About;
                user.instagramUrl = model.instagramUrl;
                user.githubUrl = model.githubUrl;
                user.youtubeUrl = model.youtubeUrl;
                user.Place = model.Place;
                user.Job = model.Job;
            
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Edit");
                }
                else
                {
                    ModelState.AddModelError("","güncelleme sırasında bir hata meydana geldi.");
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
}