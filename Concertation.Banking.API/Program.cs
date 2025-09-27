using Carter;
using Concertation.Banking.API.Features.Auth.Handlers;
using Concertation.Banking.API.Features.Auth.Models;
using Concertation.Banking.API.Infrastructure.Database;
using Concertation.Banking.API.Shared.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .CreateLogger();

//builder.Host.UseSerilog();

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Configurations
builder.Services.AddControllers();

//builder.Services.AddValidatorsFromAssemblyContaining<TransferFundsRequestValidator>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

// Redis
builder.Services.AddRedis(builder.Configuration.GetConnectionString("Redis")!);

// Charge les paramètres Keycloak depuis appsettings.json
builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection("Keycloak"));

// Authentification via Keycloak (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string? keycloakUrl = builder.Configuration["Keycloak:BaseUrl"];
        string? realm = builder.Configuration["Keycloak:Realm"];
        string? audience = builder.Configuration["Keycloak:Audience"];

        options.Authority = $"{keycloakUrl}/realms/{realm}";
        options.Audience = audience;
        options.RequireHttpsMetadata = false; // Dev only

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{keycloakUrl}/realms/{realm}",
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, // 🔑 Obligatoire
            ClockSkew = TimeSpan.Zero
        };

        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{keycloakUrl}/realms/{realm}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever { RequireHttps = false } // ✅ pour autoriser HTTP en dev
            );

        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context => Task.CompletedTask,
            OnAuthenticationFailed = context => Task.CompletedTask
        };
    });

builder.Services.AddAuthorization();

// Services
builder.Services.AddApplicationServices();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCarter();

WebApplication app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();
app.MapScalarApiReference();
//}

app.UseHttpsRedirection();

//app.MapEndpoints();
app.MapCarter();
app.UseGlobalExceptionHandler();
app.UseSerilogRequestLogging();

//app.UseCors("AllowAngularOrigins");
app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    using IServiceScope scope = context.RequestServices.CreateScope();
    IServiceProvider services = scope.ServiceProvider;
    IWebHostEnvironment env = services.GetRequiredService<IWebHostEnvironment>();

    if (env.IsDevelopment())
    {
        AppDbContext db = services.GetRequiredService<AppDbContext>();
        // Force la création des tables si elles n’existent pas encore
        await db.Database.EnsureCreatedAsync();

        db.Database.Migrate();
        SeedData.Initialize(services);
    }

    await next();
});


app.Run();
