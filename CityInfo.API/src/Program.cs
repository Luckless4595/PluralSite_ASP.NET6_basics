using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using CityInfo.API.src.Services.Implementations;
using CityInfo.API.src.Services.Interfaces;
using CityInfo.API.src.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Logging
var useSerilog = true;

if (useSerilog)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console()
        .WriteTo.File("./logs/cityInfoLogs.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    
    builder.Host.UseSerilog();
} else{
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
}

// Redirect standard output stream back to the console
Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });

// Authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["Authentication:KeygenSecret"]))
    };
});

// Authorization
builder.Services.AddScoped<IAuthorizationHandler, CityRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanOnlyRequestPOIofHomeCity", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new CityRequirement());
    });
});

// Database
builder.Services.AddDbContext<CityInfoContext>(DbContextOptions => DbContextOptions.UseSqlite(
    builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]
));

// Services
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

// Controllers and formatters
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
// Order matters when working with middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
