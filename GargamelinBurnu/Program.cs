using GargamelinBurnu.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureCookie();
builder.Services.ConfigureEmailSender(builder.Configuration);
builder.Services.ConfigureRepositoryRegistration();
builder.Services.ConfigureServicesRegistration();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.ConfigureDefaultAdminUser();

app.Run();