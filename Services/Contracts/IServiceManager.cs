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
    INotificationService NotificationService { get; }
    IFollowingSubjects FollowingSubjects { get; }
    
    IAwardUserService AwardUserService { get; }
    IReportService ReportService { get; }
    IArticleService ArticleService { get; }
    
}