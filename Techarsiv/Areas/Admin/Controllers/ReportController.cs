using Entities.RequestParameters;
using GargamelinBurnu.Areas.Admin.Models.Reports;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin, Moderator")]
public class ReportController : Controller
{
    private readonly IServiceManager _manager;

    public ReportController(IServiceManager manager)
    {
        _manager = manager;
    }


    public IActionResult Index(CommonRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;


        var reports = _manager
            .ReportService
            .GetAllReports(false)
            .Include(s => s.User)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((p.PageNumber-1)*p.Pagesize)
            .Take(p.Pagesize)
            .Select(s => new ReportViewModel()
            {
                CreatedAt = s.CreatedAt,
                ReportUsername = s.User.UserName,
                Username = s.username,
                Cause = s.Cause,
                reportId = s.ReportId
            })
            .ToList();

        
        
        int total_count = _manager
            .ReportService
            .GetAllReports(false)
            .Count();
        
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };
        
        var realModel = new PaginationReportViewModel()
        {
            List = reports,
            Pagination = pagination
        };
        
        
        return View(realModel);
    }

    [HttpPost("{reportId}")]
    public async Task<IActionResult> removeReport(int reportId)
    {
        await _manager.ReportService.deleteOneReport(reportId);

        return RedirectToAction("Index");
    }
}