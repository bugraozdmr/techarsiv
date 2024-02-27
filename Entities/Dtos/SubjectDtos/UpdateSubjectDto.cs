namespace Entities.Dtos.SubjectDtos;

public class UpdateSubjectDto
{
    public int SubjectId { get; set; }
    public bool IsActive { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string? Prefix { get; set; }
}