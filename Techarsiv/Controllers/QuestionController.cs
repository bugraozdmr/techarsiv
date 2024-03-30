using Entities.RequestParameters;
using GargamelinBurnu.Models;
using GargamelinBurnu.Models.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

[Authorize(Roles = "Admin")]
public class QuestionController : Controller
{
    private readonly IServiceManager _manager;

    public QuestionController(IServiceManager manager)
    {
        _manager = manager;
    }

    [HttpGet("/forum/sorular")]
    public IActionResult Index(CommonRequestParameters p)
    {
        List<QuestionViewModel> model;
        
        model = _manager
            .QuestionService
            .GetAllQuestion(false)
            .Include(s => s.User)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((p.PageNumber-1)*(p.Pagesize))
            .Take(p.Pagesize)
            .Select(s => new QuestionViewModel()
            {
                CreatedAt = s.CreatedAt,
                QuestionDesc = s.QuestionDesc,
                Username = s.User.UserName,
                UserImage = s.User.Image,
                Url = s.Url
            })
            .ToList();

        var realModel = new PaginationQuestionViewModel();
        realModel.List = model;
        
        int total_count = _manager
            .QuestionService
            .GetAllQuestion(false)
            .Count();
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };
        
        realModel.Pagination = pagination;
        
        return View(realModel);
    }

    [HttpGet("forum/soru/{url}")]
    public async Task<IActionResult> Details(string url)
    {
        var question = _manager
            .QuestionService
            .GetAllQuestion(false)
            .Select(s => new QuestionPageViewModel()
            {
                ChoiceA = s.ChoiceA,
                ChoiceB = s.ChoiceB,
                ChoiceC = s.ChoiceC,
                ChoiceD = s.ChoiceD,
                Image = s.Image,
                rightAnswer = s.RightAnswer,
                QuestionDesc =s.QuestionDesc
            })
            .FirstOrDefault();

        return View(question);
    }
}