namespace Services.Contracts;

public interface IServiceManager
{
    ISubjectService SubjectService { get; }
    ICategoryService CategoryService { get; }
    ICommentService CommentService { get; }
    ILikeDService LikeDService { get; }
}