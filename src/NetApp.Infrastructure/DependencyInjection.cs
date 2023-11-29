global using NetApp.Extensions;
global using Microsoft.Extensions.Logging;
global using NetApp.Models;
global using NetApp.Dtos;
global using NetApp.Constants;
global using NetApp.Application.Services;
global using NetApp.Infrastructure.Services;
global using NetApp.Shared.Constants;
global using NetApp.Domain.Constants;
global using NetApp.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetApp.Domain.Repositories;
using NetApp.Infrastructure.Common;
using NetApp.Infrastructure.Contexts;
using NetApp.Infrastructure.Identity.Services;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using NetApp.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace NetApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NetAppDbContext>(x => x.UseSqlite(configuration.GetConnectionString("NetAppDb"), option => option.MigrationsAssembly(typeof(NetAppDbContext).Assembly.FullName)));
        services.AddScoped<INetAppDbContext>(provider => provider.GetService<NetAppDbContext>()!);
        services.AddIdentity<NetAppUser, NetAppRole>()
           .AddEntityFrameworkStores<NetAppDbContext>()
           .AddDefaultTokenProviders()
           .AddApiEndpoints();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IDatabaseSeeder, ApplicationDataSeeder>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRepositoryProvider, RepositoryProvider>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.AddAuthentication(options =>
                   {
                       options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                   })
                       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
                       {
                           jwtOptions.RequireHttpsMetadata = false;
                           jwtOptions.SaveToken = false;
                           jwtOptions.TokenValidationParameters = new TokenValidationParameters
                           {
                               ValidateIssuerSigningKey = true,
                               ValidateIssuer = true,
                               ValidateAudience = true,
                               ClockSkew = TimeSpan.Zero,
                               ValidIssuer = configuration["JwtSettings:Issuer"],
                               ValidAudience = configuration["JwtSettings:Audience"],
                               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                           };

                           jwtOptions.Events = new JwtBearerEvents()
                           {
                               OnMessageReceived = context =>
                               {
                                   var accessToken = context.Request.Query["access_token"];
                                   if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments(SharedConstants.SignalR.HubUrl))
                                       context.Token = accessToken;

                                   return Task.CompletedTask;
                               },

                               OnChallenge = context =>
                               {
                                   context.HandleResponse();
                                   context.Response.StatusCode = 401;
                                   context.Response.ContentType = "application/json";
                                   return context.Response.WriteAsJsonAsync(Response.Fail(context.Error!));
                               },

                               OnForbidden = context =>
                               {
                                   context.Response.StatusCode = 403;
                                   context.Response.ContentType = "application/json";
                                   return context.Response.WriteAsJsonAsync(Response.Fail("Unathorized access denied."));
                               }
                           };
                       });
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
