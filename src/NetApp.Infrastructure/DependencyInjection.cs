global using Microsoft.Extensions.Logging;
global using NetApp.Application.Services;
global using NetApp.Constants;
global using NetApp.Domain.Constants;
global using NetApp.Dtos;
global using NetApp.Extensions;
global using NetApp.Infrastructure.Identity.Models;
global using NetApp.Infrastructure.Services;
global using NetApp.Models;
global using NetApp.Shared.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetApp.Domain.Models;
using NetApp.Infrastructure.Common;
using NetApp.Infrastructure.Contexts;
using NetApp.Infrastructure.Identity.Services;
using NetApp.Infrastructure.Security;
using System.Reflection;

namespace NetApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NetAppDbContext>(x => x.UseSqlite(configuration.GetConnectionString("NetAppDb"), option => option.MigrationsAssembly(typeof(NetAppDbContext).Assembly.FullName)));
        //services.AddScoped<NetAppDbContext>();
        services.AddIdentity<NetAppUser, NetAppRole>()
           .AddEntityFrameworkStores<NetAppDbContext>()
           .AddDefaultTokenProviders()
           .AddApiEndpoints();

        services.AddScoped<IDatabaseSeeder, ApplicationDataSeeder>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRepositoryProvider, RepositoryProvider>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        services.AddSignalR();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = NetAppAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = NetAppAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = NetAppAuthenticationDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = NetAppAuthenticationDefaults.AuthenticationScheme;
        })
        .AddScheme<NetAppAuthenticationOptions, NetAppAuthenticationHandler>(NetAppAuthenticationDefaults.AuthenticationScheme, null);

        services.AddAuthorization(options =>
        {
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString()!, policy => policy.RequireClaim(SharedConstants.CustomClaimTypes.Permission, propertyValue.ToString()!));
                }
            }
        });

        return services;
    }
    public static void SeedDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        var seederService = serviceScope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        seederService.Initialize();
    }

}
