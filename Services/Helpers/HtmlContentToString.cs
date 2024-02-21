using Microsoft.AspNetCore.Html;

namespace Services.Helpers;

public static class HtmlContentToString
{
    public static string ConvertHtmlContentToString(IHtmlContent htmlContent)
    {
        using (var writer = new StringWriter())
        {
            htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}