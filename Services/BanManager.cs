using AutoMapper;
using Entities.Dtos.Ban;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Repositories.Contracts;
using Repositories.EF;
using Services.Contracts;

namespace Services;

public class BanManager : IBanService
{
    private readonly IRepositoryManager _manager;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RepositoryContext _context;

    public BanManager(IRepositoryManager manager, 
        IMapper mapper, 
        UserManager<User> userManager,
        RepositoryContext context)
    {
        _manager = manager;
        _mapper = mapper;
        _userManager = userManager;
        _context = context;
    }

    public async Task<int> CreateBan(BanCauseDto dto)
    {
        var userid = _userManager
            .Users
            .Where(u => u.UserName.Equals(dto.username))
            .Select(u => u.Id)
            .FirstOrDefault();

        if (userid is null)
        {
            return -1;
        }
        
        var ban = _mapper.Map<Ban>(dto);
        ban.CreatedAt = DateTime.Now;
        ban.UserId = userid;
        
        _manager.Bans.CreateBan(ban);

        var user = await _userManager.FindByIdAsync(userid);

        if (user is null)
        {
            return -1;
        }
        
        // user banUntill update
        if (ban.Cause.Equals("swear"))
        {
            user.BanUntill = DateTime.Now.AddHours(6);
        }
        else if (ban.Cause.Equals("wrongSubject"))
        {
            user.BanUntill = DateTime.Now.AddHours(1);
        }
        else if (ban.Cause.Equals("inappropriatePhoto"))
        {
            // kötü fotoğraf
            user.BanUntill = DateTime.Now.AddDays(1);
            user.Image = "/images/user/samples/avatar_7.jpg";
        }
        else if (ban.Cause.Equals("inappropriateUsername"))
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 1001);

            var username = "user" + randomNumber;

            var checkUser = _userManager.FindByNameAsync(username);
            
            user.UserName = username;
            user.NormalizedUserName = username.ToUpper();
            
            user.BanUntill = DateTime.Now.AddDays(1);
        }
        else if (ban.Cause.Equals("anotherLang"))
        {
            user.BanUntill = DateTime.Now.AddHours(1);
        }
        else
        {   // olurda biri değişirse giden val'ı
            user.BanUntill = DateTime.Now.AddHours(1);
        }

        
        // atılamama ihtimali var ama yok gibi davrancam
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public IQueryable<Ban> getAllBans(bool trackChanges) => _manager.Bans.GetAllBans(false);

    public async Task checkBan(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is not null)
        {
            if (user.BanUntill != null && user.BanUntill < DateTime.Now)
            {
                user.BanUntill = null;
                // awaiti unutma amk
                await _manager.SaveAsync();
            }
        }
    }
}