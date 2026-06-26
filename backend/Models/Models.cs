namespace Mundial2026.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "user"; // admin | user
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = ""; // MEX, ARG, etc.
    public string FlagEmoji { get; set; } = "";
    public string Group { get; set; } = ""; // A-L
}

public class Match
{
    public int Id { get; set; }
    public string Phase { get; set; } = "group"; // group | round_of_16 | quarter | semi | third | final
    public string Group { get; set; } = ""; // A-L (only for group phase)
    public int MatchDay { get; set; } // 1, 2, 3 within group
    public int HomeTeamId { get; set; }
    public Team HomeTeam { get; set; } = null!;
    public int AwayTeamId { get; set; }
    public Team AwayTeam { get; set; } = null!;
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public bool Played { get; set; } = false;
    public DateTime MatchDate { get; set; }
    public string Venue { get; set; } = "";
    // For knockout rounds: which slot positions feed into this match
    public string? SlotLabel { get; set; } // e.g. "1A vs 2B"
    public int? UpdatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int MatchNumber { get; set; } // Sequential match number for scheduling
}

public class GroupStanding
{
    public string Group { get; set; } = "";
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int Points => Won * 3 + Drawn;
    public int GoalDifference => GoalsFor - GoalsAgainst;
}
