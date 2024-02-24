using System.Web;
using Entities.Dtos;
using Entities.Models;
using GargamelinBurnu.Infrastructure.Helpers.Contracts;
using GargamelinBurnu.Models;
using GargamelinBurnu.Models.Userpage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Helpers;


namespace GargamelinBurnu.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender _emailSender;


    public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginModel model)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
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
                    
                    bool passwordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

                    if (!passwordCorrect)
                    {
                        ModelState.AddModelError("", $"Kullanıcı adı ya da şifre hatalı.");
                        return View(model);
                    }
                    
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınızı onaylayınız.");
                        return View(model);
                    }

                    
                    ModelState.AddModelError("",$"Parolanız hatalı.");    
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
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] RegisterDto model)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        if (ModelState.IsValid)
        {
            var user = new User()
            {
                UserName = model.Username,
                FullName = model.FullName,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };
            
            Random rnd = new Random();
            int randomSayi = rnd.Next(1, 11);

            user.Image = $"/images/user/samples/avatar_{randomSayi}.jpg";
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                var roleResult = await _userManager
                    .AddToRoleAsync(user, "User");

                if (roleResult.Succeeded)
                {
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    
                    var encoded_token = HttpUtility.UrlEncode(token);
                    
                    var url = Url.Action("ConfirmEmail", "Account",new {id=user.Id,token=encoded_token});
                
                    // email

                    await _emailSender.SendEmailAsync(user.Email, "Hesap onayı",
                        $"Lütfen email hesabınızı onaylamak için maile " +
                        $"<a href='https://localhost:7056{url}'>tıklayın</a>");
                
                
                    TempData["message"] = "email hesabınızdaki onay mailine tıklayın.";
                    
                    return RedirectToAction("Login");
                }
                    
                
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
    
    public IActionResult ForgotPassword()
    {
        return View();
    }
    
    public async Task<IActionResult> ConfirmEmail(string id, string token)
    {
        if (id == null || token == null)
        {
            TempData["message"] = "Geçersiz Token";
            return View();
        }
        
        var user = await _userManager.FindByIdAsync(id);

        if (user is not null)
        {
            var decodedToken = HttpUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user,decodedToken);
            
            if (result.Succeeded)
            {
                TempData["message"] = "Hesabınız onaylandı";
                return View();
            }

            TempData["message"] = "Bir şeyler ters gitti.";
            return View();
        }
        
        
        TempData["message"] = "Kullanıcı bulunamadı";
        return View();
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword([FromForm] string Email)
    {
        if (string.IsNullOrEmpty(Email))
        {
            return View();
        }

        var user = await _userManager.FindByEmailAsync(Email);

        if (user is null)
        {
            ModelState.AddModelError("","Bu mail sistemde bulunmuyor.");
            return View();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        var encoded_token = HttpUtility.UrlEncode(token);
        
        var url = Url.Action("ResetPassword", "Account",new {id=user.Id,token=encoded_token});
                
        // email

        await _emailSender.SendEmailAsync(user.Email, "Parola Sıfırlama",
            $"Şifrenizi değiştirmek için " +
            $"<strong><a href='https://localhost:7056{url}'>tıklayın</a></strong>");

        TempData["message"] = "eposta adresinize gönderilen link ile şifrenizi sıfırlayabilirsiniz.";
        return View();
        
    }
    
    
    public IActionResult ResetPassword(string id,string token)
    {
        if (id == null || token == null)
        {
            TempData["message"] = "değerler boş";
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordModel() { Token = token ,Id = id};
        
        return View(model);
        
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user is null)
            {
                TempData["message"] = "Bir hata oluştu.";
                return RedirectToAction("Index", "Home");
            }
            
            var decodedToken = HttpUtility.UrlDecode(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken,model.Password);

            if (result.Succeeded)
            {
                TempData["message"] = "Şifre değiştirildi.";
                return RedirectToAction("Login");
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("",err.Description);
            }
        }
        
        return View(model);
        
    }
}