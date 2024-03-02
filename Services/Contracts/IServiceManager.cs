namespace Services.Contracts;

public interface IServiceManager
{
    ISubjectService SubjectService { get; }
    ICategoryService CategoryService { get; }
    ICommentService CommentService { get; }
    ILikeDService LikeDService { get; }
    ICommentLikeDService CommentLikeDService { get; }
    IBanService BanService { get; }
    IAwardService AwardService { get; }
    
    IAwardUserService AwardUserService { get; }
    
}