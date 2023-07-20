﻿global using NetApp.Models;
global using NetApp.Dtos;
global using NetApp.Extensions;
global using NetApp.Constants;
global using MudBlazor;
global using NetApp.UI.Infrastructure.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using NetApp.Shared.Constants;
using System.Reflection;
using static NetApp.Constants.SharedConstants;
using NetApp.UI.Infrastructure.Services;

namespace NetApp.UI.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions()
                .AddAuthorizationCore(options => RegisterPermissionClaims(options))
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .AddTransient<NetAppAuthStateProvider>()
                .AddTransient<AuthenticationStateProvider, NetAppAuthStateProvider>()
                .AddSingleton<IStorageService, LocalStorageService>();
        return services;
    }


    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is null) continue;
            options.AddPolicy(propertyValue.ToString()!, policy => policy.RequireClaim(CustomClaimTypes.Permission, propertyValue.ToString()!));
        }
    }
}
