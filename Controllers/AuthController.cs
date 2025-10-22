using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VortexDemo.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private static readonly List<DemoUser> DemoUsers = new()
    {
        new DemoUser
        {
            Id = "user-1",
            Email = "admin@example.com",
            Password = "password123",
            Name = "Admin User",
            Role = "admin",
            Groups = new List<UserGroup>
            {
                new UserGroup { Type = "workspace", Id = "ws-1", Name = "Main Workspace" },
                new UserGroup { Type = "team", Id = "team-1", Name = "Engineering" }
            }
        },
        new DemoUser
        {
            Id = "user-2",
            Email = "alice@example.com",
            Password = "password123",
            Name = "Alice Johnson",
            Role = "admin",
            Groups = new List<UserGroup>
            {
                new UserGroup { Type = "workspace", Id = "ws-1", Name = "Main Workspace" },
                new UserGroup { Type = "team", Id = "team-1", Name = "Engineering" }
            }
        },
        new DemoUser
        {
            Id = "user-3",
            Email = "bob@example.com",
            Password = "password123",
            Name = "Bob Smith",
            Role = "member",
            Groups = new List<UserGroup>
            {
                new UserGroup { Type = "workspace", Id = "ws-1", Name = "Main Workspace" }
            }
        }
    };

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = DemoUsers.FirstOrDefault(u =>
            u.Email == request.Email && u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Ok(new { success = true, user = new { user.Id, user.Email, user.Name, user.Role, user.Groups } });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { success = true });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = DemoUsers.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new { user.Id, user.Email, user.Name, user.Role, user.Groups });
    }

    [HttpGet("users")]
    public IActionResult GetDemoUsers()
    {
        return Ok(DemoUsers.Select(u => new { u.Email, Password = "password123" }));
    }

    public static List<DemoUser> GetDemoUsersStatic()
    {
        return DemoUsers;
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class DemoUser
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<UserGroup> Groups { get; set; } = new();
}

public class UserGroup
{
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
