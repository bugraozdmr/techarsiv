using Entities.Models;
using GargamelinBurnu.Infrastructure.Helpers;
using GargamelinBurnu.Infrastructure.Helpers.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories.EF;

namespace GargamelinBurnu.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("sqlConnection");

        services.AddDbContextPool<RepositoryContext>(opt =>
        {
            opt.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 4, 28)));
            opt.EnableSensitiveDataLogging(true);
        });
    }
    
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
             
                options.SignIn.RequireConfirmedAccount = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureCookie(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "gb_cookie";
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        });
    }
    
    public static void ConfigureRepositoryRegistration(this IServiceCollection services)
    {
        
    }

    public static void ConfigureEmailSender(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, SmtpEmailSender>(i => new SmtpEmailSender(
            configuration["EmailSender:Host"],
            configuration.GetValue<int>("EmailSender:Port"),
            configuration.GetValue<bool>("EmailSender:EnableSSL"),
            configuration["EmailSender:Username"],
            configuration["EmailSender:Password"]
        ));
    }
}