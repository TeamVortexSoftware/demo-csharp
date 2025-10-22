using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamVortexSoftware.VortexSDK;

namespace VortexDemo.Controllers;

[ApiController]
[Route("api/vortex")]
[Authorize]
public class VortexController : ControllerBase
{
    private readonly VortexClient _vortex;
    private static readonly List<DemoUser> DemoUsers = AuthController.GetDemoUsersStatic();

    public VortexController(VortexClient vortex)
    {
        _vortex = vortex;
    }

    private DemoUser? GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return DemoUsers.FirstOrDefault(u => u.Id == userId);
    }

    [HttpPost("jwt")]
    public IActionResult GenerateJWT()
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        try
        {
            var identifiers = new List<Identifier>
            {
                new Identifier("email", user.Email)
            };

            var groups = user.Groups.Select(g =>
                new Group(g.Type, g.Id, g.Name)).ToList();

            var jwt = _vortex.GenerateJwt(user.Id, identifiers, groups, user.Role);

            return Ok(new { jwt });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("invitations")]
    public async Task<IActionResult> GetInvitations([FromQuery] string targetType, [FromQuery] string targetValue)
    {
        try
        {
            var invitations = await _vortex.GetInvitationsByTargetAsync(targetType, targetValue);
            return Ok(new { invitations });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("invitations/{id}")]
    public async Task<IActionResult> GetInvitation(string id)
    {
        try
        {
            var invitation = await _vortex.GetInvitationAsync(id);
            return Ok(invitation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("invitations/{id}")]
    public async Task<IActionResult> RevokeInvitation(string id)
    {
        try
        {
            await _vortex.RevokeInvitationAsync(id);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("invitations/accept")]
    public async Task<IActionResult> AcceptInvitations([FromBody] AcceptInvitationsRequest request)
    {
        try
        {
            var target = new InvitationTarget(request.Target.Type, request.Target.Value);
            var result = await _vortex.AcceptInvitationsAsync(request.InvitationIds, target);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("invitations/by-group/{type}/{id}")]
    public async Task<IActionResult> GetInvitationsByGroup(string type, string id)
    {
        try
        {
            var invitations = await _vortex.GetInvitationsByGroupAsync(type, id);
            return Ok(new { invitations });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("invitations/by-group/{type}/{id}")]
    public async Task<IActionResult> DeleteInvitationsByGroup(string type, string id)
    {
        try
        {
            await _vortex.DeleteInvitationsByGroupAsync(type, id);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("invitations/{id}/reinvite")]
    public async Task<IActionResult> Reinvite(string id)
    {
        try
        {
            var result = await _vortex.ReinviteAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class AcceptInvitationsRequest
{
    public List<string> InvitationIds { get; set; } = new();
    public TargetRequest Target { get; set; } = new();
}

public class TargetRequest
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

// Helper extension for AuthController
public static class AuthControllerExtensions
{
    private static readonly List<DemoUser> DemoUsers = new()
    {
        new DemoUser
        {
            Id = "user-1",
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
            Id = "user-2",
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

    public static List<DemoUser> GetDemoUsersStatic() => DemoUsers;
}
