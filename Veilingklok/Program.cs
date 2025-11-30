using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Repositories.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Logging (console + debug)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Controllers + JSON (camelCase)
builder.Services.AddControllers()
    .AddNewtonsoftJson(o =>
        o.SerializerSettings.ContractResolver =
            new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver());

// EF Core (SQL Server)
builder.Services.AddDbContext<MyContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// AutoMapper (scant huidige assembly)
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// SignalR (realtime)
builder.Services.AddSignalR().AddJsonProtocol();

// Health checks (/health)
builder.Services.AddHealthChecks();

// DI scan (services in Veilingklok.Features)
builder.Services.Scan(scan => scan.FromApplicationDependencies()
    .AddClasses(c => c.InNamespaces("Veilingklok.Features"))
    .AsMatchingInterface()
    .WithScopedLifetime());

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Veilingklok API", Version = "v1" });
});
builder.Services.AddSwaggerGenNewtonsoftSupport(); // nodig i.c.m. NewtonsoftJson

// CORS (Vite)
builder.Services.AddCors(opt => opt.AddPolicy("AllowFrontend", p => p
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

var app = builder.Build();

// Global error handler (1 plek voor 500)
app.UseExceptionHandler(a => a.Run(async ctx =>
{
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { status = 500, message = "Er ging iets mis op de server." });
}));

// Swagger UI alleen in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

// Routes
app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auction");

app.Run();
