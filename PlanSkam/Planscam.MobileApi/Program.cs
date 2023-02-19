using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Planscam.DataAccess;
using Planscam.Entities;
using Planscam.FsServices;

namespace Planscam.MobileApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.ClaimsIssuer = JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthorization();
        services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"), action =>
                    action.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
                options.UseOpenIddict();
            })
            .AddIdentity<User, IdentityRole>(options => options.User.RequireUniqueEmail = true)
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
        });
        services.AddOpenIddict()
            .AddCore(options =>
                options.UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>())
            .AddServer(options =>
            {
                options
                    .AcceptAnonymousClients()
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow();
                options.SetTokenEndpointUris("/Auth/Login");
                var cfg = options.UseAspNetCore();
                if (builder.Environment.IsDevelopment())
                    cfg.DisableTransportSecurityRequirement();
                cfg.EnableTokenEndpointPassthrough();
                options
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();
            })
            .AddValidation(options =>
            {
                options.UseAspNetCore();
                options.UseLocalServer();
            });
        services.AddSingleton<UsersRepo>();
        services.AddScoped<PlaylistsRepo>();
        services.AddScoped<AuthorsRepo>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.UseSwagger().UseSwaggerUI();
        app.UseHttpsRedirection()
            .UseAuthentication()
            .UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
