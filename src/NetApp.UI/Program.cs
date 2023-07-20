using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NetApp.UI;
using NetApp.UI.Infrastructure;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddClientServices(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddTransient<AuthenticationHeaderHandler>()
    .AddScoped(sp => sp
        .GetRequiredService<IHttpClientFactory>()
        .CreateClient("NetApp.UI").EnableIntercept(sp))
    .AddHttpClient("NetApp.UI", client => client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] + "/api/"))
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddHttpClientInterceptor();

await builder.Build().RunAsync();
