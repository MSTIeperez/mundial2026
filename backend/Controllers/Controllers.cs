using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Mundial2026.Api.Services;

namespace Mundial2026.Api.Controllers;

// ─── Auth Controller ──────────────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _auth.LoginAsync(req.Username, req.Password);
        if (!result.Success) return Unauthorized(new { error = result.Error });
        return Ok(new { token = result.Token, user = result.User });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var result = await _auth.RegisterAsync(req.Username, req.Email, req.Password);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(new { token = result.Token, user = result.User });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return Ok(new { id = userId, username, role });
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Email, string Password);

// ─── Matches Controller ───────────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matches;
    public MatchesController(IMatchService matches) => _matches = matches;

    [HttpGet("groups")]
    public async Task<IActionResult> GetAllGroupMatches()
        => Ok(await _matches.GetAllGroupMatchesAsync());

    [HttpGet("groups/{group}")]
    public async Task<IActionResult> GetGroupMatches(string group)
        => Ok(await _matches.GetGroupMatchesAsync(group));

    [HttpGet("knockout/{phase}")]
    public async Task<IActionResult> GetKnockoutMatches(string phase)
        => Ok(await _matches.GetKnockoutMatchesAsync(phase));

    [HttpGet("knockout")]
        public async Task<IActionResult> GetAllKnockoutMatches()
            => Ok(await _matches.GetAllKnockoutMatchesAsync());

    [HttpPut("{id}/score")]
    public async Task<IActionResult> UpdateScore(int id, [FromBody] ScoreRequest req)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ok = await _matches.UpdateMatchScoreAsync(id, req.HomeScore, req.AwayScore, userId);
        if (!ok) return BadRequest(new { error = "Partido no encontrado o no es de fase de grupos" });
        return Ok(new { success = true });
    }
}

public record ScoreRequest(int HomeScore, int AwayScore);

// ─── Standings Controller ─────────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StandingsController : ControllerBase
{
    private readonly IStandingsService _standings;
    public StandingsController(IStandingsService standings) => _standings = standings;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _standings.GetAllStandingsAsync());

    [HttpGet("{group}")]
    public async Task<IActionResult> GetGroup(string group)
        => Ok(await _standings.GetGroupStandingsAsync(group));

    [HttpGet("knockout-slots")]
    public async Task<IActionResult> GetAllKnockoutSlots()
        => Ok(await _standings.GetAllKnockoutSlotsAsync());
}
