using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;
using Planscam.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"), action =>
        action.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    options.UseOpenIddict();
});
services.AddIdentity<User, IdentityRole>(options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<AppDbContext>();
services.AddSingleton<UsersRepo>();
services.AddScoped<PlaylistsRepo>();
services.AddScoped<AuthorsRepo>();

services.AddCors();
services.AddControllersWithViews();

services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    }).AddCookie(options => { options.LoginPath = "/account/google-login"; })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = "842296346308-nn6m0csbh0ldm66o9tei07ae9elcjkr4.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });

services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Auth/Login");
    options.AccessDeniedPath = new PathString("/Auth/AccessDenied"); //TODO
});

var app = builder.Build();

app.UseMiddleware<SubscriptionsMiddleware>();

#region migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
#endregion

(app.Environment.IsDevelopment() ? app : app.UseExceptionHandler("/Home/Error").UseHsts())
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin())
    .UseAuthentication()
    .UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
