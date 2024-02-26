using Entities.RequestParameters;
using GargamelinBurnu.Areas.Admin.Models.Tables;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SubjectController : Controller
{
    private readonly IServiceManager _manager;

    public SubjectController(IServiceManager manager)
    {
        _manager = manager;
    }

    public IActionResult GetAllSubjects(SubjectRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        
        var model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.User)
            .Where(s => s.IsActive.Equals(true))
            .OrderBy(s => s.CreatedAt)
            .Take(10)
            .Select(s => new TableSubjectViewModel()
            {
                Username = s.User.UserName,
                CreatedAt = s.CreatedAt,
                SubjectUrl = s.Url,
                Title = s.Title
            }).ToList();

        int total_count = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Count();
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        var realModel = new PaginationSubjectListViewModel()
        {
            List = model,
            Pagination = pagination
        };
        
        
        return View(realModel);
    }

    public IActionResult waitingApproval(SubjectRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        
        var model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.User)
            .Include(s => s.Category)
            .Where(s => s.IsActive.Equals(false))
            .OrderBy(s => s.CreatedAt)
            .Take(10)
            .Select(s => new TableSubjectViewModel()
            {
                Username = s.User.UserName,
                CreatedAt = s.CreatedAt,
                SubjectUrl = s.Url,
                Title = s.Title,
                categoryName = s.Category.CategoryName
            }).ToList();

        int total_count = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Count();
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        var realModel = new PaginationSubjectListViewModel()
        {
            List = model,
            Pagination = pagination
        };
        
        
        return View(realModel);
    }
}