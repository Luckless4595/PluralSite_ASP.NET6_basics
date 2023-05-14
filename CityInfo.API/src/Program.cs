using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;


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

builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);
    setupAction.ReportApiVersions = true;
});

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
builder.Services.AddSwaggerGen(setupAction=>
{
    var xmDocsFile = "Documentation.xml"; //  $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmDocsFilePath = Path.Combine(Directory.GetCurrentDirectory(),xmDocsFile);//(AppContext.BaseDirectory, xmDocsFile);
    setupAction.IncludeXmlComments(xmDocsFilePath);

       setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API"
    });
    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CityInfoApiBearerAuth" }
            }, new List<string>() }
    });
});

// Build the application
var app = builder.Build();

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
