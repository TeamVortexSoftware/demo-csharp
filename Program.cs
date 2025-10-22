using Microsoft.AspNetCore.Authentication.Cookies;
using TeamVortexSoftware.VortexSDK;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "session";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Register Vortex client
builder.Services.AddSingleton(sp =>
{
    var apiKey = Environment.GetEnvironmentVariable("VORTEX_API_KEY") ?? "demo-api-key";
    return new VortexClient(apiKey);
});

var app = builder.Build();

// Configure middleware
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 Vortex C# SDK Demo Server");
Console.WriteLine("📡 Server running on http://localhost:5001");
Console.WriteLine("🔑 Using API key: " + (Environment.GetEnvironmentVariable("VORTEX_API_KEY") ?? "demo-api-key")[..Math.Min(10, (Environment.GetEnvironmentVariable("VORTEX_API_KEY") ?? "demo-api-key").Length)] + "...");

app.Run("http://localhost:5001");
