using Entities.Models;

namespace Services.Contracts;

public interface IAwardUserService
{
    IQueryable<AwardUser> AwardUsers { get; }
    Task GiveAward(AwardUser model);
}