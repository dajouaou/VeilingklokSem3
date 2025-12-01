using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Services;
using Veilingklok.Features.VeilingmeesterDashboard.Mapping;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Database.Seed; // Seeder
using Veilingklok.Infrastructure.Repositories;
using Veilingklok.Infrastructure.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Controllers (camelCase JSON)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// DbContext
builder.Services.AddDbContext<MyContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(VeilingDashboardMappingProfile).Assembly);

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

// Dispatcher
builder.Services.AddScoped<IAuctionEventDispatcher, AuctionEventDispatcher>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Veilingklok API", Version = "v1" });
});

// CORS voor Vite + SignalR
builder.Services.AddCors(opt => opt.AddPolicy("AllowFrontend", p => p
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

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
    var db = scope.ServiceProvider.GetRequiredService<MyContext>();
    await db.Database.MigrateAsync();   // maakt/upgrade db
    await DbSeeder.SeedAsync(db);       // seed alleen als leeg
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
        c.RoutePrefix = string.Empty; // Swagger op root: http://localhost:5000/
    });
}

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
