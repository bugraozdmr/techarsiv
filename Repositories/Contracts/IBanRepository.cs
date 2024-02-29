using Entities.Models;

namespace Repositories.Contracts;

public interface IBanRepository : IRepositoryBase<Ban>
{
    IQueryable<Ban> GetAllBans(bool trackchanges);
    void CreateBan(Ban ban);
}