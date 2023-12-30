using Microsoft.Extensions.DependencyInjection;

namespace NetApp.UI.Components;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNetAppUIComponents(this  IServiceCollection services)
    {
        services.AddScoped<IDialogService, DialogService>();

        return services;
    }
}
