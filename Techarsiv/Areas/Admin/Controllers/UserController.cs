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
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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
                .OrderByDescending(s => s.CreatedAt)
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


    [HttpGet("/Admin/User/giveRole/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> giveRole(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        
        if (user == null)
        {
            return NotFound();
        }
        List<string> userRoles = (await _userManager.GetRolesAsync(user)).ToList();
        
        
        
        return View(new UserRoleUpdateViewModel()
        {
            Roles = _roleManager.Roles.Select(r => r.Name).ToList(),
            UserRoles = userRoles
        });
    }

    
    [HttpPost("/Admin/User/giveRole/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> giveRole([FromForm] UserRoleUpdateViewModel model,string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null)
        {
            return NotFound();
        }
        
        if (model.Roles.Count > 0)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            // kullanıcının tüm rolleri gitti
            var r1 = await _userManager.RemoveFromRolesAsync(user, userRoles);
            var r2 = await _userManager.AddToRolesAsync(user, model.Roles);
        }
        
        return RedirectToAction("allUsers");
    }
}