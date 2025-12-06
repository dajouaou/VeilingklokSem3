using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Services;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Veilingklok.Features.Veiling.Services;
using Veilingklok.Features.VM.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;



AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("UNHANDLED EXCEPTION:");
    Console.WriteLine(e.ExceptionObject?.ToString());
};

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);


builder.Services.AddDbContext<MyContext>(opt =>
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));




builder.Services.AddSignalR()
    .AddJsonProtocol(o =>
        o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);


builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(c => c.InNamespaces("Veilingklok.Features"))
    .AsMatchingInterface()
    .WithScopedLifetime());

builder.Services.AddScoped<IVMService, VMService>();
builder.Services.AddScoped<IVeilingService, VeilingService>();



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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"])
            )
        };
    });


builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowFrontend", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    ));

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<MyContext>();
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine("MIGRATION ERROR:");
        Console.WriteLine(ex.ToString());
    }
}


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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHealthChecks("/health");



app.Run();
