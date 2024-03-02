using Entities.Models;
using Repositories.EF;
using Services.Contracts;

namespace Services;

public class AwardManager : IAwardService
{
    private readonly RepositoryContext _context;

    public AwardManager(RepositoryContext context)
    {
        _context = context;
    }

    public IQueryable<Awards> Awards => _context.Awards.Select(s => new Awards()
    {
        Title = s.Title,
        AwardsId = s.AwardsId,
        point = s.point
    });
}