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
                UserName = adminUser
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            
            if (!result.Succeeded)
                throw new Exception("Admin could not created.");
            
            var roleResult = await userManager.AddToRolesAsync(user,
                roleManager
                    .Roles
                    .Select(r => r.Name)
                    .ToList());

            if (!roleResult.Succeeded)
                throw new Exception("System have problems with role defination for admin.");
        }
    } 
}