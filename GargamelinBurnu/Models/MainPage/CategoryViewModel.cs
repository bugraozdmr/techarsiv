using Entities.RequestParameters;
using GargamelinBurnu.Infrastructure;

namespace GargamelinBurnu.Models;

public class CategoryViewModel
{
    public string CategoryName { get; set; }
    public string CategoryUrl { get; set; }
    public string CategoryDesc { get; set; }
    public SubjectRequestParameters p { get; set; }
}