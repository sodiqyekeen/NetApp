global using NetApp.Models;
global using NetApp.Dtos;
global using NetApp.Application.Services;
using NetApp.Infrastructure;
using NetApp.Application;

using NetApp.Api.Endpoints;
using NetApp.Api.Services;
using NetApp.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["AllowedHosts"] ?? "*");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ApiErrorHandler>();
var api = app.MapGroup("/api").WithOpenApi();
api.MapGroup("identity")
.MapIdentityEndpoints();

app.SeedDatabase();
app.Run();
