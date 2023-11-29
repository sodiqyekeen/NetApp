global using NetApp.Application.Services;
global using NetApp.Dtos;
global using NetApp.Models;
using NetApp.Api;
using NetApp.Api.Endpoints;
using NetApp.Api.Services;
using NetApp.Application;
using NetApp.Infrastructure;
using NetApp.Infrastructure.Identity.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["AllowedHosts"] ?? "*")));

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
api.MapIdentityEndpoints();
//app.MapIdentityApi<NetAppUser>();

app.SeedDatabase();
app.Run();
