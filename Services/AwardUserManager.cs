using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories.EF;
using Services.Contracts;

namespace Services;

public class AwardUserManager : IAwardUserService
{
    private readonly RepositoryContext _context;
    private readonly UserManager<User> _userManager;

    public AwardUserManager(RepositoryContext context, 
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    
    public IQueryable<AwardUser> AwardUsers => _context.AwardUsers.Select(s => new AwardUser()
    {
        CreatedAt = s.CreatedAt,
        UserId = s.UserId,
        AwardsId = s.AwardsId
    });
    
    public async Task GiveAward(AwardUser model)
    {
        if (_context.AwardUsers.FirstOrDefault(l => l.UserId == model.UserId && l.AwardsId == model.AwardsId) is not null)
        {
            // bos
        }
        else
        {
            _context.AwardUsers.Add(model);
            var user = await _userManager.FindByIdAsync(model.UserId);

            int point = _context
                .Awards
                .Where(s => s.AwardsId.Equals(model.AwardsId))
                .Select(s => s.point)
                .FirstOrDefault();

            user.Points = user.Points + point;
            
            
            _context.SaveChanges();
        }
    }
}