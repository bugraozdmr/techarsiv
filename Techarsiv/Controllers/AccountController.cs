using System.Web;
using Entities.Dtos;
using Entities.Models;
using GargamelinBurnu.Infrastructure.Helpers.Contracts;
using GargamelinBurnu.Models;
using GargamelinBurnu.Models.Userpage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Services.Helpers;


namespace GargamelinBurnu.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IDistributedCache _cache;




    public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailSender emailSender, IDistributedCache cache)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _cache = cache;
    }

    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View("LoginRegister");
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] RegisterLoginViewModel model)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        if (ModelState.IsValid)
        {
            
            
            User user;
            // username eposta'da olabilir
            if (model.LoginModel.Username.Contains("@") && model.LoginModel.Username.Contains(".com"))
            {
                user = await _userManager.FindByEmailAsync(model.LoginModel.Username);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.LoginModel.Username);    
            }
            

            if (user is not null)
            {
                await _signInManager.SignOutAsync();

                var result = await _signInManager
                    .PasswordSignInAsync(user, model.LoginModel.Password,
                        model.LoginModel.RememberMe, true);
                
                if (result.Succeeded)
                {
                    //await _userManager.ResetAccessFailedCountAsync(user);
                    //await _userManager.SetLockoutEndDateAsync(user, null);

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
                    
                    bool passwordCorrect = await _userManager.CheckPasswordAsync(user, model.LoginModel.Password);

                    if (!passwordCorrect)
                    {
                        ModelState.AddModelError("", $"Girilen bilgiler hatalı.");
                        return View("LoginRegister",model);
                    }
                    
                    // mail onay kaldırıldı
                    /*if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınızı onaylayınız.");
                        return View("LoginRegister",model);
                    }*/

                    
                    ModelState.AddModelError("",$"Girdiğiniz bilgiler hatalı.");    
                }
            }
            else
            {
                ModelState.AddModelError("", $"Girilen bilgiler hatalı");
            }
        }
    
        return View("LoginRegister",model);
    }

    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View("LoginRegister");
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterLoginViewModel model)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        if (ModelState.IsValid)
        {
            var user = new User()
            {
                UserName = model.RegisterDto.Username,
                FullName = model.RegisterDto.FullName,
                Email = model.RegisterDto.Email,
                CreatedAt = DateTime.UtcNow
            };
            
            Random rnd = new Random();
            int randomSayi = rnd.Next(1, 11);

            user.Image = $"/images/user/samples/avatar_{randomSayi}.jpg";
            user.emailActive = false;
            user.canTakeEmail = false;
            
            /*
            // mail
            
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    
            var encoded_token = HttpUtility.UrlEncode(token);
            
            var url = Url.Action("ConfirmEmail", "Account",new {id=user.Id,token=encoded_token});
            
            // email

            int result2 = await _emailSender.SendEmailAsync(user.Email, "Hesap onayı",
                $"<h3 style=\"text-align: center;margin-bottom: 20px; \">Teşekkürler</h3>\n    <p style=\"color:orange\">Merhaba {user.UserName},</p>\n    <p>Katılımınızı onayladığınız için teşekkür ediyoruz sizleri aramızda görmek için sabırsızlıkla bekliyoruz.</p>\n    <p>Sorun ve şikayetleriniz için <a href=\"mailto:support@techarsiv.com\" style=\"text-decoration: none;\">support@techariv.com</a></p>    <div style=\"text-align: center;\">\n        <a href=\"https://techarsiv.com{url}\" style=\"display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none;margin-top:10px;border-radius:5px\">Tıkla</a>\n    </div>"
                );
            */
            
            var result = await _userManager.CreateAsync(user, model.RegisterDto.Password);
            
            if (result.Succeeded)
            {
                var roleResult = await _userManager
                    .AddToRoleAsync(user, "User");

                if (roleResult.Succeeded)
                {
                    //TempData["message_login"] = "email hesabınızdaki onay mailine tıklayın,maili göremiyorsanız spamı kontrol etmeyi unutmayın.";
                    
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
        return View("LoginRegister",model);
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
            TempData["confirmEmail"] = "Geçersiz Token";
            return View();
        }
        
        var user = await _userManager.FindByIdAsync(id);

        if (user is not null)
        {
            var decodedToken = HttpUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user,decodedToken);
            
            if (result.Succeeded)
            {
                TempData["confirmEmail"] = "Hesabınız onaylandı";
                return View();
            }

            TempData["confirmEmail"] = "Bir şeyler ters gitti.";
            return View();
        }
        
        
        TempData["confirmEmail"] = "Kullanıcı bulunamadı";
        return View();
    }
    
    
    [HttpPost]
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
            $"<h3 style=\"text-align: center;margin-bottom: 20px; \">Şifre Sıfırla</h3>\n    <p style=\"color:orange\">Merhaba {user.UserName},</p>\n    <p>Şifrenizi sıfırlamak için linke tıklamanız gerekiyor.</p>\n    <p>Sorun ve şikayetleriniz için <a href=\"mailto:support@techarsiv.com\" style=\"text-decoration: none;\">support@techariv.com</a></p>    <div style=\"text-align: center;\">\n        <a href=\"https://techarsiv.com{url}\" style=\"display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none;margin-top:10px;border-radius:5px\">Tıkla</a>\n    </div>");

        TempData["message_reset"] = "eposta adresinize gönderilen link ile şifrenizi sıfırlayabilirsiniz ,maili göremiyorsanız spamı kontrol etmeyi unutmayın.";
        return View();
        
    }
    
    
    public IActionResult ResetPassword(string id,string token)
    {
        if (id == null || token == null)
        {
            TempData["message_login"] = "değerler boş";
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordModel() { Token = token ,Id = id};
        
        return View(model);
        
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user is null)
            {
                TempData["message_login"] = "Bir hata oluştu.";
                return RedirectToAction("Index", "Home");
            }
            
            var decodedToken = HttpUtility.UrlDecode(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken,model.Password);

            if (result.Succeeded)
            {
                TempData["message_login"] = "Şifre değiştirildi.";
                return RedirectToAction("Login");
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("",err.Description);
            }
        }
        
        return View(model);
        
    }

    public IActionResult resendMail()
    {
        // iptal
        return RedirectToAction("Login");
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> resendMail(string Email)
    {
        // iptal
        return RedirectToAction("Login");
        
        
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

        
        
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    
        var encoded_token = HttpUtility.UrlEncode(token);
            
        var url = Url.Action("ConfirmEmail", "Account",new {id=user.Id,token=encoded_token});
            
        // email

        int result2 = await _emailSender.SendEmailAsync(user.Email, "Hesap onayı",
            $"<h3 style=\"text-align: center;margin-bottom: 20px; \">Özür dileriz</h3>\n    <p style=\"color:orange\">Merhaba {user.UserName},</p>\n    <p>Katılımınızı onayladığınız için teşekkür ediyoruz sizleri aramızda görmek için sabırsızlıkla bekliyoruz.</p>\n    <p>Sorun ve şikayetleriniz için <a href=\"mailto:support@techarsiv.com\" style=\"text-decoration: none;\">support@techariv.com</a></p>    <div style=\"text-align: center;\">\n        <a href=\"https://techarsiv.com{url}\" style=\"display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none;margin-top:10px;border-radius:5px\">Tıkla</a>\n    </div>"
        );


        TempData["message_login"] = "mailinizi kontrol edin";
        return RedirectToAction("Login");

    }
    

    public IActionResult AccessDenied()
    {
        return View();
    }
}