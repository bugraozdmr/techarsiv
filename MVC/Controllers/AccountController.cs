using Entities.Dtos;
using Entities.Models;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;


    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByNameAsync(model.Username);

            if (user is not null)
            {
                await _signInManager.SignOutAsync();
                
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);

                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockoutDate.Value - DateTime.UtcNow;

                    ModelState.AddModelError("",
                        $"Hesabınız kilitlendi, Lütfen {timeLeft.Minutes} dk sonra bekleyiniz.");
                }
                else
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınızı onaylayınız.");
                        return View(model);
                    }
                    
                    
                }
            }
            else
            {
                ModelState.AddModelError("", $"Kullanıcı adı ya da şifre hatalı.");
            }
        }
    
        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] RegisterDto model)
    {
        if (ModelState.IsValid)
        {
            var user = new User()
            {
                UserName = model.Username,
                FullName = model.FullName,
                Email = model.Email
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                var roleResult = await _userManager
                    .AddToRoleAsync(user, "User");

                if (roleResult.Succeeded)   // boş gitmesin
                    return RedirectToAction("Login");
                
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
        }
        return View(model);
    }
    
    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index","Home");
    }
}