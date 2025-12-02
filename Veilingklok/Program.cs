using System.Text;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Validators;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Database.Seed;
using Veilingklok.Infrastructure.SignalR;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// -------------------------------------------------------------
// LOGGING
// -------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// -------------------------------------------------------------
// CONTROLLERS + JSON camelCase
// -------------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// -------------------------------------------------------------
// DATABASE (SQL Server via EF Core) 
// -------------------------------------------------------------
builder.Services.AddDbContext<MyContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// -------------------------------------------------------------
// AUTOMAPPER – laad ALLE mapping-profielen in hele solution
// -------------------------------------------------------------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// -------------------------------------------------------------
// SIGNALR – realtime + camelCase JSON
// -------------------------------------------------------------
builder.Services.AddSignalR()
    .AddJsonProtocol(o =>
        o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// -------------------------------------------------------------
// HEALTHCHECKS
// -------------------------------------------------------------
builder.Services.AddHealthChecks();

// -------------------------------------------------------------
// DEPENDENCY INJECTION – ALLE Feature-services automatisch
// -------------------------------------------------------------
builder.Services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(c => c.InNamespaces(
        "Veilingklok.Features",
        "Veilingklok.Core",
        "Veilingklok.Infrastructure"))
    .AsMatchingInterface()
    .WithScopedLifetime());

// Dispatcher voor realtime SignalR events
builder.Services.AddScoped<IAuctionEventDispatcher, AuctionEventDispatcher>();

// -------------------------------------------------------------
// FLUENTVALIDATION 
// -------------------------------------------------------------
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

// -------------------------------------------------------------
// SWAGGER + JWT Security
// -------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Veilingklok API",
        Version = "v1",
        Description = "Digitale veilingklok voor Royal FloraHolland (studentproject jem-id)"
    });

    // JWT bearer token input
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Vul de JWT-token in. Format: <token> (zonder 'Bearer ' prefix)",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Alle endpoints vereisen standaard een geldige JWT
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// -------------------------------------------------------------
// CORS – voor Vite + React + SignalR
// -------------------------------------------------------------
builder.Services.AddCors(opt => opt.AddPolicy("AllowFrontend", p => p
    .WithOrigins(
        "http://localhost:5173",
        "http://127.0.0.1:5173"
    )
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .ExposeHeaders("Authorization")
));

// -------------------------------------------------------------
// AUTHENTICATIE – JWT
// -------------------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// -------------------------------------------------------------
// DATABASE MIGRATIE + SEED
// -------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

// -------------------------------------------------------------
// GLOBAL ERROR HANDLER — uniforme API errors
// -------------------------------------------------------------
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async ctx =>
    {
        ctx.Response.ContentType = "application/json";
        var feature = ctx.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        switch (ex)
        {
            case ArgumentException:
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsJsonAsync(new { status = 400, message = ex.Message });
                return;

            case UnauthorizedAccessException:
                ctx.Response.StatusCode = 401;
                await ctx.Response.WriteAsJsonAsync(new { status = 401, message = ex.Message });
                return;
        }

        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsJsonAsync(new { status = 500, message = "Server error." });
    });
});

// -------------------------------------------------------------
// SWAGGER
// -------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// -------------------------------------------------------------
// PIPELINE
// -------------------------------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// -------------------------------------------------------------
// ENDPOINTS
// -------------------------------------------------------------
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<AuctionHub>("/hubs/auction");

app.Run();
