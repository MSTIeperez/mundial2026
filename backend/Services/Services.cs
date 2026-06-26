using Microsoft.EntityFrameworkCore;
using Mundial2026.Api.Data;
using Mundial2026.Api.Models;

namespace Mundial2026.Api.Services;

// ─── Match Service ────────────────────────────────────────────────────────────
public interface IMatchService
{
    Task<List<MatchDto>> GetGroupMatchesAsync(string group);
    Task<List<MatchDto>> GetAllGroupMatchesAsync();
    Task<List<MatchDto>> GetKnockoutMatchesAsync(string phase);
    Task<bool> UpdateMatchScoreAsync(int matchId, int homeScore, int awayScore, int userId);
}

public record MatchDto(
    int Id, int MatchNumber, string Phase, string Group, int MatchDay,
    TeamInfo HomeTeam, TeamInfo AwayTeam,
    int? HomeScore, int? AwayScore, bool Played,
    DateTime MatchDate, string Venue, string? SlotLabel);

public record TeamInfo(int Id, string Name, string Code, string FlagEmoji);

public class MatchService : IMatchService
{
    private readonly AppDbContext _db;

    public MatchService(AppDbContext db) => _db = db;

    public async Task<List<MatchDto>> GetGroupMatchesAsync(string group)
    {
        return await _db.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.Phase == "group" && m.Group == group.ToUpper())
            .OrderBy(m => m.MatchDay).ThenBy(m => m.MatchDate)
            .Select(m => ToDto(m))
            .ToListAsync();
    }

    public async Task<List<MatchDto>> GetAllGroupMatchesAsync()
    {
        return await _db.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.Phase == "group")
            .OrderBy(m => m.Group).ThenBy(m => m.MatchDay).ThenBy(m => m.MatchDate)
            .Select(m => ToDto(m))
            .ToListAsync();
    }

    public async Task<List<MatchDto>> GetKnockoutMatchesAsync(string phase)
    {
        return await _db.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.Phase == phase)
            .OrderBy(m => m.MatchDate)
            .Select(m => ToDto(m))
            .ToListAsync();
    }

    public async Task<bool> UpdateMatchScoreAsync(int matchId, int homeScore, int awayScore, int userId)
    {
        var match = await _db.Matches.FindAsync(matchId);
        if (match == null || match.Phase != "group") return false;

        match.HomeScore = homeScore;
        match.AwayScore = awayScore;
        match.Played = true;
        match.UpdatedByUserId = userId;
        match.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    private static MatchDto ToDto(Match m) => new(
        m.Id, m.MatchNumber, m.Phase, m.Group, m.MatchDay,
        new TeamInfo(m.HomeTeam.Id, m.HomeTeam.Name, m.HomeTeam.Code, m.HomeTeam.FlagEmoji),
        new TeamInfo(m.AwayTeam.Id, m.AwayTeam.Name, m.AwayTeam.Code, m.AwayTeam.FlagEmoji),
        m.HomeScore, m.AwayScore, m.Played,
        m.MatchDate, m.Venue, m.SlotLabel);
}

// ─── Standings Service ────────────────────────────────────────────────────────
public interface IStandingsService
{
    Task<List<StandingDto>> GetGroupStandingsAsync(string group);
    Task<Dictionary<string, List<StandingDto>>> GetAllStandingsAsync();
    Task<List<KnockoutSlotDto>> GetAllKnockoutSlotsAsync();
}

public record StandingDto(
    int Position, TeamInfo Team, string Group,
    int Played, int Won, int Drawn, int Lost,
    int GoalsFor, int GoalsAgainst, int GoalDifference, int Points);

public record KnockoutSlotDto(int MatchNumber, string SlotLabel, TeamInfo? HomeTeam, TeamInfo? AwayTeam, bool Resolved);

public class StandingsService : IStandingsService
{
    private readonly AppDbContext _db;

    public StandingsService(AppDbContext db) => _db = db;

    public async Task<List<StandingDto>> GetGroupStandingsAsync(string group)
    {
        var matches = await _db.Matches
            .Include(m => m.HomeTeam).Include(m => m.AwayTeam)
            .Where(m => m.Phase == "group" && m.Group == group.ToUpper() && m.Played)
            .ToListAsync();

        var teams = await _db.Teams.Where(t => t.Group == group.ToUpper()).ToListAsync();
        return CalculateStandings(group.ToUpper(), teams, matches);
    }

    public async Task<Dictionary<string, List<StandingDto>>> GetAllStandingsAsync()
    {
        var matches = await _db.Matches
            .Include(m => m.HomeTeam).Include(m => m.AwayTeam)
            .Where(m => m.Phase == "group" && m.Played)
            .ToListAsync();

        var teams = await _db.Teams.ToListAsync();
        var result = new Dictionary<string, List<StandingDto>>();

        foreach (var g in teams.GroupBy(t => t.Group).OrderBy(g => g.Key))
        {
            var groupMatches = matches.Where(m => m.Group == g.Key).ToList();
            result[g.Key] = CalculateStandings(g.Key, g.ToList(), groupMatches);
        }

        return result;
    }

