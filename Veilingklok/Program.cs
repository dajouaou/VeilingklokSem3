using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Hubs;
using Veilingklok.Infrastructure.Database;
using Scrutor;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ======================================================
// 1. DATABASE (EF Core 8)
// ======================================================
builder.Services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
);

// ======================================================
// 2. CONTROLLERS + JSON + VALIDATION
// ======================================================
builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddFluentValidation();

// Registreer ALLE validators automatisch
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ======================================================
// 3. AUTOMAPPER (12.0.1 — matching DI extensie)
// ======================================================
builder.Services.AddAutoMapper(typeof(Program));

// ======================================================
// 4. SIGNALR (Realtime met JSON protocol)
// ======================================================
builder.Services.AddSignalR()
    .AddJsonProtocol();

// ======================================================
// 5. DEPENDENCY INJECTION (SCRUTOR AUTO-SCANNER)
// ======================================================
//
// Registreert automatisch ALLE services:
// - IProductService → ProductService
// - IAuctionService → AuctionService
// enzovoort.
//
// Dit werkt perfect met jouw Features/… structuur.
//
builder.Services.Scan(scan =>
    scan.FromAssemblyOf<Program>()
        .AddClasses()                  // alle classes
        .AsMatchingInterface()         // interface met zelfde naam
        .WithScopedLifetime()
);

// ======================================================
// 6. SWAGGER (OpenAPI 3)
// ======================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================================================
// 7. CORS (voor Vite React localhost:5173)
// ======================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(
                "http://localhost:5173"   // Vite dev
            )
    );
});

// ======================================================
// BUILD APPLICATION
// ======================================================
var app = builder.Build();

// ======================================================
// 8. MIDDLEWARE PIPELINE
// ======================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

// ======================================================
// 9. ROUTING
// ======================================================
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auction");

// ======================================================
// 10. START APP
// ======================================================
app.Run();
