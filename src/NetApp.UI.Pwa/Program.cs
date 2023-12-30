global using NetApp.UI.Infrastructure;
global using NetApp.UI.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NetApp.UI.Pwa;
using System.Net.Http;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddNetAppUIComponents();
//builder.Services.AddOidcAuthentication(options =>
//{
//    // Configure your authentication provider options here.
//    // For more information, see https://aka.ms/blazor-standalone-auth
//    builder.Configuration.Bind("Local", options.ProviderOptions);
//});
//builder.Services.AddClientServices(builder.Configuration);
//builder.Services.AddTransient<AuthenticationHeaderHandler>()
//    .AddScoped(sp => sp
//        .GetRequiredService<IHttpClientFactory>()
//        .CreateClient("NetApp.UI.Pwa").EnableIntercept(sp))
//    .AddHttpClient("NetApp.UI.Pwa", client =>
//    {
//        client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] + "/api/");
//        client.Timeout = TimeSpan.FromSeconds(60);
//    })
//    .AddHttpMessageHandler<AuthenticationHeaderHandler>();
//builder.Services.AddHttpClientInterceptor();
await builder.Build().RunAsync();
