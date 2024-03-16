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
        
        return View("LoginRegister");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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

                var result = await _signInManager.PasswordSignInAsync(user, model.LoginModel.Password, model.LoginModel.RememberMe, true);
                
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
                    
                    bool passwordCorrect = await _userManager.CheckPasswordAsync(user, model.LoginModel.Password);

                    if (!passwordCorrect)
                    {
                        ModelState.AddModelError("", $"Girilen bilgiler hatalı.");
                        return View("LoginRegister",model);
                    }
                    
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınızı onaylayınız.");
                        return View("LoginRegister",model);
                    }

                    
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
    [ValidateAntiForgeryToken]
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
            
            
            var result = await _userManager.CreateAsync(user, model.RegisterDto.Password);
            
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
                        $"<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"en\">\n <head>\n  <meta charset=\"UTF-8\">\n  <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\n  <meta name=\"x-apple-disable-message-reformatting\">\n  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n  <meta content=\"telephone=no\" name=\"format-detection\">\n    <style type=\"text/css\">\n    a {{text-decoration: none;}}\n    </style>\n    <![endif]--><!--[if gte mso 9]><style>sup {{ font-size: 100% !important; }}</style><![endif]--><!--[if gte mso 9]>\n<xml>\n    <o:OfficeDocumentSettings>\n    <o:AllowPNG></o:AllowPNG>\n    <o:PixelsPerInch>96</o:PixelsPerInch>\n    </o:OfficeDocumentSettings>\n</xml>\n<![endif]-->\n  <style type=\"text/css\">\n.rollover:hover .rollover-first {{\n  max-height:0px!important;\n  display:none!important;\n  }}\n  .rollover:hover .rollover-second {{\n  max-height:none!important;\n  display:block!important;\n  }}\n  .rollover span {{\n  font-size:0px;\n  }}\n  u + .body img ~ div div {{\n  display:none;\n  }}\n  #outlook a {{\n  padding:0;\n  }}\n  span.MsoHyperlink,\nspan.MsoHyperlinkFollowed {{\n  color:inherit;\n  mso-style-priority:99;\n  }}\n  a.es-button {{\n  mso-style-priority:100!important;\n  text-decoration:none!important;\n  }}\n  a[x-apple-data-detectors] {{\n  color:inherit!important;\n  text-decoration:none!important;\n  font-size:inherit!important;\n  font-family:inherit!important;\n  font-weight:inherit!important;\n  line-height:inherit!important;\n  }}\n  .es-desk-hidden {{\n  display:none;\n  float:left;\n  overflow:hidden;\n  width:0;\n  max-height:0;\n  line-height:0;\n  mso-hide:all;\n  }}\n  .es-button-border:hover > a.es-button {{\n  color:#ffffff!important;\n  }}\n@media only screen and (max-width:600px) {{.es-m-p0r {{ padding-right:0px!important }} *[class=\"gmail-fix\"] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} h1 {{ font-size:36px!important; text-align:left }} h2 {{ font-size:26px!important; text-align:left }} h3 {{ font-size:20px!important; text-align:left }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }} .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:36px!important }} .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:26px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:12px!important }} .es-header-body p, .es-header-body a {{ font-size:14px!important }} .es-content-body p, .es-content-body a {{ font-size:14px!important }} .es-footer-body p, .es-footer-body a {{ font-size:14px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }} .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }} .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover span, .es-m-txt-c .rollover span, .es-m-txt-l .rollover span {{ line-height:0!important; font-size:0!important }} .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:20px!important; line-height:120%!important }} a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }} table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }} table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .es-social td {{ padding-bottom:10px }} .h-auto {{ height:auto!important }} }}\n@media screen and (max-width:384px) {{.mail-message-content {{ width:414px!important }} }}\n</style>\n </head>\n <body class=\"body\" style=\"width:100%;height:100%;padding:0;Margin:0\">\n  <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"en\" style=\"background-color:#FAFAFA\"><!--[if gte mso 9]>\n\t\t\t<v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\">\n\t\t\t\t<v:fill type=\"tile\" color=\"#fafafa\"></v:fill>\n\t\t\t</v:background>\n\t\t<![endif]-->\n   <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FAFAFA\">\n     <tr>\n      <td valign=\"top\" style=\"padding:0;Margin:0\">\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td class=\"es-info-area\" align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px\" bgcolor=\"#FFFFFF\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-header\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table bgcolor=\"#ffffff\" class=\"es-header-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px\">\n             <tr>\n              <td align=\"left\" style=\"Margin:0;padding-top:10px;padding-right:20px;padding-bottom:10px;padding-left:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td class=\"es-m-p0r\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     \n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\n             <tr>\n              <td align=\"left\" style=\"padding:0;Margin:0;padding-right:20px;padding-left:20px;padding-top:15px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;padding-top:15px;padding-bottom:15px\"><h1 style=\"Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:46px;font-style:normal;font-weight:bold;line-height:55px;color:#333333\">Katıldığın için teşekkürler</h1></td>\n                     </tr>\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:24px;letter-spacing:0;color:#333333;font-size:16px\">Merhaba, <strong>{user.UserName}</strong> Siteme üye olduğun için teşekkürler! Siteyi kullanabilmeye bir adım kaldı.</p></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n             <tr>\n              <td class=\"esdev-adapt-off\" align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:560px\">\n                 <tr>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr class=\"es-mobile-hidden\">\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:146px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:121px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:155px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\n                     <tr class=\"es-mobile-hidden\">\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:138px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n             <tr>\n              <td align=\"left\" style=\"padding:0;Margin:0;padding-right:20px;padding-bottom:10px;padding-left:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:5px\" role=\"presentation\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><span class=\"es-button-border\" style=\"border-style:solid;border-color:#2CB543;background:#5C68E2;border-width:0px;display:inline-block;border-radius:6px;width:auto\"><a href=\"https://techarsiv.com{url}\" class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none !important;mso-line-height-rule:exactly;color:#FFFFFF;font-size:20px;padding:10px 30px 10px 30px;display:inline-block;background:#5C68E2;border-radius:6px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:24px;width:auto;text-align:center;letter-spacing:0;mso-padding-alt:0;mso-border-alt:10px solid #5C68E2;border-left-width:30px;border-right-width:30px\">Hesabını Onayla</a></span></td>\n                     </tr>\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;padding-bottom:10px;padding-top:20px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">Sorun mu var ? Mail gönder &nbsp;<a target=\"_blank\" href=\"https://techarsiv.com/\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\">support@techarsiv.com</a></p><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\"><br>Teşekkürler,</p><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">techarsiv.com</p></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-footer-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:640px\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"Margin:0;padding-right:20px;padding-left:20px;padding-bottom:20px;padding-top:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"left\" style=\"padding:0;Margin:0;width:600px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;padding-top:15px;padding-bottom:15px;font-size:0\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-table-not-adapt es-social\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><a href=\"\"><img title=\"Facebook\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/facebook-logo-black.png\" alt=\"Fb\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><img title=\"X.com\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/x-logo-black.png\" alt=\"X\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><img title=\"Instagram\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/instagram-logo-black.png\" alt=\"Inst\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0\"><img title=\"Youtube\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/youtube-logo-black.png\" alt=\"Yt\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td class=\"es-info-area\" align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px\" bgcolor=\"#FFFFFF\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table></td>\n     </tr>\n   </table>\n  </div>\n </body>\n</html>"
                        );
                
                
                    TempData["message_login"] = "email hesabınızdaki onay mailine tıklayın,maili göremiyorsanız spamı kontrol etmeyi unutmayın.";
                    
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
            $"<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"en\">\n <head>\n  <meta charset=\"UTF-8\">\n  <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\n  <meta name=\"x-apple-disable-message-reformatting\">\n  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n  <meta content=\"telephone=no\" name=\"format-detection\">\n    <style type=\"text/css\">\n    a {{text-decoration: none;}}\n    </style>\n    <![endif]--><!--[if gte mso 9]><style>sup {{ font-size: 100% !important; }}</style><![endif]--><!--[if gte mso 9]>\n<xml>\n    <o:OfficeDocumentSettings>\n    <o:AllowPNG></o:AllowPNG>\n    <o:PixelsPerInch>96</o:PixelsPerInch>\n    </o:OfficeDocumentSettings>\n</xml>\n<![endif]-->\n  <style type=\"text/css\">\n.rollover:hover .rollover-first {{\n  max-height:0px!important;\n  display:none!important;\n  }}\n  .rollover:hover .rollover-second {{\n  max-height:none!important;\n  display:block!important;\n  }}\n  .rollover span {{\n  font-size:0px;\n  }}\n  u + .body img ~ div div {{\n  display:none;\n  }}\n  #outlook a {{\n  padding:0;\n  }}\n  span.MsoHyperlink,\nspan.MsoHyperlinkFollowed {{\n  color:inherit;\n  mso-style-priority:99;\n  }}\n  a.es-button {{\n  mso-style-priority:100!important;\n  text-decoration:none!important;\n  }}\n  a[x-apple-data-detectors] {{\n  color:inherit!important;\n  text-decoration:none!important;\n  font-size:inherit!important;\n  font-family:inherit!important;\n  font-weight:inherit!important;\n  line-height:inherit!important;\n  }}\n  .es-desk-hidden {{\n  display:none;\n  float:left;\n  overflow:hidden;\n  width:0;\n  max-height:0;\n  line-height:0;\n  mso-hide:all;\n  }}\n  .es-button-border:hover > a.es-button {{\n  color:#ffffff!important;\n  }}\n@media only screen and (max-width:600px) {{.es-m-p0r {{ padding-right:0px!important }} *[class=\"gmail-fix\"] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} h1 {{ font-size:36px!important; text-align:left }} h2 {{ font-size:26px!important; text-align:left }} h3 {{ font-size:20px!important; text-align:left }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }} .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:36px!important }} .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:26px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:12px!important }} .es-header-body p, .es-header-body a {{ font-size:14px!important }} .es-content-body p, .es-content-body a {{ font-size:14px!important }} .es-footer-body p, .es-footer-body a {{ font-size:14px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }} .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }} .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover span, .es-m-txt-c .rollover span, .es-m-txt-l .rollover span {{ line-height:0!important; font-size:0!important }} .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:20px!important; line-height:120%!important }} a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }} table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }} table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .es-social td {{ padding-bottom:10px }} .h-auto {{ height:auto!important }} }}\n@media screen and (max-width:384px) {{.mail-message-content {{ width:414px!important }} }}\n</style>\n </head>\n <body class=\"body\" style=\"width:100%;height:100%;padding:0;Margin:0\">\n  <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"en\" style=\"background-color:#FAFAFA\"><!--[if gte mso 9]>\n\t\t\t<v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\">\n\t\t\t\t<v:fill type=\"tile\" color=\"#fafafa\"></v:fill>\n\t\t\t</v:background>\n\t\t<![endif]-->\n   <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FAFAFA\">\n     <tr>\n      <td valign=\"top\" style=\"padding:0;Margin:0\">\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td class=\"es-info-area\" align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px\" bgcolor=\"#FFFFFF\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-header\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table bgcolor=\"#ffffff\" class=\"es-header-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px\">\n             <tr>\n              <td align=\"left\" style=\"Margin:0;padding-top:10px;padding-right:20px;padding-bottom:10px;padding-left:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td class=\"es-m-p0r\" valign=\"top\" align=\"center\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     \n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\n             <tr>\n              <td align=\"left\" style=\"padding:0;Margin:0;padding-right:20px;padding-left:20px;padding-top:15px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;padding-top:15px;padding-bottom:15px\"><h1 style=\"Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:46px;font-style:normal;font-weight:bold;line-height:55px;color:#333333\">Şifre Sıfırlama</h1></td>\n                     </tr>\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:24px;letter-spacing:0;color:#333333;font-size:16px\">Merhaba, <strong>{user.UserName}</strong> şifreni sıfırlamak için linke tıkla.</p></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n             <tr>\n              <td class=\"esdev-adapt-off\" align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:560px\">\n                 <tr>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr class=\"es-mobile-hidden\">\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:146px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:121px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:155px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\n                     <tr class=\"es-mobile-hidden\">\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:138px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n             <tr>\n              <td align=\"left\" style=\"padding:0;Margin:0;padding-right:20px;padding-bottom:10px;padding-left:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:5px\" role=\"presentation\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><span class=\"es-button-border\" style=\"border-style:solid;border-color:#2CB543;background:#5C68E2;border-width:0px;display:inline-block;border-radius:6px;width:auto\"><a href=\"https://techarsiv.com{url}\" class=\"es-button\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none !important;mso-line-height-rule:exactly;color:#FFFFFF;font-size:20px;padding:10px 30px 10px 30px;display:inline-block;background:#5C68E2;border-radius:6px;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:24px;width:auto;text-align:center;letter-spacing:0;mso-padding-alt:0;mso-border-alt:10px solid #5C68E2;border-left-width:30px;border-right-width:30px\">Sıfırla</a></span></td>\n                     </tr>\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;padding-bottom:10px;padding-top:20px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">Sorun mu var ? Mail gönder &nbsp;<a target=\"_blank\" href=\"https://techarsiv.com/\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\">support@gargamelinburnu.com</a></p><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\"><br>Teşekkürler,</p><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">gargamelinburnu.com</p></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-footer-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:640px\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"Margin:0;padding-right:20px;padding-left:20px;padding-bottom:20px;padding-top:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"left\" style=\"padding:0;Margin:0;width:600px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;padding-top:15px;padding-bottom:15px;font-size:0\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-table-not-adapt es-social\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><a href=\"\"><img title=\"Facebook\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/facebook-logo-black.png\" alt=\"Fb\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><img title=\"X.com\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/x-logo-black.png\" alt=\"X\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><img title=\"Instagram\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/instagram-logo-black.png\" alt=\"Inst\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0\"><img title=\"Youtube\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/youtube-logo-black.png\" alt=\"Yt\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td class=\"es-info-area\" align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px\" bgcolor=\"#FFFFFF\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table></td>\n     </tr>\n   </table>\n  </div>\n </body>\n</html>");

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
    [ValidateAntiForgeryToken]
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

    public IActionResult AccessDenied()
    {
        return View();
    }
}