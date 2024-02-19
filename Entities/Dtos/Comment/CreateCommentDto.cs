using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Comment;

public class CreateCommentDto
{
    // şu anlık böyle req kaldırılabilir
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Yorum eklenmeli")]
    public string? Text { get; set; }
    
    public int? SubjectId { get; set; }
    
    public string? UserId { get; set; }
}