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
    Task<List<MatchDto>> GetAllKnockoutMatchesAsync();
    Task<bool> UpdateMatchScoreAsync(int matchId, int homeScore, int awayScore, int userId);
}

public record MatchDto(
    int Id,
    int MatchNumber,        // En grupos = MatchDay (1-3); en eliminatorias = número FIFA (73-88)
    string Phase,
    string Group,
    int MatchDay,
    TeamInfo? HomeTeam,     // Nullable: null cuando el equipo aún no clasificó
    TeamInfo? AwayTeam,     // Nullable: null cuando el equipo aún no clasificó
    int? HomeScore,
    int? AwayScore,
    bool Played,
    DateTime MatchDate,
    string Venue,
    string? SlotLabel);

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

    public async Task<List<MatchDto>> GetAllKnockoutMatchesAsync()
    {
        return await _db.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Where(m => m.Phase != "group")
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

    // FIX 1: MatchNumber = MatchDay (el seeder guarda el número FIFA 73-88 en MatchDay para eliminatorias)
    // FIX 2: HomeTeam / AwayTeam son null en partidos eliminatorios pendientes → null-safe
    private static MatchDto ToDto(Match m) => new(
        m.Id,
        m.MatchDay,   // MatchDay almacena: jornada (1-3) en grupos, número FIFA (73-88) en eliminatorias
        m.Phase,
        m.Group,
        m.MatchDay,
        m.HomeTeam != null
            ? new TeamInfo(m.HomeTeam.Id, m.HomeTeam.Name, m.HomeTeam.Code, m.HomeTeam.FlagEmoji)
            : null,
        m.AwayTeam != null
            ? new TeamInfo(m.AwayTeam.Id, m.AwayTeam.Name, m.AwayTeam.Code, m.AwayTeam.FlagEmoji)
            : null,
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

public record KnockoutSlotDto(
    int MatchNumber,
    string SlotLabel,
    TeamInfo? HomeTeam,
    TeamInfo? AwayTeam,
    bool Resolved);

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
        var allStandings = await GetAllStandingsAsync();
        var knockoutMatches = await _db.Matches
            .Include(m => m.HomeTeam).Include(m => m.AwayTeam)
            .Where(m => m.Phase != "group")
            .OrderBy(m => m.MatchDate)
            .ToListAsync();

        // Mejores 3eros ordenados por criterios FIFA
        var thirdPlace = allStandings.Values
            .Where(s => s.Count >= 3)
            .Select(s => s[2])
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.GoalsFor)
            .ToList();

        var resolvedSlots = new List<KnockoutSlotDto>();
        foreach (var m in knockoutMatches)
        {
            // FIX 1: MatchNumber = MatchDay (el seeder guarda el número FIFA en MatchDay)
            var matchNum = m.MatchDay;

            // 1. Resolver equipos por las reglas de llaves del SlotLabel
            var (home, away) = ResolveSlot(m.SlotLabel ?? "", allStandings, thirdPlace);

            // FIX 2: el fallback ya no compara con IDs hardcodeados (1/2),
            //        solo verifica que el equipo exista en la BD (HomeTeamId != null)
            var finalHome = home
                ?? (m.HomeTeamId.HasValue && m.HomeTeam != null
                    ? new TeamInfo(m.HomeTeam.Id, m.HomeTeam.Name, m.HomeTeam.Code, m.HomeTeam.FlagEmoji)
                    : null);

            var finalAway = away
                ?? (m.AwayTeamId.HasValue && m.AwayTeam != null
                    ? new TeamInfo(m.AwayTeam.Id, m.AwayTeam.Name, m.AwayTeam.Code, m.AwayTeam.FlagEmoji)
                    : null);

            resolvedSlots.Add(new KnockoutSlotDto(
                matchNum,
                m.SlotLabel ?? "",
                finalHome,
                finalAway,
                finalHome != null && finalAway != null));
        }

        return resolvedSlots;
    }

    private (TeamInfo? home, TeamInfo? away) ResolveSlot(
        string slotLabel,
        Dictionary<string, List<StandingDto>> standings,
        List<StandingDto> thirdPlace)
    {
        var parts = slotLabel.Split(" vs ");
        if (parts.Length != 2) return (null, null);

        TeamInfo? ResolveTeam(string token)
        {
            token = token.Trim();

            // Formato "1A", "2B" → posición + grupo
            if (token.Length >= 2 && char.IsDigit(token[0]))
            {
                var pos = int.Parse(token[0].ToString()) - 1; // 0-based
                var grp = token[1].ToString().ToUpper();
                if (standings.TryGetValue(grp, out var st) && st.Count > pos)
                    return st[pos].Team;
                return null;
            }

            // FIX 4: el formato del seeder es "3°(A/B/C/D/F)", no "3rd"
            //        Soportamos ambos por compatibilidad
            if (token.StartsWith("3°") || token.StartsWith("3rd") || token.StartsWith("3"))
            {
                // Extraer grupos elegibles entre paréntesis: "3°(A/B/C/D/F)" → {A,B,C,D,F}
                var eligible = new HashSet<string>();
                var start = token.IndexOf('(');
                var end   = token.IndexOf(')');
                if (start >= 0 && end > start)
                {
                    var inner = token[(start + 1)..end];
                    foreach (var g in inner.Split('/'))
                        eligible.Add(g.Trim().ToUpper());
                }

                // Si no hay grupos entre paréntesis, devolver el mejor 3ero global
                if (eligible.Count == 0)
                    return thirdPlace.FirstOrDefault()?.Team;

                // Mejor 3ero de entre los grupos elegibles para este slot
                return thirdPlace.FirstOrDefault(s => eligible.Contains(s.Group))?.Team;
            }

            return null;
        }

        return (ResolveTeam(parts[0]), ResolveTeam(parts[1]));
    }

    // FIX 5: HomeTeamId / AwayTeamId son int? → usar .HasValue y .Value antes de indexar
    private static List<StandingDto> CalculateStandings(
        string group, List<Team> teams, List<Match> matches)
    {
        var s = teams.ToDictionary(t => t.Id, _ => new int[6]); // W,D,L,GF,GA,Pts

        foreach (var m in matches.Where(m =>
            m.Played &&
            m.HomeScore.HasValue &&
            m.HomeTeamId.HasValue &&   // null-check: partido de grupos siempre tiene equipo
            m.AwayTeamId.HasValue))
        {
            var hg  = m.HomeScore!.Value;
            var ag  = m.AwayScore!.Value;
            var hId = m.HomeTeamId!.Value;
            var aId = m.AwayTeamId!.Value;

            s[hId][3] += hg; s[hId][4] += ag;
            s[aId][3] += ag; s[aId][4] += hg;

            if      (hg > ag)  { s[hId][0]++; s[aId][2]++; s[hId][5] += 3; }
            else if (hg == ag) { s[hId][1]++; s[aId][1]++; s[hId][5]++; s[aId][5]++; }
            else               { s[aId][0]++; s[hId][2]++; s[aId][5] += 3; }
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