    public async Task<List<KnockoutSlotDto>> GetAllKnockoutSlotsAsync()
    {
        // Get standings for all groups
        var allStandings = await GetAllStandingsAsync();
        var knockoutMatches = await _db.Matches
            .Include(m => m.HomeTeam).Include(m => m.AwayTeam)
            .Where(m => m.Phase != "group")
            .OrderBy(m => m.MatchDate)
            .ToListAsync();

        // Get best 3rd-place teams across all groups
        var thirdPlace = allStandings.Values
            .Where(s => s.Count >= 3)
            .Select(s => s[2]) // 3rd in each group
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .ToList();

        // Resolve slots based on current standings
        var resolvedSlots = new List<KnockoutSlotDto>();
        foreach (var m in knockoutMatches)
        {
            // 1. Intentamos resolver quién juega usando las reglas de llaves (Ej. "1A vs 2B")
            var (home, away) = ResolveSlot(m.MatchNumber, m.SlotLabel ?? "", allStandings, thirdPlace);

            // 2. Si aún no se resuelve, mantenemos los placeholders de la base de datos (si existen)
            var finalHome = home ?? (m.HomeTeamId > 0 && m.HomeTeam != null && m.HomeTeam.Id != 1 ? 
                new TeamInfo(m.HomeTeam.Id, m.HomeTeam.Name, m.HomeTeam.Code, m.HomeTeam.FlagEmoji) : null);
                
            var finalAway = away ?? (m.AwayTeamId > 0 && m.AwayTeam != null && m.AwayTeam.Id != 2 ? 
                new TeamInfo(m.AwayTeam.Id, m.AwayTeam.Name, m.AwayTeam.Code, m.AwayTeam.FlagEmoji) : null);

            // 3. Agregamos el DTO con su MatchNumber
            resolvedSlots.Add(new KnockoutSlotDto(
                m.MatchNumber,             // <-- Pasamos el número de partido al frontend
                m.SlotLabel ?? "", 
                finalHome, 
                finalAway, 
                finalHome != null && finalAway != null
            ));
        }

        return resolvedSlots;
    }

    private (TeamInfo? home, TeamInfo? away) ResolveSlot(
        int matchNumber, string slotLabel,
        Dictionary<string, List<StandingDto>> standings,
        List<StandingDto> thirdPlace)
    {
        // Parse slot like "1A vs 2B"
        var parts = slotLabel.Split(" vs ");
        if (parts.Length != 2) return (null, null);

        TeamInfo? ResolveTeam(string token)
        {
            token = token.Trim();
            if (token.Length >= 2 && char.IsDigit(token[0]))
            {
                var pos = int.Parse(token[0].ToString()) - 1;
                var grp = token[1].ToString();
                if (standings.TryGetValue(grp, out var st) && st.Count > pos)
                    return st[pos].Team;
            }
            else if (token.StartsWith("3rd"))
            {
                // Best 3rd place
                return thirdPlace.FirstOrDefault()?.Team;
            }
            return null;
        }

        return (ResolveTeam(parts[0]), ResolveTeam(parts[1]));
    }

    private static List<StandingDto> CalculateStandings(
        string group, List<Team> teams, List<Match> matches)
    {
        var stats = teams.ToDictionary(t => t.Id, t => new
        {
            Team = t,
            W = 0, D = 0, L = 0, GF = 0, GA = 0, P = 0
        });

        // Mutable dict
        var s = teams.ToDictionary(t => t.Id, _ => new int[6]); // W,D,L,GF,GA,P

        foreach (var m in matches.Where(m => m.Played && m.HomeScore.HasValue))
        {
            var hg = m.HomeScore!.Value;
            var ag = m.AwayScore!.Value;

            s[m.HomeTeamId][3] += hg;
            s[m.HomeTeamId][4] += ag;
            s[m.AwayTeamId][3] += ag;
            s[m.AwayTeamId][4] += hg;

            if (hg > ag) { s[m.HomeTeamId][0]++; s[m.AwayTeamId][2]++; s[m.HomeTeamId][5] += 3; }
            else if (hg == ag) { s[m.HomeTeamId][1]++; s[m.AwayTeamId][1]++; s[m.HomeTeamId][5]++; s[m.AwayTeamId][5]++; }
            else { s[m.AwayTeamId][0]++; s[m.HomeTeamId][2]++; s[m.AwayTeamId][5] += 3; }
        }

        return teams
            .OrderByDescending(t => s[t.Id][5])
            .ThenByDescending(t => s[t.Id][3] - s[t.Id][4])
            .ThenByDescending(t => s[t.Id][3])
            .ThenBy(t => t.Name)
            .Select((t, i) =>
            {
                var played = s[t.Id][0] + s[t.Id][1] + s[t.Id][2];
                return new StandingDto(
                    i + 1,
                    new TeamInfo(t.Id, t.Name, t.Code, t.FlagEmoji),
                    group, played,
                    s[t.Id][0], s[t.Id][1], s[t.Id][2],
                    s[t.Id][3], s[t.Id][4],
                    s[t.Id][3] - s[t.Id][4],
                    s[t.Id][5]);
            })
            .ToList();
    }
}
