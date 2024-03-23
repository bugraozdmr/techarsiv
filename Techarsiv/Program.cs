

//using GargamelinBurnu.Hubs;
using GargamelinBurnu.Infrastructure.Extensions;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureCookie();
builder.Services.ConfigureEmailSender(builder.Configuration);
builder.Services.ConfigureRepositoryRegistration();
builder.Services.ConfigureServicesRegistration();
//builder.Services.AddSignalR();

// not sure ? -- sunucu cookie
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
});

// automapper -- sunucu cookie
builder.Services.AddAutoMapper(typeof(Program));

// not sure ?
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory())).SetDefaultKeyLifetime(TimeSpan.FromDays(7));


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

// 404 sayfaları
app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name:"Admin",
        areaName:"Admin",
        pattern:"Admin/{controller=Dashboard}/{action=Index}/{id?}"
    );
    // !?
    endpoints.MapAreaControllerRoute(
        name:"Main",
        areaName:"Main",
        pattern:"{controller=Home}/{action=Index}/{id?}"
    );
    
    endpoints.MapControllerRoute(
        name: "sitemap",
        pattern: "sitemap.xml",
        defaults: new { controller = "Home", action = "Sitemap" });

    endpoints.MapControllerRoute(
        name: "robots",
        pattern: "robots.txt",
        defaults: new { controller = "Home", action = "robots" });

    
    endpoints.MapControllerRoute("default","{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute("details", "{controller}");

    endpoints.MapRazorPages();

});




//app.UseCookiePolicy();
app.ConfigureDefaultRoles();
app.ConfigureDefaultAdminUser();
//app.MapHub<UsersOnlineHub>("/UsersOnlineHub");

app.Run();