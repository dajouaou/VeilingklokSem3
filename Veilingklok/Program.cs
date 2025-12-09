using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Services;
using Veilingklok.Features.VeilingPublic.Services;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Database.Seed; // Seeder
using Veilingklok.Infrastructure.Repositories;
using Veilingklok.Infrastructure.SignalR.Hubs;


AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("UNHANDLED EXCEPTION:");
    Console.WriteLine(e.ExceptionObject.ToString());
};

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Controllers (camelCase JSON)
builder.Services.AddControllers()
   
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
    .AddJsonOptions(o =>
 {
     o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
     o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
 });

builder.Services.AddScoped<IVeilingPublicService, VeilingPublicService>();

// DbContext
builder.Services.AddDbContext<MyContext>(opt =>
    opt.UseSqlite(config.GetConnectionString("DefaultConnection")));


// SignalR (camelCase payloads)
builder.Services.AddSignalR()
    .AddJsonProtocol(o => o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// Health Checks
builder.Services.AddHealthChecks();

// Dependency Injection: feature services
builder.Services.Scan(scan => scan.FromApplicationDependencies()
    .AddClasses(c => c.InNamespaces("Veilingklok.Features"))
    .AsMatchingInterface()
    .WithScopedLifetime());


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Veilingklok API", Version = "v1" });
});

// CORS voor Vite + SignalR
builder.Services.AddCors(opt => opt.AddPolicy("AllowFrontend", p => p
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()));


// DI services
builder.Services.AddScoped<IGebruikerRepository, GebruikerRepository>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Migrate + Seed (vóór app.Run)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<MyContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(db, passwordService); // ← FIX: geef passwordService mee
    }
    catch (Exception ex)
    {
        Console.WriteLine("MIGRATION/SEED ERROR:");
        Console.WriteLine(ex.ToString());
    }
}


// Global error handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async ctx =>
    {
        ctx.Response.ContentType = "application/json";
        var feature = ctx.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ex is ArgumentException)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { status = 400, message = ex.Message });
            return;
        }

        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(new { status = 500, message = "Server error." });
    });
});

// Swagger (development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Veilingklok API v1");
        c.RoutePrefix = "swagger";
    });
}
app.UseStaticFiles();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");

// Authentication + Authorization
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<AuctionHub>("/hubs/auction");

app.Run();
