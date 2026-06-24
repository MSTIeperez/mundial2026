using Mundial2026.Api.Models;
using BCrypt.Net;

namespace Mundial2026.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Users.Any()) return;

        // Seed admin user
        db.Users.Add(new User
        {
            Username = "admin",
            Email = "admin@mundial2026.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "admin"
        });

        // FIFA World Cup 2026 - 48 teams, 12 groups of 4
        var teams = new List<Team>
        {
            // Group A
            new() { Name = "México", Code = "MEX", FlagEmoji = "🇲🇽", Group = "A" },
            new() { Name = "Canadá", Code = "CAN", FlagEmoji = "🇨🇦", Group = "A" },
            new() { Name = "Ecuador", Code = "ECU", FlagEmoji = "🇪🇨", Group = "A" },
            new() { Name = "Jamaica", Code = "JAM", FlagEmoji = "🇯🇲", Group = "A" },
            // Group B
            new() { Name = "Argentina", Code = "ARG", FlagEmoji = "🇦🇷", Group = "B" },
            new() { Name = "Chile", Code = "CHI", FlagEmoji = "🇨🇱", Group = "B" },
            new() { Name = "Perú", Code = "PER", FlagEmoji = "🇵🇪", Group = "B" },
            new() { Name = "Australia", Code = "AUS", FlagEmoji = "🇦🇺", Group = "B" },
            // Group C
            new() { Name = "Estados Unidos", Code = "USA", FlagEmoji = "🇺🇸", Group = "C" },
            new() { Name = "Uruguay", Code = "URU", FlagEmoji = "🇺🇾", Group = "C" },
            new() { Name = "Panamá", Code = "PAN", FlagEmoji = "🇵🇦", Group = "C" },
            new() { Name = "Bolivia", Code = "BOL", FlagEmoji = "🇧🇴", Group = "C" },
            // Group D
            new() { Name = "Brasil", Code = "BRA", FlagEmoji = "🇧🇷", Group = "D" },
            new() { Name = "Colombia", Code = "COL", FlagEmoji = "🇨🇴", Group = "D" },
            new() { Name = "Venezuela", Code = "VEN", FlagEmoji = "🇻🇪", Group = "D" },
            new() { Name = "Costa Rica", Code = "CRC", FlagEmoji = "🇨🇷", Group = "D" },
            // Group E
            new() { Name = "Francia", Code = "FRA", FlagEmoji = "🇫🇷", Group = "E" },
            new() { Name = "Bélgica", Code = "BEL", FlagEmoji = "🇧🇪", Group = "E" },
            new() { Name = "Polonia", Code = "POL", FlagEmoji = "🇵🇱", Group = "E" },
            new() { Name = "Marruecos", Code = "MAR", FlagEmoji = "🇲🇦", Group = "E" },
            // Group F
            new() { Name = "España", Code = "ESP", FlagEmoji = "🇪🇸", Group = "F" },
            new() { Name = "Alemania", Code = "GER", FlagEmoji = "🇩🇪", Group = "F" },
            new() { Name = "Turquía", Code = "TUR", FlagEmoji = "🇹🇷", Group = "F" },
            new() { Name = "Georgia", Code = "GEO", FlagEmoji = "🇬🇪", Group = "F" },
            // Group G
            new() { Name = "Portugal", Code = "POR", FlagEmoji = "🇵🇹", Group = "G" },
            new() { Name = "Croacia", Code = "CRO", FlagEmoji = "🇭🇷", Group = "G" },
            new() { Name = "Dinamarca", Code = "DEN", FlagEmoji = "🇩🇰", Group = "G" },
            new() { Name = "Eslovenia", Code = "SVN", FlagEmoji = "🇸🇮", Group = "G" },
            // Group H
            new() { Name = "Inglaterra", Code = "ENG", FlagEmoji = "🏴󠁧󠁢󠁥󠁮󠁧󠁿", Group = "H" },
            new() { Name = "Países Bajos", Code = "NED", FlagEmoji = "🇳🇱", Group = "H" },
            new() { Name = "Serbia", Code = "SRB", FlagEmoji = "🇷🇸", Group = "H" },
            new() { Name = "Escocia", Code = "SCO", FlagEmoji = "🏴󠁧󠁢󠁳󠁣󠁴󠁿", Group = "H" },
            // Group I
            new() { Name = "Italia", Code = "ITA", FlagEmoji = "🇮🇹", Group = "I" },
            new() { Name = "Rumanía", Code = "ROU", FlagEmoji = "🇷🇴", Group = "I" },
            new() { Name = "Ucrania", Code = "UKR", FlagEmoji = "🇺🇦", Group = "I" },
            new() { Name = "Eslovaquia", Code = "SVK", FlagEmoji = "🇸🇰", Group = "I" },
            // Group J
            new() { Name = "Japón", Code = "JPN", FlagEmoji = "🇯🇵", Group = "J" },
            new() { Name = "Corea del Sur", Code = "KOR", FlagEmoji = "🇰🇷", Group = "J" },
            new() { Name = "Arabia Saudita", Code = "KSA", FlagEmoji = "🇸🇦", Group = "J" },
            new() { Name = "Nueva Zelanda", Code = "NZL", FlagEmoji = "🇳🇿", Group = "J" },
            // Group K
            new() { Name = "Senegal", Code = "SEN", FlagEmoji = "🇸🇳", Group = "K" },
            new() { Name = "Egipto", Code = "EGY", FlagEmoji = "🇪🇬", Group = "K" },
            new() { Name = "Nigeria", Code = "NGA", FlagEmoji = "🇳🇬", Group = "K" },
            new() { Name = "Camerún", Code = "CMR", FlagEmoji = "🇨🇲", Group = "K" },
            // Group L
            new() { Name = "Irán", Code = "IRN", FlagEmoji = "🇮🇷", Group = "L" },
            new() { Name = "Uzbekistán", Code = "UZB", FlagEmoji = "🇺🇿", Group = "L" },
            new() { Name = "Honduras", Code = "HON", FlagEmoji = "🇭🇳", Group = "L" },
            new() { Name = "Nueva Caledonia", Code = "NCL", FlagEmoji = "🇳🇨", Group = "L" },
        };

        db.Teams.AddRange(teams);
        await db.SaveChangesAsync();

        // Generate group stage matches
        var matches = new List<Match>();
        var groups = teams.GroupBy(t => t.Group).OrderBy(g => g.Key);

        var baseDate = new DateTime(2026, 6, 11);
        var venues = new Dictionary<string, string[]>
        {
            ["A"] = ["Estadio Azteca, CDMX", "AT&T Stadium, Dallas"],
            ["B"] = ["Hard Rock Stadium, Miami", "MetLife Stadium, NYC"],
            ["C"] = ["SoFi Stadium, LA", "Levi's Stadium, SF"],
            ["D"] = ["NRG Stadium, Houston", "Rose Bowl, LA"],
            ["E"] = ["BC Place, Vancouver", "Stade Olympique, Montreal"],
            ["F"] = ["Arrowhead Stadium, KC", "Lincoln Financial, Philly"],
            ["G"] = ["Gillette Stadium, Boston", "M&T Bank Stadium, Baltimore"],
            ["H"] = ["Bank of America, Charlotte", "Mercedes-Benz, Atlanta"],
            ["I"] = ["Camping World, Orlando", "Raymond James, Tampa"],
            ["J"] = ["AT&T Stadium, Dallas", "NRG Stadium, Houston"],
            ["K"] = ["MetLife Stadium, NYC", "Hard Rock Stadium, Miami"],
            ["L"] = ["SoFi Stadium, LA", "Rose Bowl, LA"],
        };

        int dayOffset = 0;
        foreach (var group in groups)
        {
            var groupTeams = group.ToList();
            var groupVenues = venues[group.Key];
            // Round robin: 3 matchdays (6 matches per group)
            // Day 1: T1 vs T2, T3 vs T4
            // Day 2: T1 vs T3, T2 vs T4  
            // Day 3: T1 vs T4, T2 vs T3
            var schedule = new[] {
                (0, 1, 2, 3), // matchday 1
                (0, 2, 1, 3), // matchday 2
                (0, 3, 1, 2), // matchday 3
            };

            for (int day = 0; day < 3; day++)
            {
                var (h1, a1, h2, a2) = schedule[day];
                matches.Add(new Match
                {
                    Phase = "group",
                    Group = group.Key,
                    MatchDay = day + 1,
                    HomeTeamId = groupTeams[h1].Id,
                    AwayTeamId = groupTeams[a1].Id,
                    MatchDate = baseDate.AddDays(dayOffset + day * 3),
                    Venue = groupVenues[0]
                });
                matches.Add(new Match
                {
                    Phase = "group",
                    Group = group.Key,
                    MatchDay = day + 1,
                    HomeTeamId = groupTeams[h2].Id,
                    AwayTeamId = groupTeams[a2].Id,
                    MatchDate = baseDate.AddDays(dayOffset + day * 3 + 1),
                    Venue = groupVenues[1]
                });
            }
            dayOffset += 1;
        }

        db.Matches.AddRange(matches);
        await db.SaveChangesAsync();

        // Seed Round of 32 placeholder matches (FIFA 2026 format)
        // 12 groups: top 2 from each = 24 + 8 best 3rd place = 32 teams
        var r32Slots = new[]
        {
            "1A vs 2B", "1C vs 2D", "1E vs 2F", "1G vs 2H",
            "1I vs 2J", "1K vs 2L", "2A vs 2C", "2E vs 2G",
            "2I vs 2K", "1B vs 3rd(ACDL)", "1D vs 3rd(BEFK)", "1F vs 3rd(CGIJ)",
            "1H vs 3rd(ADHL)", "1J vs 3rd(BEFK)", "1L vs 3rd(GHIJ)", "3rd(remaining) vs 3rd(remaining)"
        };

        var knockoutDate = baseDate.AddDays(30);
        foreach (var slot in r32Slots)
        {
            matches.Add(new Match
            {
                Phase = "round_of_32",
                Group = "",
                MatchDay = 0,
                HomeTeamId = 1, // placeholder
                AwayTeamId = 2, // placeholder
                MatchDate = knockoutDate,
                Venue = "Por definir",
                SlotLabel = slot
            });
            knockoutDate = knockoutDate.AddDays(1);
        }

        db.Matches.AddRange(matches.Skip(matches.Count - r32Slots.Length));
        await db.SaveChangesAsync();
    }
}
