using Entities.Dtos.Question;
using Entities.Models;
using Entities.RequestParameters;
using GargamelinBurnu.Areas.Admin.Models.Article;
using GargamelinBurnu.Areas.Admin.Models.Question;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using Services.Helpers;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class QuestionController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;

    public QuestionController(UserManager<User> userManager, 
        IServiceManager manager)
    {
        _userManager = userManager;
        _manager = manager;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Index([FromForm] CreateQuestionDto dto,IFormFile file)
    {
        if (ModelState.IsValid)
        {
            if (file is null)
            {
                ModelState.AddModelError("","file boş olamaz");
                return View(dto);
            }
            
            var uzanti = Path.GetExtension(file.FileName);
            
            if (uzanti != ".jpg" && uzanti != ".png" && uzanti != ".jpeg" && uzanti != ".webp")
            {
                ModelState.AddModelError("","uzantı hatalı");
                return View(dto);
            }
            
            var maxFileLength = 2 * 1024 * 1024;

            if (file.Length > maxFileLength)
            {
                ModelState.AddModelError("","dosya 2 mb'dan büyük olamaz");
                return View(dto);
            }
            
            var name_without_extension = Path.GetFileNameWithoutExtension(file.FileName);

            var name = 
                $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(name_without_extension.Replace(' ', '-').ToLower()))}"+$"-{SlugModifier.GenerateUniqueHash()}";;
             
            var filename = name + uzanti;
            
            // file operation
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/questions", filename);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            dto.Image = String.Concat("/images/questions/", filename);

            dto.UserId = _userManager
                .Users
                .Where(s => s.UserName.Equals(User.Identity.Name))
                .Select(s => s.Id)
                .FirstOrDefault();

            // await !
            await _manager.QuestionService.CreateQuestion(dto);


            return RedirectToAction("getAllArticles","Article");
        }
        
        return View(dto);
    }

    public IActionResult getAllQuestions(CommonRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;

        List<QuestionViewModel> model = new List<QuestionViewModel>();
        PaginationQuestionViewModel RealModel = new PaginationQuestionViewModel(); 
        int total_count;

        List<string> listCheck = ["A", "B", "C", "D"];
        
        if (p.SearchTerm == null || p.SearchTerm == "")
        {
            model = _manager
                .QuestionService
                .GetAllQuestion(false)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new QuestionViewModel()
                {
                    createdAt = s.CreatedAt,
                    questionDesc = s.QuestionDesc,
                    RightAnswer = listCheck[s.RightAnswer-1],
                    Username = s.User.UserName,
                    Url = s.Url
                }).ToList();

            total_count = _manager
                .QuestionService
                .GetAllQuestion(false)
                .Count();
        }
        else
        {
            // hata var gibi
            model = _manager
                .QuestionService
                .GetAllQuestion(false)
                .Include(s => s.User)
                .Where(s => s.QuestionDesc.Contains(p.SearchTerm.ToLower()))
                .OrderByDescending(s => s.CreatedAt)
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new QuestionViewModel()
                {
                    Username = s.User.UserName,
                    createdAt = s.CreatedAt,
                    questionDesc = s.QuestionDesc,
                    RightAnswer = listCheck[s.RightAnswer-1],
                    Url = s.Url
                }).ToList();

            total_count = _manager
                .QuestionService
                .GetAllQuestion(false)
                .Count(s => s.QuestionDesc.Contains(p.SearchTerm.ToLower()));
            
            RealModel.Param = $"SearchTerm={p.SearchTerm}";
            RealModel.area = $"/Admin/Question";
        }
        
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        RealModel.List = model;
        RealModel.Pagination = pagination;
        
        
        return View(RealModel);
    }
    
    [HttpGet("admin/question/edit/{url}")]
    public IActionResult Edit(string url)
    {
        var question = _manager
            .QuestionService
            .GetAllQuestion(false)
            .Include(s => s.User)
            .Where(s => s.Url.Equals(url))
            .Select(s => new EditQuestionViewModel()
            {
                CreatedAt = s.CreatedAt,
                QuestionId = s.QuestionId,
                Url = s.Url,
                questionDesc = s.QuestionDesc,
                RightAnswer = s.RightAnswer,
                ChoiceA = s.ChoiceA,
                ChoiceB = s.ChoiceB,
                ChoiceC = s.ChoiceC,
                ChoiceD = s.ChoiceD
            })
            .FirstOrDefault();
        
        return View(question);
    }
    
    [HttpPost("admin/question/edit/{url}")]
    public async Task<IActionResult> Edit(EditQuestionViewModel model,IFormFile? file,string url)
    {
        // çok salak saçma bir işlem -- modelden alıp dto doldurmak ...
        if (ModelState.IsValid)
        {
            var dto = new EditQuestionDto();

            if (model.QuestionId != null)
            {
                dto.QuestionId = model.QuestionId;
                dto.QuestionDesc = model.questionDesc;
                dto.ChoiceA = model.ChoiceA;
                dto.ChoiceB = model.ChoiceB;
                dto.ChoiceC = model.ChoiceC;
                dto.ChoiceD = model.ChoiceD;
                dto.RightAnswer = model.RightAnswer;

                if (file is not null)
                {
                    var uzanti = Path.GetExtension(file.FileName);
            
                    if (uzanti != ".jpg" && uzanti != ".png" && uzanti != ".jpeg" && uzanti != ".webp")
                    {
                        ModelState.AddModelError("","uzantı hatalı");
                        return View(model);
                    }

                    var maxFileLength = 2 * 1024 * 1024;

                    if (file.Length > maxFileLength)
                    {
                        ModelState.AddModelError("","dosya 2 mb'dan büyük olamaz");
                        return View(model);
                    }
            
                    var name_without_extension = Path.GetFileNameWithoutExtension(file.FileName);
                    
                    var name = 
                        $"{SlugModifier.RemoveNonAlphanumericAndSpecialChars(SlugModifier.ReplaceTurkishCharacters(name_without_extension.Replace(' ', '-').ToLower()))}"+$"-{SlugModifier.GenerateUniqueHash()}";;
             
                    var filename = name + uzanti;
            
                    // file operation
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/questions", filename);

                    // garbage collector hemen çalışsın bitsin
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    dto.Image = String.Concat("/images/questions/", filename);
                }
                
                
                await _manager.QuestionService.UpdateQuestion(dto);

                return RedirectToAction("getAllQuestions");
            }
            else
            {
                ModelState.AddModelError("","bir şeyler ters gitti");
            }
        }
        
        return View(model);
    }
}