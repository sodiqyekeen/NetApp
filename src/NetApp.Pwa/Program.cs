using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NetApp.Pwa;
using MudBlazor.Services;
using NetApp.UI.Infrastructure;
using Toolbelt.Blazor.Extensions.DependencyInjection;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddClientServices(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// builder.Services.AddTransient<AuthenticationHeaderHandler>()
//     .AddScoped(sp => sp
//         .GetRequiredService<IHttpClientFactory>()
//         .CreateClient("NetApp.Pwa").EnableIntercept(sp))
//     .AddHttpClient("NetApp.Pwa", client => client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] + "/api/"))
//     .AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddHttpClientInterceptor();

await builder.Build().RunAsync();
