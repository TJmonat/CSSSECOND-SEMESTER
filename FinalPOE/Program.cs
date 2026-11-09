using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using RaceDayAP1.Data;

using RaceDayAP1.Models;

using Microsoft.OpenApi;

using RaceDayAP1.Services;

using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);



var azureConnectionString =

    builder.Configuration["AzureBlobStorage:ConnectionString"];

if (string.IsNullOrWhiteSpace(azureConnectionString))

{

    throw new InvalidOperationException(

        "Azure Blob Storage connection string is missing.");

}

builder.Services.AddSingleton(

    new BlobServiceClient(azureConnectionString));

builder.Services.AddScoped<AzureBlobStorageService>();


builder.Services

    .AddAuthentication("RaceDay.Auth")

    .AddCookie("RaceDay.Auth", options =>

    {

        options.Cookie.Name = "RaceDay.Auth";

        options.Cookie.HttpOnly = true;

        options.Cookie.SecurePolicy =

            CookieSecurePolicy.Always;

        options.Cookie.SameSite = SameSiteMode.Lax;

        options.SlidingExpiration = true;

        options.ExpireTimeSpan =

            TimeSpan.FromMinutes(30);

        options.Events.OnRedirectToLogin = context =>

        {

            context.Response.StatusCode =

                StatusCodes.Status401Unauthorized;

            return Task.CompletedTask;

        };

        options.Events.OnRedirectToAccessDenied = context =>

        {

            context.Response.StatusCode =

                StatusCodes.Status403Forbidden;

            return Task.CompletedTask;

        };

    });

builder.Services.AddAuthorization();


builder.Services.AddScoped<

    IPasswordHasher<UserAccount>,

    PasswordHasher<UserAccount>>();

builder.Services.AddControllers();


builder.Services.AddDbContext<RaceDayContext>(options =>

    options.UseSqlServer(

        builder.Configuration.GetConnectionString(

            "DefaultConnection")));



builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>

{

    options.AddSecurityDefinition("RaceDay.Auth",

        new OpenApiSecurityScheme

        {

            Name = "RaceDay.Auth",

            Type = SecuritySchemeType.ApiKey,

            In = ParameterLocation.Cookie,

            Description = "RaceDay authentication cookie"

        });

    options.AddSecurityRequirement(document =>

        new OpenApiSecurityRequirement

        {

            [new OpenApiSecuritySchemeReference(

                "RaceDay.Auth", document)] = new List<string>()

        });

});
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