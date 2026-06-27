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

        // Equipos - El índice 0 de cada grupo representa a la cabeza de serie (A1, B1, C1...)
        var teams = new List<Team>
        {
            // Grupo A (México oficial)
            new() { Name = "México", Code = "MEX", FlagEmoji = "🇲🇽", Group = "A" },
            new() { Name = "Sudáfrica", Code = "RSA", FlagEmoji = "🇿🇦", Group = "A" },
            new() { Name = "Corea del Sur", Code = "KOR", FlagEmoji = "🇰🇷", Group = "A" },
            new() { Name = "Chequia", Code = "CZE", FlagEmoji = "🇨🇿", Group = "A" },
            // Grupo B (Canadá oficial)
            new() { Name = "Canadá", Code = "CAN", FlagEmoji = "🇨🇦", Group = "B" },
            new() { Name = "Bosnia y Herzegovina", Code = "BIH", FlagEmoji = "🇧🇦", Group = "B" },
            new() { Name = "Catar", Code = "QAT", FlagEmoji = "🇶🇦", Group = "B" },
            new() { Name = "Suiza", Code = "SUI", FlagEmoji = "🇨🇭", Group = "B" },
            // Grupo C
            new() { Name = "Brasil", Code = "BRA", FlagEmoji = "🇧🇷", Group = "C" },
            new() { Name = "Marruecos", Code = "MAR", FlagEmoji = "🇲🇦", Group = "C" },
            new() { Name = "Haití", Code = "HAI", FlagEmoji = "🇭🇹", Group = "C" },
            new() { Name = "Escocia", Code = "SCO", FlagEmoji = "🏴󠁧󠁢󠁳󠁣󠁴󠁿", Group = "C" },
            // Grupo D (Estados Unidos oficial)
            new() { Name = "Estados Unidos", Code = "USA", FlagEmoji = "🇺🇸", Group = "D" },
            new() { Name = "Paraguay", Code = "PAR", FlagEmoji = "🇵🇾", Group = "D" },
            new() { Name = "Australia", Code = "AUS", FlagEmoji = "🇦🇺", Group = "D" },
            new() { Name = "Turquía", Code = "TUR", FlagEmoji = "🇹🇷", Group = "D" },
            // Grupo E
            new() { Name = "Alemania", Code = "GER", FlagEmoji = "🇩🇪", Group = "E" },
            new() { Name = "Curazao", Code = "CUW", FlagEmoji = "🇨🇼", Group = "E" },
            new() { Name = "Costa de Marfil", Code = "CIV", FlagEmoji = "🇨🇮", Group = "E" },
            new() { Name = "Ecuador", Code = "ECU", FlagEmoji = "🇪🇨", Group = "E" },
            // Grupo F
            new() { Name = "Países Bajos", Code = "NED", FlagEmoji = "🇳🇱", Group = "F" },
            new() { Name = "Japón", Code = "JPN", FlagEmoji = "🇯🇵", Group = "F" },
            new() { Name = "Suecia", Code = "SWE", FlagEmoji = "🇸🇪", Group = "F" },
            new() { Name = "Túnez", Code = "TUN", FlagEmoji = "🇹🇳", Group = "F" },
            // Grupo G
            new() { Name = "Bélgica", Code = "BEL", FlagEmoji = "🇧🇪", Group = "G" },
            new() { Name = "Egipto", Code = "EGY", FlagEmoji = "🇪🇬", Group = "G" },
            new() { Name = "Irán", Code = "IRN", FlagEmoji = "🇮🇷", Group = "G" },
            new() { Name = "Nueva Zelanda", Code = "NZL", FlagEmoji = "🇳🇿", Group = "G" },
            // Grupo H
            new() { Name = "España", Code = "ESP", FlagEmoji = "🇪🇸", Group = "H" },
            new() { Name = "Cabo Verde", Code = "CPV", FlagEmoji = "🇨🇻", Group = "H" },
            new() { Name = "Arabia Saudita", Code = "KSA", FlagEmoji = "🇸🇦", Group = "H" },
            new() { Name = "Uruguay", Code = "URU", FlagEmoji = "🇺🇾", Group = "H" },
            // Grupo I
            new() { Name = "Francia", Code = "FRA", FlagEmoji = "🇫🇷", Group = "I" },
            new() { Name = "Senegal", Code = "SEN", FlagEmoji = "🇸🇳", Group = "I" },
            new() { Name = "Irak", Code = "IRQ", FlagEmoji = "🇮🇶", Group = "I" },
            new() { Name = "Noruega", Code = "NOR", FlagEmoji = "🇳🇴", Group = "I" },
            // Grupo J
            new() { Name = "Argentina", Code = "ARG", FlagEmoji = "🇦🇷", Group = "J" },
            new() { Name = "Argelia", Code = "ALG", FlagEmoji = "🇩🇿", Group = "J" },
            new() { Name = "Austria", Code = "AUT", FlagEmoji = "🇦🇹", Group = "J" },
            new() { Name = "Jordania", Code = "JOR", FlagEmoji = "🇯🇴", Group = "J" },
            // Grupo K
            new() { Name = "Portugal", Code = "POR", FlagEmoji = "🇵🇹", Group = "K" },
            new() { Name = "RD Congo", Code = "COD", FlagEmoji = "🇨🇩", Group = "K" },
            new() { Name = "Uzbekistán", Code = "UZB", FlagEmoji = "🇺🇿", Group = "K" },
            new() { Name = "Colombia", Code = "COL", FlagEmoji = "🇨🇴", Group = "K" },
            // Grupo L
            new() { Name = "Inglaterra", Code = "ENG", FlagEmoji = "🏴󠁧󠁢󠁥󠁮󠁧󠁿", Group = "L" },
            new() { Name = "Croacia", Code = "CRO", FlagEmoji = "🇭🇷", Group = "L" },
            new() { Name = "Ghana", Code = "GHA", FlagEmoji = "🇬🇭", Group = "L" },
            new() { Name = "Panamá", Code = "PAN", FlagEmoji = "🇵🇦", Group = "L" },
        };

        db.Teams.AddRange(teams);
        await db.SaveChangesAsync();

        var matches = new List<Match>();

        // Función auxiliar para registrar la agenda real de FIFA 2026
        void AddGroupMatches(string groupKey, (int Day, int HomeIdx, int AwayIdx, DateTime Date, string Venue)[] schedule)
        {
            var groupTeams = teams.Where(t => t.Group == groupKey).ToList();
            foreach (var match in schedule)
            {
                matches.Add(new Match
                {
                    Phase = "group",
                    Group = groupKey,
                    MatchDay = match.Day,
                    HomeTeamId = groupTeams[match.HomeIdx].Id,
                    AwayTeamId = groupTeams[match.AwayIdx].Id,
                    MatchDate = match.Date,
                    Venue = match.Venue,
                    SlotLabel = $"Match {matches.Count + 1}" // Etiqueta de partido secuencial
                });
            }
        }

        // ==========================================
        // AGENDA OFICIAL FIFA 2026 (72 PARTIDOS FASE DE GRUPOS)
        // Estadios, fechas y cruces (Local vs Visitante) 100% reales
        // ==========================================

        AddGroupMatches("A", [
            (1, 0, 1, new DateTime(2026, 6, 11, 13, 0, 0), "Estadio Ciudad de México, CDMX"),
            (1, 2, 3, new DateTime(2026, 6, 11, 20, 0, 0), "Estadio Guadalajara"),
            (2, 3, 1, new DateTime(2026, 6, 18, 10, 0, 0), "Estadio Atlanta, Atlanta"),
            (2, 0, 2, new DateTime(2026, 6, 18, 19, 0, 0), "Estadio Guadalajara"),
            (3, 1, 2, new DateTime(2026, 6, 24, 19, 0, 0), "Estadio Monterrey, Nuevo León"),      // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 24, 19, 0, 0), "Estadio Ciudad de México, CDMX") // Simultáneo
        ]);

        AddGroupMatches("B", [
            (1, 0, 1, new DateTime(2026, 6, 12, 13, 0, 0), "BMO Field, Toronto"),
            (1, 2, 3, new DateTime(2026, 6, 13, 13, 0, 0), "Levi's Stadium, San Francisco"),
            (2, 3, 1, new DateTime(2026, 6, 18, 13, 0, 0), "Estadio Los Angeles, Los Ángeles"),
            (2, 0, 2, new DateTime(2026, 6, 18, 16, 0, 0), "BC Place, Vancouver"),
            (3, 3, 0, new DateTime(2026, 6, 24, 13, 0, 0), "BC Place, Vancouver"),  // Simultáneo
            (3, 1, 2, new DateTime(2026, 6, 24, 13, 0, 0), "Lumen Field, Seattle")  // Simultáneo
        ]);

        AddGroupMatches("C", [
            (1, 0, 1, new DateTime(2026, 6, 13, 16, 0, 0), "MetLife Stadium, New York/New Jersey"),
            (1, 2, 3, new DateTime(2026, 6, 13, 19, 0, 0), "Gillette Stadium, Boston"),
            (2, 3, 1, new DateTime(2026, 6, 19, 16, 0, 0), "Gillette Stadium, Boston"),
            (2, 0, 2, new DateTime(2026, 6, 19, 18, 30, 0), "Lincoln Financial Field, Philadelphia"),
            (3, 3, 0, new DateTime(2026, 6, 24, 16, 0, 0), "Hard Rock Stadium, Miami"), // Simultáneo
            (3, 1, 2, new DateTime(2026, 6, 24, 16, 0, 0), "Estadio Atlanta, Atlanta") // Simultáneo
        ]);

        AddGroupMatches("D", [
            (1, 0, 1, new DateTime(2026, 6, 12, 17, 0, 0), "SoFi Stadium, Los Angeles"),
            (1, 2, 3, new DateTime(2026, 6, 13, 22, 0, 0), "BC Place, Vancouver"),
            (2, 0, 2, new DateTime(2026, 6, 19, 13, 0, 0), "Lumen Field, Seattle"),
            (2, 3, 1, new DateTime(2026, 6, 19, 21, 0, 0), "Levi's Stadium, San Francisco"),
            (3, 3, 0, new DateTime(2026, 6, 25, 20, 0, 0), "SoFi Stadium, Los Angeles"), // Simultáneo
            (3, 1, 2, new DateTime(2026, 6, 25, 20, 0, 0), "Levi's Stadium, San Francisco") // Simultáneo
        ]);

        AddGroupMatches("E", [
            (1, 0, 1, new DateTime(2026, 6, 14, 11, 0, 0), "NRG Stadium, Houston"),
            (1, 2, 3, new DateTime(2026, 6, 14, 17, 0, 0), "Lincoln Financial Field, Philadelphia"),
            (2, 0, 2, new DateTime(2026, 6, 20, 14, 0, 0), "BMO Field, Toronto"),
            (2, 3, 1, new DateTime(2026, 6, 20, 18, 0, 0), "Arrowhead Stadium, Kansas City"),
            (3, 1, 2, new DateTime(2026, 6, 25, 14, 0, 0), "Lincoln Financial Field, Philadelphia"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 25, 14, 0, 0), "New Meadowlands Stadium, New York/New Jersey") // Simultáneo
        ]);

        AddGroupMatches("F", [
            (1, 0, 1, new DateTime(2026, 6, 14, 14, 0, 0), "AT&T Stadium, Dallas"),
            (1, 2, 3, new DateTime(2026, 6, 15, 20, 0, 0), "Estadio Monterrey, Nuevo León"),
            (2, 0, 2, new DateTime(2026, 6, 20, 11, 0, 0), "NRG Stadium, Houston"),
            (2, 3, 1, new DateTime(2026, 6, 20, 22, 0, 0), "Estadio Monterrey, Nuevo León"),
            (3, 1, 2, new DateTime(2026, 6, 25, 17, 0, 0), "Arrowhead Stadium, Kansas City"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 25, 17, 0, 0), "AT&T Stadium, Dallas") // Simultáneo
        ]);

        AddGroupMatches("G", [
            (1, 0, 1, new DateTime(2026, 6, 15, 13, 0, 0), "Lumen Field, Seattle"),
            (1, 2, 3, new DateTime(2026, 6, 15, 19, 0, 0), "SoFi Stadium, Los Angeles"),
            (2, 0, 2, new DateTime(2026, 6, 21, 13, 0, 0), "SoFi Stadium, Los Angeles"),
            (2, 3, 1, new DateTime(2026, 6, 21, 19, 0, 0), "BC Place, Vancouver"),
            (3, 1, 2, new DateTime(2026, 6, 26, 21, 0, 0), "Lumen Field, Seattle"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 26, 21, 0, 0), "BC Place, Vancouver") // Simultáneo
        ]);

        AddGroupMatches("H", [
            (1, 0, 1, new DateTime(2026, 6, 15, 10, 0, 0), "Mercedes-Benz Stadium, Atlanta"),
            (1, 2, 3, new DateTime(2026, 6, 16, 16, 0, 0), "Hard Rock Stadium, Miami"),
            (2, 0, 2, new DateTime(2026, 6, 21, 10, 0, 0), "Mercedes-Benz Stadium, Atlanta"),
            (2, 3, 1, new DateTime(2026, 6, 21, 16, 0, 0), "Hard Rock Stadium, Miami"),
            (3, 1, 2, new DateTime(2026, 6, 26, 18, 0, 0), "NRG Stadium, Houston"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 26, 18, 0, 0), "Estadio Guadalajara, Guadalajara") // Simultáneo
        ]);

        AddGroupMatches("I", [
            (1, 0, 1, new DateTime(2026, 6, 16, 13, 0, 0), "New Meadowlands Stadium, New York/New Jersey"),
            (1, 2, 3, new DateTime(2026, 6, 16, 16, 0, 0), "Gillette Stadium, Boston"),
            (2, 0, 2, new DateTime(2026, 6, 22, 15, 0, 0), "Lincoln Financial Field, Philadelphia"),
            (2, 3, 1, new DateTime(2026, 6, 22, 18, 0, 0), "New Meadowlands Stadium, New York/New Jersey"),
            (3, 1, 2, new DateTime(2026, 6, 26, 13, 0, 0), "BMO Field, Toronto"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 26, 13, 0, 0), "Gillette Stadium, Boston") // Simultáneo
        ]);

        AddGroupMatches("J", [
            (1, 0, 1, new DateTime(2026, 6, 16, 19, 0, 0), "Arrowhead Stadium, Kansas City"),
            (1, 2, 3, new DateTime(2026, 6, 16, 22, 0, 0), "Levi's Stadium, San Francisco"),
            (2, 0, 2, new DateTime(2026, 6, 22, 11, 0, 0), "AT&T Stadium, Dallas"),
            (2, 3, 1, new DateTime(2026, 6, 22, 21, 0, 0), "Levi's Stadium, San Francisco"),
            (3, 1, 2, new DateTime(2026, 6, 27, 20, 0, 0), "Arrowhead Stadium, Kansas City"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 27, 20, 0, 0), "AT&T Stadium, Dallas") // Simultáneo
        ]);

        AddGroupMatches("K", [
            (1, 0, 1, new DateTime(2026, 6, 17, 11, 0, 0), "NRG Stadium, Houston"),
            (1, 2, 3, new DateTime(2026, 6, 17, 20, 0, 0), "Estadio Ciudad de México, CDMX"),
            (2, 0, 2, new DateTime(2026, 6, 23, 11, 0, 0), "NRG Stadium, Houston"),
            (2, 3, 1, new DateTime(2026, 6, 23, 20, 0, 0), "Estadio Guadalajara, Guadalajara"),
            (3, 1, 2, new DateTime(2026, 6, 27, 17, 30, 0), "Mercedes-Benz Stadium, Atlanta"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 27, 17, 30, 0), "Hard Rock Stadium, Miami") // Simultáneo
        ]);

        AddGroupMatches("L", [
            (1, 0, 1, new DateTime(2026, 6, 17, 14, 0, 0), "AT&T Stadium, Dallas"),
            (1, 2, 3, new DateTime(2026, 6, 18, 17, 0, 0), "BMO Field, Toronto"),
            (2, 0, 2, new DateTime(2026, 6, 23, 14, 0, 0), "Gillette Stadium, Boston"),
            (2, 3, 1, new DateTime(2026, 6, 23, 17, 0, 0), "BMO Field, Toronto"),
            (3, 1, 2, new DateTime(2026, 6, 27, 15, 0, 0), "Lincoln Financial Field, Philadelphias"), // Simultáneo
            (3, 3, 0, new DateTime(2026, 6, 27, 15, 0, 0), "New Meadowlands Stadium, New York/New Jersey") // Simultáneo
        ]);

        //db.Matches.AddRange(matches);
        //await db.SaveChangesAsync();

    // ===============================================================
    // Ronda de 32 (Fase Eliminatoria) - Generación de placeholders FIFA 2026
        // Matches 73-88 * 28 jun - 3 jul 2026
    // REGLAS FIFA:
    //  12 ganadores de grupo + 12 subcampeones + 8 mejores 3eros = 32 equipos
    //  4 cruces 1° vs 2° de grupos diferentes
    //  8 cruces 1° vs mejor 3° lugar (posibles grupos definidos por FIFA)
    //  4 cruces 2° vs mejor 2° de grupos diferentes
    // ===============================================================
    
       var knockoutSchedule = new (int MatchNumber, string Phase, string SlotLabel, DateTime Date, string Venue)[]
        {
            // --- 16vos de Final (Round of 32) ---
            (73, "round_of_32", "2A vs 2B", new DateTime(2026, 6, 28, 14, 0, 0), "SoFi Stadium, Los Angeles"),
            (74, "round_of_32", "1E vs 3°(A/B/C/D/F)", new DateTime(2026, 6, 29, 15,30, 0), "Gillette Stadium, Boston"),
            (75, "round_of_32", "1F vs 3°(A/B/C/D/E)", new DateTime(2026, 6, 29, 20, 0, 0), "Estadio BBVA, Monterrey"),
            (76, "round_of_32", "1C vs 3°(A/B/D/E/F)", new DateTime(2026, 6, 29, 12, 0, 0), "NRG Stadium, Houston"),
            (77, "round_of_32", "1I vs 3°(C/D/E/F/G)", new DateTime(2026, 6, 30, 16, 0, 0), "MetLife Stadium, New York/New Jersey"),
            (78, "round_of_32", "2E vs 2F", new DateTime(2026, 6, 30, 12, 0, 0), "AT&T Stadium, Dallas"),
            (79, "round_of_32", "1A vs 3°(C/E/F/H/I)", new DateTime(2026, 6, 30, 20, 0, 0), "Estadio Azteca, Mexico City"),
            (80, "round_of_32", "1L vs 3°(E/H/I/J/K)", new DateTime(2026, 7, 1, 11, 0, 0), "Mercedes-Benz Stadium, Atlanta"),
            (81, "round_of_32", "1D vs 3°(B/E/F/I/J)", new DateTime(2026, 7, 1, 19, 0, 0), "Levi's Stadium, San Francisco"),
            (82, "round_of_32", "2G vs 2H", new DateTime(2026, 7, 1, 15, 0, 0), "Lumen Field, Seattle"),
            (83, "round_of_32", "1G vs 3°(A/E/H/I/J)", new DateTime(2026, 7, 2, 14, 0, 0), "SoFi Stadium, Los Angeles"),
            (84, "round_of_32", "1B vs 3°(E/F/G/I/J)", new DateTime(2026, 7, 2, 22, 0, 0), "BC Place, Vancouver"),
            (85, "round_of_32", "1J vs 3°(A/B/F/G/H)", new DateTime(2026, 7, 2, 18, 0, 0), "BMO Field, Toronto"),
            (86, "round_of_32", "1H vs 3°(B/C/E/F/G)", new DateTime(2026, 7, 3, 17, 0, 0), "Hard Rock Stadium, Miami"),
            (87, "round_of_32", "1K vs 3°(D/E/I/J/L)", new DateTime(2026, 7, 3, 20, 30, 0), "Arrowhead Stadium, Kansas City"),
            (88, "round_of_32", "2I vs 2J", new DateTime(2026, 7, 3, 13, 0, 0), "AT&T Stadium, Dallas"),

            // --- Octavos de Final (Round of 16) ---
            (89, "round_of_16", "Ganador 74 vs Ganador 77", new DateTime(2026, 7, 4, 16, 0, 0), "Lincoln Financial Field, Philadelphia"),
            (90, "round_of_16", "Ganador 73 vs Ganador 75", new DateTime(2026, 7, 4, 12, 0, 0), "NRG Stadium, Houston"),
            (91, "round_of_16", "Ganador 76 vs Ganador 78", new DateTime(2026, 7, 5, 15, 0, 0), "MetLife Stadium, New York/New Jersey"),
            (92, "round_of_16", "Ganador 79 vs Ganador 80", new DateTime(2026, 7, 5, 19, 0, 0), "Estadio Azteca, Mexico City"),
            (93, "round_of_16", "Ganador 83 vs Ganador 84", new DateTime(2026, 7, 6, 14, 0, 0), "AT&T Stadium, Dallas"),
            (94, "round_of_16", "Ganador 81 vs Ganador 82", new DateTime(2026, 7, 6, 19, 0, 0), "Lumen Field, Seattle"),
            (95, "round_of_16", "Ganador 86 vs Ganador 88", new DateTime(2026, 7, 7, 11, 0, 0), "Mercedes-Benz Stadium, Atlanta"),
            (96, "round_of_16", "Ganador 85 vs Ganador 87", new DateTime(2026, 7, 7, 15, 0, 0), "BC Place, Vancouver"),

            // --- Cuartos de Final (Quarterfinals) ---
            (97, "quarterfinals", "Ganador 89 vs Ganador 90", new DateTime(2026, 7, 9, 15, 0, 0), "Gillette Stadium, Boston"),
            (98, "quarterfinals", "Ganador 93 vs Ganador 94", new DateTime(2026, 7, 10, 14, 0, 0), "SoFi Stadium, Los Angeles"),
            (99, "quarterfinals", "Ganador 91 vs Ganador 92", new DateTime(2026, 7, 11, 16, 0, 0), "Hard Rock Stadium, Miami"),
            (100, "quarterfinals", "Ganador 95 vs Ganador 96", new DateTime(2026, 7, 11, 20, 0, 0), "Arrowhead Stadium, Kansas City"),

            // --- Semifinales (Semifinals) ---
            (101, "semifinals", "Ganador 97 vs Ganador 98", new DateTime(2026, 7, 14, 14, 0, 0), "AT&T Stadium, Dallas"),
            (102, "semifinals", "Ganador 99 vs Ganador 100", new DateTime(2026, 7, 15, 14, 0, 0), "Mercedes-Benz Stadium, Atlanta"),

            // --- Partido por el Tercer Lugar (Third Place) ---
            (103, "third_place", "Perdedor 101 vs Perdedor 102", new DateTime(2026, 7, 18, 16, 0, 0), "Hard Rock Stadium, Miami"),

            // --- Gran Final (Final) ---
            (104, "final", "Ganador 101 vs Ganador 102", new DateTime(2026, 7, 19, 14, 0, 0), "MetLife Stadium, New York/New Jersey")
        };
        
        foreach (var km in knockoutSchedule)
        {
            matches.Add(new Match
            {
                Phase = km.Phase,
                Group = "",
                MatchDay = km.MatchNumber, // Placeholder para el día del partido
                HomeTeamId = null, // Placeholder para el equipo local
                AwayTeamId = null, // Placeholder para el equipo visitante
                MatchDate = km.Date,
                Venue = km.Venue,
                SlotLabel = km.SlotLabel,

            });
        }

        var orderedMatches = matches.OrderBy(m => m.MatchDate).ToList();

        // 2. Asignamos el número de partido (MatchNumber) del 1 al 104
        for (int i = 0; i < orderedMatches.Count; i++)
        {
            orderedMatches[i].MatchNumber = i + 1;
        }

        db.Matches.AddRange(orderedMatches);
        await db.SaveChangesAsync();
    }
}