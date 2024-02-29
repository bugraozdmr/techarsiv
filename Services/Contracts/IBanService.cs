using Entities.Dtos.Ban;
using Entities.Models;

namespace Services.Contracts;

public interface IBanService
{
    Task<int> CreateBan(BanCauseDto dto);
    Task checkBan(string username);
    IQueryable<Ban> getAllBans(bool trackChanges);
}