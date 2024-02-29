using Entities.RequestParameters;

namespace GargamelinBurnu.Models;

public class IndexPageViewModel
{
    public string section { get; set; }
    public DateTime banuntill { get; set; }
    public SubjectRequestParameters p { get; set; }
}