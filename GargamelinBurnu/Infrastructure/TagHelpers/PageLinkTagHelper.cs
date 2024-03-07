using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GargamelinBurnu.Infrastructure.TagHelpers;

[HtmlTargetElement("div",Attributes = "page-model")]
public class PageLinkTagHelper : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;
    
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }
    
    public Pagination PageModel { get; set; }
    public string? PageAction { get; set; }
    public string? goingTo { get; set; }
    public string? param { get; set; }
    public string? area { get; set; }

    // isimlendirme değer alacaksa page-class diyorsak burdaki prop pageClass olmalı
    public bool PageClassesEnabled { get; set; }
    public string PageClass { get; set; } = String.Empty;
    public string PageClassNormal { get; set; } = String.Empty;
    public string PageClassSelected { get; set; } = String.Empty;
    
    
    public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
    {
        _urlHelperFactory = urlHelperFactory;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext is not null && PageModel is not null)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            
            TagBuilder result = new TagBuilder("div");

            
            var flag = 0;   // tekrarı engellicek
            
            for (int i = (PageModel.CurrentPage >= 5 ? (PageModel.CurrentPage - 2) : 1); i <= PageModel.TotalPage; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                if (goingTo is not null && goingTo != "")
                {
                    // details for every each subject with this if condition
                    if (PageAction is null)
                    {
                        string url = $"/{goingTo}?PageNumber={i}";
                        tag.Attributes["href"] = url;
                    }
                    else
                    {
                        string url = $"/{PageAction}/{goingTo}?PageNumber={i}";
                        tag.Attributes["href"] = url;
                    }
                    
                    if (PageModel.CurrentPage >= 5 && flag != 1)
                    {
                        TagBuilder tag_first = new TagBuilder("a");
                        tag_first.Attributes["href"] = $"/{PageAction}/{goingTo}?PageNumber=1";
                        tag_first.AddCssClass(PageClass);
                        tag_first.AddCssClass(PageClassNormal);
                        tag_first.InnerHtml.Append("1");
                        result.InnerHtml.AppendHtml(tag_first);

                        // [...]
                        TagBuilder tag1 = new TagBuilder("a");
                        tag1.AddCssClass(PageClass);
                        tag1.AddCssClass(PageClassNormal);
                        tag1.InnerHtml.Append("...");
                        result.InnerHtml.AppendHtml(tag1);
                        
                        flag = 1;
                    }
                }
                else
                {
                    if (param is not null && param != "")
                    {
                        string url;
                        if (area == null || area == "")
                        {
                             url = $"https://localhost:7056/{PageAction}?PageNumber={i}&{param}";    
                        }
                        else
                        {
                            // başka param gelirse o da if ile eklenir
                            url = $"https://localhost:7056{area}/{PageAction}?PageNumber={i}&{param}";
                        }
                        tag.Attributes["href"] = url;
                    }
                    else
                    {
                        tag.Attributes["href"] = urlHelper.Action(PageAction,new {PageNumber = i});
                    }
                    
                    if (PageModel.CurrentPage >= 5 && flag != 1)
                    {
                        TagBuilder tag_first = new TagBuilder("a");
                        tag_first.Attributes["href"] = urlHelper.Action(PageAction,new {PageNumber = 1});;
                        tag_first.AddCssClass(PageClass);
                        tag_first.AddCssClass(PageClassNormal);
                        tag_first.InnerHtml.Append("1");
                        result.InnerHtml.AppendHtml(tag_first);
                        
                        // [...]
                        TagBuilder tag1 = new TagBuilder("a");
                        tag1.AddCssClass(PageClass);
                        tag1.AddCssClass(PageClassNormal);
                        tag1.InnerHtml.Append("...");
                        result.InnerHtml.AppendHtml(tag1);
                        
                        flag = 1;
                    }

                }

                
                
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);

                if (i == PageModel.CurrentPage + 2 && i != PageModel.TotalPage)
                {
                    TagBuilder tag1 = new TagBuilder("a");
                    tag1.AddCssClass(PageClass);
                    tag1.AddCssClass(PageClassNormal);
                    tag1.InnerHtml.Append("...");
                    result.InnerHtml.AppendHtml(tag1);

                    if (!(i > PageModel.TotalPage-3))
                    {
                        i = PageModel.TotalPage - 3;    
                    }
                    
                }
            }

            
            
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}