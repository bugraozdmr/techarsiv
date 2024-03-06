namespace Repositories.Contracts;

public interface IRepositoryManager
{
    ISubjectRepository Subject { get; }
    ICategoryRepository Categories { get; }
    ICommentRepository Comments { get; }
    IBanRepository Bans { get; }
    INotificationRepository Notification { get; }
    IReportRepository ReportRepository { get; }

    Task SaveAsync();
}