using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Services;
using Veilingklok.Features.Veiling.Services;
using Veilingklok.Features.VeilingPublic.Services;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Database.Seed;
using Veilingklok.Infrastructure.Repositories;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
using Veilingklok.Infrastructure.SignalR.Hubs;
using Veilingklok.Features.PrijsHistorie.Services;





AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("UNHANDLED EXCEPTION:");
    Console.WriteLine(e.ExceptionObject.ToString());
};

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<MyContext>(options =>
{
    // Als we lokaal draaien (Development)
    if (builder.Environment.IsDevelopment())
    {
        // Gebruik de lokale connection string uit appsettings.json
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        // Als we op Azure draaien (Production)
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING"));
    }
});

builder.Services.AddScoped<IPrijsHistorieService, PrijsHistorieService>();

builder.Services.AddSignalR()
    .AddJsonProtocol(o =>
        o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    );

builder.Services.AddHealthChecks();

builder.Services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(c => c.InNamespaces("Veilingklok.Features"))
    .AsMatchingInterface()
    .WithScopedLifetime()
);

builder.Services.AddScoped<IGebruikerRepository, GebruikerRepository>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IVeilingBroadcastService, VeilingBroadcastService>();
builder.Services.AddScoped<IVeilingPublicService, VeilingPublicService>();
builder.Services.AddHostedService<PrijsMechanismeService>();
builder.Services.AddHostedService<VeilingCleanupService>();


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hub/veiling"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

var allowedOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();

    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Veilingklok API",
        Version = "v1"
    });
   
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Plak hier je JWT token (zonder 'Bearer ')"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
culture.DateTimeFormat.DateSeparator = "-";

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var db = scope.ServiceProvider.GetRequiredService<MyContext>();
//        var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

//        await db.Database.MigrateAsync();
//        await DbSeeder.SeedAsync(db, passwordService);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("MIGRATION / SEED ERROR:");
//        Console.WriteLine(ex);
//    }
//}

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
            await ctx.Response.WriteAsJsonAsync(new
            {
                status = 400,
                message = ex.Message
            });
            return;
        }

        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(new
        {
            status = 500,
            message = "Interne serverfout."
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Veilingklok API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<AuctionHub>("/hub/veiling");

// Kleine startpagina zodat / geen 404 geeft
app.MapGet("/", () => Results.Ok("API draait. Gebruik /health of /api/..."));

app.Run();
