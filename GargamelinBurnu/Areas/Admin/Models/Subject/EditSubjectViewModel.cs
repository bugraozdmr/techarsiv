namespace GargamelinBurnu.Areas.Admin.Models.Subject;

public class EditSubjectViewModel
{
    public int SubjectId { get; set; }
    public int CategoryId { get; set; }
    public string SubjectOwner { get; set; }
    public string CategoryName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
}