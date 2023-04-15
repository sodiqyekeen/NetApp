global using NetApp.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetApp.Application.Common;
using NetApp.Application.Interfaces.Identity;
using NetApp.Application.Interfaces;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Common;
using NetApp.Infrastructure.Contexts;
using NetApp.Infrastructure.Identity.Models;
using NetApp.Infrastructure.Identity.Services;
using System.Reflection;

namespace NetApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NetAppDbContext>(x => x.UseSqlite(configuration.GetConnectionString("RevliogDb"), option => option.MigrationsAssembly(typeof(RevliogDbContext).Assembly.FullName)));
        services.AddScoped<INetAppDbContext>(provider => provider.GetService<NetAppDbContext>()!);
        services.AddIdentity<NetAppUser, NetAppRole>()
           .AddEntityFrameworkStores<NetAppDbContext>()
           .AddDefaultTokenProviders();


        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IRoleClaimService, RoleClaimService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRepositoryProvider, RepositoryProvider>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        return services;
    }
}
