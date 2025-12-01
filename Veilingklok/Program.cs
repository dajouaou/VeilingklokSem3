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

// logging basic
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// controllers (camelCase json)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// DbContextOptions<MyContext> object aanmaken in program cs
builder.Services.AddDbContext<MyContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// automapper
builder.Services.AddAutoMapper(typeof(VeilingDashboardMappingProfile).Assembly);

// signalr (camelCase payloads)
builder.Services.AddSignalR()
    .AddJsonProtocol(o =>
        o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// health
builder.Services.AddHealthChecks();

// DI: jouw feature services
builder.Services.Scan(scan => scan.FromApplicationDependencies()
    .AddClasses(c => c.InNamespaces("Veilingklok.Features"))
    .AsMatchingInterface()
    .WithScopedLifetime());

// DI: dispatcher (realtime centraal)
builder.Services.AddScoped<IAuctionEventDispatcher, AuctionEventDispatcher>();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Veilingklok API", Version = "v1" }));

// cors voor Vite + SignalR
builder.Services.AddCors(opt => opt.AddPolicy("AllowFrontend", p => p
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));
//DI geconfigureerd 
builder.Services.AddScoped<IGebruikerRepository, GebruikerRepository>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
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

//  migrate + seed (MOET vóór app.Run)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyContext>();
    await db.Database.MigrateAsync();   //maakt/upgrade db
    await DbSeeder.SeedAsync(db);       // seed alleen als leeg
}

// errors: ArgumentException => 400, rest => 500
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

// swagger only dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // serve wwwroot (bv /img/products/...)

app.UseRouting();
app.UseCors("AllowFrontend"); // credentials
app.UseAuthorization();
// authenticatie
app.UseAuthentication();
app.UseAuthorization();


// endpoints
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<AuctionHub>("/hubs/auction");

app.Run();
