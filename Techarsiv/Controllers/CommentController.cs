using System.Text.RegularExpressions;
using Entities.Dtos.Comment;
using Entities.Dtos.Notification;
using Entities.Models;
using GargamelinBurnu.Infrastructure.Helpers.Contracts;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.EF;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class CommentController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;
    private readonly RepositoryContext _context;
    private readonly IEmailSender _emailSender;


    public CommentController(UserManager<User> userManager
        , IServiceManager manager, 
        RepositoryContext context, 
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _manager = manager;
        _context = context;
        _emailSender = emailSender;
    }

    [Authorize]
    public IActionResult LikeComment(int CommentId)
    {
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new { UserName = s.UserName, UserId = s.Id })
            .FirstOrDefault();

        
        var userId = user.UserId;
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }
        
        var searched = _manager.CommentLikeDService.CLikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
        if (searched is null)
        {
            var search1 = _manager.CommentLikeDService.CDislikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
            if (search1 is not null)
            {
                dislikeCommentRemove(CommentId, userId);
                
            }
            _manager.CommentLikeDService.Like(new Commentlike()
            {
                CommentId = CommentId,
                UserId = userId
            });
            
            return Json(new
            {
                success = 1,
                likecount = _manager
                    .CommentLikeDService
                    .CLikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count(),
                dislikecount = _manager
                    .CommentLikeDService
                    .CDislikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count()
            });
        }
        else
        {
            return likeCommentRemove(CommentId, userId);
        }
    }


    [Authorize]
    public IActionResult likeCommentRemove(int CommentId,string userId)
    {
        _manager.CommentLikeDService.LikeRemove(CommentId, userId);

        return Json(new
        {
            success = 2,
            likecount = _manager
                .CommentLikeDService
                .CLikes
                .Where(l => l.CommentId.Equals(CommentId)).Count(),
            dislikecount = _manager
                .CommentLikeDService
                .CDislikes
                .Where(l => l.CommentId.Equals(CommentId)).Count()
        });
    }

    [Authorize]
    public async Task<IActionResult> dislikeComment(int CommentId)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var userId = user.Id;
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }
        
        var searched = _manager.CommentLikeDService.CDislikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
        if (searched is null)
        {
            var search1 = _manager.CommentLikeDService.CLikes.FirstOrDefault(s => s.UserId.Equals(userId) && s.CommentId.Equals(CommentId));
            if (search1 is not null)
            {
                likeCommentRemove(CommentId, userId);
            }
            _manager.CommentLikeDService.disLike(new CommentDislike()
            {
                CommentId = CommentId,
                UserId = userId
            });
            
            return Json(new
            {
                success = 1,
                likecount = _manager
                    .CommentLikeDService
                    .CLikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count(),
                dislikecount = _manager
                    .CommentLikeDService
                    .CDislikes
                    .Where(l => l.CommentId.Equals(CommentId)).Count()
            });
        }
        else
        {
            return dislikeCommentRemove(CommentId, userId);
        }
    }

    [Authorize]
    public IActionResult dislikeCommentRemove(int CommentId,string userId)
    {
        _manager.CommentLikeDService.disLikeRemove(CommentId, userId);

        return Json(new
        {
            success = 2,
            likecount = _manager
                .CommentLikeDService
                .CLikes
                .Where(l => l.CommentId.Equals(CommentId)).Count(),
            dislikecount = _manager
                .CommentLikeDService
                .CDislikes
                .Where(l => l.CommentId.Equals(CommentId)).Count()
        });
    }
    
    
    [Authorize]
    public async Task<IActionResult> addComment(int SubjectId,string Text)
    {
        var userBan = _userManager
            .Users
            .Where(u => u.UserName.Equals(User.Identity.Name))
            .Select(u => u.BanUntill)
            .FirstOrDefault();
        
        if (userBan != null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        var user = _userManager
            .Users
            .Where(s => s.UserName.Equals(User.Identity.Name))
            .Select(s => new {  UserId = s.Id,
                username=s.UserName,
                userimage=s.Image })
            .FirstOrDefault();
        
        if (user is null)
        {
            return Json(new
            {
                success = -1
            });
        }

        string[] yasakliKelimeler = { "amına koyayım", "siktiğim", "sikik","dalyarak","dalyarrak","yarrak","piç","siktirgit","siktir"};
        string pattern = string.Join("|", yasakliKelimeler);

        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        MatchCollection matches = regex.Matches(Text.ToLower());

        if (matches.Count > 0)
        {
            return Json(new
            {
                success = -2
            });
        }
        
        CommentDetailsViewModel model = new CommentDetailsViewModel();
        
        model = await _userManager.Users
            .Where(u => u.UserName == User.Identity.Name)
            .Include(u => u.Comments)
            .Select(u => new  CommentDetailsViewModel()
            {
                Username = u.UserName,
                CreatedAt = u.CreatedAt,
                Count = u.Comments.Count,
                userSignature = u.signature,
                userImage = u.Image
            })
            .FirstOrDefaultAsync();
        
        var result = await _manager.CommentService.CreateComment(new CreateCommentDto()
        {
            SubjectId = SubjectId,
            Text = Text,
            UserId = user.UserId
        });

        if (result == 1)
        {
            // Notification islemleri
            // sadece ilgilenleri alcak onlara göndercek... baya yavaşlatcak
            var subjectnot = _context.FollowingSubjects.AsNoTracking()
                .Include(s => s.User)
                .Where(s => s.SubjectId.Equals(SubjectId))
                .ToList();

            // 3 tur kontrol
            var commentId = _manager
                .CommentService
                .getAllComments(false)
                .OrderBy(s => s.CreatedAt)
                .Where(s => s.Text.Equals(Text) &&
                            s.SubjectId.Equals(SubjectId) && s.UserId.Equals(user.UserId))
                .Select(s => s.CommentId)
                .FirstOrDefault();
            
            
            // subject name
            var subjectneed = _manager
                .SubjectService
                .GetAllSubjects(false)
                .Where(s => s.SubjectId.Equals(SubjectId))
                .Select(s => new
                {
                    title = s.Title,
                    subjecturl = s.Url        
                })
                .FirstOrDefault();
            
            
            
            if (subjectnot != null)
            {
                foreach (var item in subjectnot)
                {
                    if (!(item.UserId.Equals(user.UserId)))
                    {
                        var count = _manager
                            .NotificationService
                            .GetAllNotification(false)
                            .Include(s => s.User)
                            .Count(s => s.User.Id.Equals(item.UserId) && s.read.Equals(false));
                        
                        
                        if (count <= 30)
                        {
                            NotificationDto dto = new NotificationDto()
                            {
                                UserId = item.UserId,
                                SubjectId = SubjectId,
                                CommentId = commentId
                            };

                        
                            _manager.NotificationService.CreateNotification(dto);    
                        }
                    }
                }
                
                // commentCount update
                var userup = await _userManager.FindByNameAsync(User.Identity.Name);
                userup.commentCount = _manager
                    .CommentService
                    .getAllComments(false)
                    .Count(s => s.UserId.Equals(userup.Id));
                _context.Update(userup);
                
                // burda olmazsa tüm hepsi için kayıt almıyor atlıyor /*/*/
                await _context.SaveChangesAsync();

                
                
                // mail sender

                // include calismiyor
                var users = _manager
                    .FollowingSubjects
                    .FSubjects
                    .Where(s => s.SubjectId.Equals(SubjectId))
                    .Select(s => s.UserId)
                    .ToList();

                if (users is not null)
                {
                    List<string> emails = new List<string>();
                    
                    // şuanlik sinir
                    int count = 0;
                    
                    foreach (var item in users)
                    {
                        var email = _userManager
                            .Users
                            .Where(s => s.Id.Equals(item))
                            .FirstOrDefault();

                        if (email.emailActive.Equals(true) && 
                            email.canTakeEmail.Equals(true))
                        {
                            if (count < 10)
                            {
                                email.canTakeEmail = false;
                                _context.Update(email);
                                
                                emails.Add(email.Email);
                                count++;
                            }

                            if (count >= 10)
                            {
                                break;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    
                    if (emails != null && emails.Count != 0)
                    {
                        await _emailSender.SendMultipleEmailsAsync(emails, "Yeni mesajınız var",
                            $"<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"en\">\n <head>\n  <meta charset=\"UTF-8\">\n  <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\n  <meta name=\"x-apple-disable-message-reformatting\">\n  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n  <meta content=\"telephone=no\" name=\"format-detection\">\n    <style type=\"text/css\">\n    a {{text-decoration: none;}}\n    </style>\n  <style type=\"text/css\">\n.rollover:hover .rollover-first {{\n  max-height:0px!important;\n  display:none!important;\n  }}\n  .rollover:hover .rollover-second {{\n  max-height:none!important;\n  display:block!important;\n  }}\n  .rollover span {{\n  font-size:0px;\n  }}\n  u + .body img ~ div div {{\n  display:none;\n  }}\n  #outlook a {{\n  padding:0;\n  }}\n  span.MsoHyperlink,\nspan.MsoHyperlinkFollowed {{\n  color:inherit;\n  mso-style-priority:99;\n  }}\n  a.es-button {{\n  mso-style-priority:100!important;\n  text-decoration:none!important;\n  }}\n  a[x-apple-data-detectors] {{\n  color:inherit!important;\n  text-decoration:none!important;\n  font-size:inherit!important;\n  font-family:inherit!important;\n  font-weight:inherit!important;\n  line-height:inherit!important;\n  }}\n  .es-desk-hidden {{\n  display:none;\n  float:left;\n  overflow:hidden;\n  width:0;\n  max-height:0;\n  line-height:0;\n  mso-hide:all;\n  }}\n  .es-button-border:hover > a.es-button {{\n  color:#ffffff!important;\n  }}\n@media only screen and (max-width:600px) {{.es-m-p0r {{ padding-right:0px!important }} *[class=\"gmail-fix\"] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} h1 {{ font-size:36px!important; text-align:left }} h2 {{ font-size:26px!important; text-align:left }} h3 {{ font-size:20px!important; text-align:left }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }} .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:36px!important }} .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:26px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:12px!important }} .es-header-body p, .es-header-body a {{ font-size:14px!important }} .es-content-body p, .es-content-body a {{ font-size:14px!important }} .es-footer-body p, .es-footer-body a {{ font-size:14px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }} .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }} .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover span, .es-m-txt-c .rollover span, .es-m-txt-l .rollover span {{ line-height:0!important; font-size:0!important }} .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:20px!important; line-height:120%!important }} a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }} table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }} table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .es-social td {{ padding-bottom:10px }} .h-auto {{ height:auto!important }} }}\n@media screen and (max-width:384px) {{.mail-message-content {{ width:414px!important }} }}\n</style>\n </head>\n <body class=\"body\" style=\"width:100%;height:100%;padding:0;Margin:0\">\n  <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"en\" style=\"background-color:#FAFAFA\">\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table bgcolor=\"#ffffff\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\n             <tr>\n              <td align=\"left\" style=\"padding:0;Margin:0;padding-right:20px;padding-left:20px;padding-top:15px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;padding-top:15px;padding-bottom:15px\"><h1 style=\"Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:46px;font-style:normal;font-weight:bold;line-height:55px;color:#333333\">Mesajınız var</h1></td>\n                     </tr>\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:24px;letter-spacing:0;color:#333333;font-size:16px\">Takip ettiğin <strong><a href=\"https://techarsiv.com/{subjectneed.subjecturl}\" style=\"text-decoration: none;color: darkorange;\">{subjectneed.title}</a></strong> isimli konuya mesaj yazıldı.</p></td>\n                     </tr>\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;padding-top:10px;padding-bottom:10px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:24px;letter-spacing:0;color:#333333;font-size:16px\">{user.username}</p>\n                      <p>{Text}</p></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n             <tr>\n              <td class=\"esdev-adapt-off\" align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:560px\">\n                 <tr>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr class=\"es-mobile-hidden\">\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:146px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:121px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-left\" align=\"left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:155px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                  <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\n                     <tr class=\"es-mobile-hidden\">\n                      <td align=\"left\" style=\"padding:0;Margin:0;width:138px\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n             <tr>\n              <td align=\"left\" style=\"padding:0;Margin:0;padding-right:20px;padding-bottom:10px;padding-left:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:5px\" role=\"presentation\">\n                     <tr>\n                      <td align=\"left\" style=\"padding:0;Margin:0;padding-bottom:10px;padding-top:20px\"><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">Sorun mu var ? Mail gönder &nbsp;<a target=\"_blank\" href=\"https://gargamelinburnu.com/\" style=\"mso-line-height-rule:exactly;text-decoration:underline;color:#5C68E2;font-size:14px\">support@gargamelinburnu.com</a></p><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\"><br>Bilgilendirme</p><p style=\"Margin:0;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;letter-spacing:0;color:#333333;font-size:14px\">herhangibir konuyu okuyana kadar daha mesaj almayacaksın.<br><br>Mesaj iptali için profiline git.</p></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top\">\n         <tr>\n          <td align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-footer-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:640px\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"Margin:0;padding-right:20px;padding-left:20px;padding-bottom:20px;padding-top:20px\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"left\" style=\"padding:0;Margin:0;width:600px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;padding-top:15px;padding-bottom:15px;font-size:0\">\n                       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-table-not-adapt es-social\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                         <tr>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><a href=\"\"><img title=\"Facebook\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/facebook-logo-black.png\" alt=\"Fb\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></a></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><img title=\"X.com\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/x-logo-black.png\" alt=\"X\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;padding-right:40px\"><img title=\"Instagram\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/instagram-logo-black.png\" alt=\"Inst\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                          <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0\"><img title=\"Youtube\" src=\"https://ffutabc.stripocdn.email/content/assets/img/social-icons/logo-black/youtube-logo-black.png\" alt=\"Yt\" width=\"32\" style=\"display:block;font-size:14px;border:0;outline:none;text-decoration:none\"></td>\n                         </tr>\n                       </table></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table>\n       <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important\">\n         <tr>\n          <td class=\"es-info-area\" align=\"center\" style=\"padding:0;Margin:0\">\n           <table class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px\" bgcolor=\"#FFFFFF\" role=\"none\">\n             <tr>\n              <td align=\"left\" style=\"padding:20px;Margin:0\">\n               <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                 <tr>\n                  <td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:560px\">\n                   <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\n                     <tr>\n                      <td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td>\n                     </tr>\n                   </table></td>\n                 </tr>\n               </table></td>\n             </tr>\n           </table></td>\n         </tr>\n       </table></td>\n     </tr>\n   </table>\n  </div>\n </body>\n</html>" 
                            );    
                    }
                }
                 
                 
                // end of sending  
            }
            
            
            
            return Json(new
            {
                success = 1,
                text = Text,
                createdAt = model.CreatedAt,
                username = model.Username,
                messageCount = model.Count,
                userSignature = model.userSignature,
                userImage = model.userImage
            });    
        }
        else
        {
            return Json(new
            {
                success = -1
            });
        }
        
    }
}