
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDayAP1.Data;
using RaceDayAP1.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services

   .AddAuthentication("RaceDay.Auth")

.AddCookie("RaceDay.Auth", options =>

{
        options.Cookie.Name = "RaceDay.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };

    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordHasher<UserAccount>,
    PasswordHasher<UserAccount>>();
builder.Services.AddControllers();
builder.Services.AddDbContext<RaceDayContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build(); 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
public partial class Program { }