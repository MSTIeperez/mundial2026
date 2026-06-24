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
            new() { Name = "Corea del Sur", Code = "KOR", FlagEmoji = "🇰🇷", Group = "A" },
            new() { Name = "Chequia", Code = "CZE", FlagEmoji = "🇨🇿", Group = "A" },
            new() { Name = "Sudáfrica", Code = "RSA", FlagEmoji = "🇿🇦", Group = "A" },
            // Group B
            new() { Name = "Canadá", Code = "CAN", FlagEmoji = "🇨🇦", Group = "B" },
            new() { Name = "Suiza", Code = "SUI", FlagEmoji = "🇨🇭", Group = "B" },
            new() { Name = "Bosnia y Herzegovina", Code = "BIH", FlagEmoji = "🇧🇦", Group = "B" },
            new() { Name = "Catar", Code = "QAT", FlagEmoji = "🇶🇦", Group = "B" },
            // Group C
            new() { Name = "Brasil", Code = "BRA", FlagEmoji = "🇧🇷", Group = "C" },
            new() { Name = "Marruecos", Code = "MAR", FlagEmoji = "🇲🇦", Group = "C" },
            new() { Name = "Escocia", Code = "SCO", FlagEmoji = "SC", Group = "C" },
            new() { Name = "Haiti", Code = "hti", FlagEmoji = "H🇹", Group = "C" },
            // Group D
            new() { Name = "Estados Unidos", Code = "USA", FlagEmoji = "🇺🇸", Group = "D" },
            new() { Name = "Australia", Code = "AUS", FlagEmoji = "🇦🇺", Group = "D" },
            new() { Name = "Paraguay", Code = "PAR", FlagEmoji = "🇵🇾", Group = "D" },
            new() { Name = "Turquía", Code = "TUR", FlagEmoji = "🇹🇷", Group = "D" },
            // Group E
            new() { Name = "Alemania", Code = "GER", FlagEmoji = "🇩🇪", Group = "E" },
            new() { Name = "Costa de Marfil", Code = "CIV", FlagEmoji = "🇨🇮", Group = "E" },
            new() { Name = "Ecuador", Code = "ECU", FlagEmoji = "🇪🇨", Group = "E" },
            new() { Name = "Curazao", Code = "CUW", FlagEmoji = "🇨🇼", Group = "E" },
            // Group F
            new() { Name = "Paises Bajos", Code = "NED", FlagEmoji = "🇳🇱", Group = "F" },
            new() { Name = "Japón", Code = "JPN", FlagEmoji = "🇯🇵", Group = "F" },
            new() { Name = "Suecia", Code = "SWE", FlagEmoji = "🇸🇪", Group = "F" },
            new() { Name = "Tunéz", Code = "TUN", FlagEmoji = "🇹🇳", Group = "F" },
            // Group G
            new() { Name = "Egipto", Code = "EGY", FlagEmoji = "🇪🇬", Group = "G" },
            new() { Name = "Iran", Code = "IRN", FlagEmoji = "🇮🇷", Group = "G" },
            new() { Name = "Bélgica", Code = "BEL", FlagEmoji = "🇧🇪", Group = "G" },
            new() { Name = "Nueva Zelanda", Code = "NZL", FlagEmoji = "🇳🇿", Group = "G" },
            // Group H
            new() { Name = "España", Code = "ESP", FlagEmoji = "🇪🇸", Group = "H" },
            new() { Name = "Uruguay", Code = "URU", FlagEmoji = "🇺🇾", Group = "H" },
            new() { Name = "Cabo Verde", Code = "CPV", FlagEmoji = "🇨🇻", Group = "H" },
            new() { Name = "Arabia Saudita", Code = "KSA", FlagEmoji = "🇸🇦", Group = "H" },
            // Group I
            new() { Name = "Francia", Code = "FRA", FlagEmoji = "🇫🇷", Group = "I" },
            new() { Name = "Noruega", Code = "NOR", FlagEmoji = "🇳🇴", Group = "I" },
            new() { Name = "Senegal", Code = "SEN", FlagEmoji = "🇸🇳", Group = "I" },
            new() { Name = "Irak", Code = "IRQ", FlagEmoji = "🇮🇶", Group = "I" },
            // Group J
            new() { Name = "Argentina", Code = "ARG", FlagEmoji = "🇦🇷", Group = "J" },
            new() { Name = "Austria", Code = "AUT", FlagEmoji = "🇦🇹", Group = "J" },
            new() { Name = "Argelia", Code = "DZA", FlagEmoji = "🇩🇿", Group = "J" },
            new() { Name = "Jordania", Code = "JOR", FlagEmoji = "🇯🇴", Group = "J" },
            // Group K
            new() { Name = "Portugal", Code = "POR", FlagEmoji = "🇵🇹", Group = "K" },
            new() { Name = "Colombia", Code = "COL", FlagEmoji = "🇨🇴", Group = "K" },
            new() { Name = "RD Congo", Code = "COD", FlagEmoji = "🇨🇩", Group = "K" },
            new() { Name = "Uzbekistán", Code = "UZB", FlagEmoji = "🇺🇿", Group = "K" },
            // Group L
            new() { Name = "Inglaterra", Code = "ENG", FlagEmoji = "󠁧󠁢󠁥󠁮󠁧󠁿🇬🇧", Group = "L" },
            new() { Name = "Ghana", Code = "GHA", FlagEmoji = "🇬🇭", Group = "L" },
            new() { Name = "Croacia", Code = "CRO", FlagEmoji = "🇭🇷", Group = "L" },
            new() { Name = "Panamá", Code = "PAN", FlagEmoji = "🇵🇦", Group = "L" },
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
