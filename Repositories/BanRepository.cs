using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;

namespace Repositories;

public class BanRepository :  RepositoryBase<Ban>,IBanRepository
{
    public BanRepository(RepositoryContext context) : base(context)
    {
    }

    public IQueryable<Ban> GetAllBans(bool trackchanges) => FindAll(false);

    public void CreateBan(Ban ban) =>  Create(ban);
}