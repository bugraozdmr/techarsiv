using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace GargamelinBurnu.Infrastructure.Extensions;

public static class ApplicationExtensions
{
    public static async void ConfigureDefaultAdminUser(this IApplicationBuilder app)
    {
        const string adminUser = "Admin";
        const string adminPassword = "bugra123";
        
        // User Manager
        UserManager<User> userManager = app
            .ApplicationServices
            .CreateScope()
            .ServiceProvider
            .GetService<UserManager<User>>();
        
        
        RoleManager<IdentityRole> roleManager = app
            .ApplicationServices
            .CreateAsyncScope()
            .ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        User user = await userManager.FindByNameAsync(adminUser);
        
        if (user is null)
        {
            user = new User()
            {
                FullName = "Grant Wick",
                Email = "bugra.ozdemir@gmail.com",
                UserName = adminUser,
                CreatedAt = DateTime.Now,
                EmailConfirmed = true,
                Image = "/images/user/samples/avatar_4.jpg"
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            
            if (!result.Succeeded)
                throw new Exception("Admin could not created.");
            
            // bozuk bu
            var roleResult = await userManager.AddToRolesAsync(user,
                roleManager
                    .Roles
                    .Select(r => r.Name)
                    .ToList());

            if (!roleResult.Succeeded)
                throw new Exception("System have problems with role definition for admin.");
        }
    }
    
    public static async void ConfigureDefaultRoles(this IApplicationBuilder app)
    {
        // User Manager
        RoleManager<IdentityRole> roleManager = app
            .ApplicationServices
            .CreateScope()
            .ServiceProvider
            .GetService<RoleManager<IdentityRole>>();
        
        var roles = new List<IdentityRole>
        {
            new IdentityRole() { Name = "User", NormalizedName = "USER" },
            new IdentityRole() { Name = "Moderator", NormalizedName = "MODERATOR" },
            new IdentityRole() { Name = "Admin", NormalizedName = "ADMIN" }
        };
        
        foreach (var role in roles)
        {
            // Role zaten varsa eklemeyi dene
            var existingRole = await roleManager.FindByNameAsync(role.Name);
            if (existingRole == null)
            {
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("System have problems with role definition.");
                }
            }
        }
    }
}