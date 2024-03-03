using Entities.Dtos.SubjectDtos;
using Entities.Models;
using Entities.RequestParameters;
using GargamelinBurnu.Areas.Admin.Models.Subject;
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

    public IActionResult GetAllSubjects(CommonRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        
        var model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.User)
            .Include(s => s.Category)
            .Where(s => s.IsActive.Equals(true))
            .OrderByDescending(s => s.CreatedAt)
            .Take(10)
            .Select(s => new TableSubjectViewModel()
            {
                Username = s.User.UserName,
                CreatedAt = s.CreatedAt,
                categoryName = s.Category.CategoryName,
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

    public IActionResult waitingApproval(CommonRequestParameters? p)
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
            .OrderByDescending(s => s.CreatedAt)
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

    
    [HttpGet("admin/edit/{url}")]
    public IActionResult Edit(string url)
    {
         // Dto ile view model birleştirmek zahmetliydi ...
         // güvenlik amacı zaten yok - admin paneli la
        
        var subject = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.Category)
            .Include(s => s.User)
            .Where(s => s.Url.Equals(url))
            .Select(s => new EditSubjectViewModel()
            {
                IsActive = s.IsActive,
                CategoryName = s.Category.CategoryName,
                Content = s.Content,
                Title = s.Title,
                Prefix = s.Prefix,
                CreatedAt = s.CreatedAt,
                SubjectOwner = s.User.UserName,
                SubjectId = s.SubjectId,
            })
            .FirstOrDefault();

        
        
        return View(subject);
    }
    
    [HttpPost("admin/edit/{url}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditSubjectViewModel model)
    {
        if (ModelState.IsValid)
        {
            var dto = new UpdateSubjectDto();

            if (model.SubjectId != null)
            {
                dto.SubjectId = model.SubjectId;
                dto.Title = model.Title;
                dto.IsActive = model.IsActive;
                dto.Content = model.Content;
                dto.Prefix = model.Prefix;    
                
                _manager.SubjectService.UpdateSubject(dto);

                return RedirectToAction("waitingApproval");
            }
            else
            {
                ModelState.AddModelError("","bir şeyler ters gitti");
            }
        }

        return View(model);
    }
}