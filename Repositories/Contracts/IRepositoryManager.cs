namespace Repositories.Contracts;

public interface IRepositoryManager
{
    ISubjectRepository Subject { get; }
    ICategoryRepository Categories { get; }
    ICommentRepository Comments { get; }
    IBanRepository Bans { get; }

    Task SaveAsync();
}