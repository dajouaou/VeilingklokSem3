using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scrutor;
using Veilingklok.Hubs;
using Veilingklok.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ----------------------------------------------------
// Logging
// ----------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ----------------------------------------------------
// Controllers + JSON (camelCase)
// ----------------------------------------------------
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(o =>
    {
        o.SerializerSettings.ContractResolver =
            new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    });



// ----------------------------------------------------
// Database (EF Core 8 + SQL Server)
// ----------------------------------------------------
builder.Services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
);

// ----------------------------------------------------
// AutoMapper
// ----------------------------------------------------
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// ----------------------------------------------------
// SignalR
// ----------------------------------------------------
builder.Services.AddSignalR()
    .AddJsonProtocol();

// ----------------------------------------------------
// Health checks
// ----------------------------------------------------
builder.Services.AddHealthChecks();

// ----------------------------------------------------
// DI scanner Scrutor voor  Features en services
// ----------------------------------------------------
builder.Services.Scan(scan =>
    scan.FromApplicationDependencies()
        .AddClasses(c => c.InNamespaces("Veilingklok.Features"))
        .AsMatchingInterface()
        .WithScopedLifetime()
);

// ----------------------------------------------------
// Swagger / OpenAPI Swashbuckle 10
// ----------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Veilingklok API",
        Version = "v1"
    });
});

// ----------------------------------------------------
// CORS voor Vite localhost:5173
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

var app = builder.Build();

// ----------------------------------------------------
// Global error handling
// ----------------------------------------------------
app.UseExceptionHandler(err =>
{
    err.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            status = 500,
            message = "Er ging iets mis op de server."
        });
    });
});

// ----------------------------------------------------
// Development tools
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

// Health endpoint
app.MapHealthChecks("/health");

// Controllers & SignalR hub
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auction");

app.Run();
