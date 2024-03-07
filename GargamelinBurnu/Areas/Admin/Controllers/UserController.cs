using Entities.Models;
using Entities.RequestParameters;
using GargamelinBurnu.Areas.Admin.Models.Users;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area(areaName:"Admin")]
[Authorize(Roles = "Admin, Moderator")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    
    public IActionResult allUsers(CommonRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;


        PaginationUserViewModel realModel = new PaginationUserViewModel();
        
        
        List<UserViewModel> model = new List<UserViewModel>();

        int total_count;
        
        if (p.SearchTerm == null || p.SearchTerm == "")
        {
            model = _userManager
                .Users
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new UserViewModel()
                {
                    Username = s.UserName,
                    CreatedAt = s.CreatedAt
                }).ToList();
            
            
            total_count = _userManager
                .Users
                .Count();
        }
        else
        {
            model = _userManager
                .Users
                .Where(s => s.UserName.Contains(p.SearchTerm.ToLower()))
                .Skip((p.PageNumber-1)*p.Pagesize)
                .Take(p.Pagesize)
                .Select(s => new UserViewModel()
                {
                    Username = s.UserName,
                    CreatedAt = s.CreatedAt
                }).ToList();
            
            realModel.Param = $"SearchTerm={p.SearchTerm}";
            realModel.area = $"/Admin/User";
            
            total_count = _userManager
                .Users
                .Count(s => s.UserName.Contains(p.SearchTerm.ToLower()));
        }
        
        
        var pagination = new Pagination()
        {
            CurrentPage = p.PageNumber,
            ItemsPerPage = p.Pagesize,
            TotalItems = total_count
        };

        realModel.List = model;
        realModel.Pagination = pagination;
        
        return View(realModel);
    }
}