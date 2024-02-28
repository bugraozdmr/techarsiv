using GargamelinBurnu.Hubs;
using GargamelinBurnu.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureCookie();
builder.Services.ConfigureEmailSender(builder.Configuration);
builder.Services.ConfigureRepositoryRegistration();
builder.Services.ConfigureServicesRegistration();
builder.Services.AddSignalR();

// automapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

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
    
    endpoints.MapControllerRoute("default","{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute("details", "{controller}");

    endpoints.MapRazorPages();

});

app.ConfigureDefaultRoles();
app.ConfigureDefaultAdminUser();
app.MapHub<UsersOnlineHub>("/UsersOnlineHub");

app.Run();