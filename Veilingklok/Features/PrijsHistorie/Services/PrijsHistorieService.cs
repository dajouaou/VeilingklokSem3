using Microsoft.Data.SqlClient;
using System.Data;
using Veilingklok.Features.PrijsHistorie.Dtos;

namespace Veilingklok.Features.PrijsHistorie.Services
{
    // Interface voor het ophalen van prijshistorie
    // Wordt gebruikt door controllers en maakt testen/mocken mogelijk
    public interface IPrijsHistorieService
    {
        Task<PrijsHistorieDto> GetPrijsHistorieAsync(string soort, int? huidigeAanvoerderId);
    }

    // Service die prijshistorie ophaalt via directe SQL queries
    // Wordt gebruikt voor rapportage en grafieken
    public class PrijsHistorieService : IPrijsHistorieService
    {
        // Configuratie wordt gebruikt om de connectionstring op te halen
        private readonly IConfiguration _config;

        // Constructor met dependency injection van IConfiguration
        public PrijsHistorieService(IConfiguration config)
        {
            _config = config;
        }

        // Haalt prijshistorie op voor een bepaalde soort
        // Optioneel kan gefilterd worden op een specifieke aanvoerder
        public async Task<PrijsHistorieDto> GetPrijsHistorieAsync(string soort, int? huidigeAanvoerderId)
        {
            // Probeer eerst Azure connectionstring, val terug op DefaultConnection
            var cs =
                _config.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
                ?? _config.GetConnectionString("DefaultConnection");

            // Stop als er geen geldige connectionstring is
            if (string.IsNullOrWhiteSpace(cs))
                throw new Exception("Geen geldige connectionstring gevonden (AZURE_SQL_CONNECTIONSTRING/DefaultConnection).");

            // DTO voorbereiden met de gevraagde soort
            var dto = new PrijsHistorieDto { Soort = soort };

            // Databaseverbinding openen
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();

            // Gemiddelde prijs en laatste 10 prijzen voor alle aanvoerders
            dto.GemiddeldeAlleAanvoerders = await QueryAvgAsync(conn, soort, null);
            dto.Laatste10AlleAanvoerders = await QueryLast10Async(conn, soort, null);

            // Als een specifieke aanvoerder is meegegeven, ook die statistieken ophalen
            if (huidigeAanvoerderId.HasValue && huidigeAanvoerderId.Value > 0)
            {
                dto.GemiddeldeHuidigeAanvoerder =
                    await QueryAvgAsync(conn, soort, huidigeAanvoerderId.Value);

                dto.Laatste10HuidigeAanvoerder =
                    await QueryLast10Async(conn, soort, huidigeAanvoerderId.Value);
            }

            // Volledig gevulde DTO teruggeven
            return dto;
        }

        // Hulpmethode: berekent de gemiddelde prijs
        // Kan gefilterd worden op soort en optioneel op aanvoerder
        private static async Task<decimal?> QueryAvgAsync(SqlConnection conn, string soort, int? aanvoerderId)
        {
            var sql = @"
SELECT AVG(CAST(b.Prijs AS DECIMAL(18,2))) AS Gemiddelde
FROM Biedingen b
JOIN VeilingProducten vp ON vp.Id = b.VeilingProductId
JOIN Aanmeldingen a ON a.Id = vp.AanmeldingId
WHERE vp.IsVerkocht = 1
  AND a.Soort = @soort
  AND (@aanvoerderId IS NULL OR a.AanvoerderId = @aanvoerderId);
";

            // SQL command voorbereiden
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@soort", SqlDbType.NVarChar, 200) { Value = soort });
            cmd.Parameters.Add(new SqlParameter("@aanvoerderId", SqlDbType.Int)
            {
                Value = (object?)aanvoerderId ?? DBNull.Value
            });

            // Query uitvoeren en resultaat uitlezen
            var result = await cmd.ExecuteScalarAsync();

            // Geen resultaat betekent geen data
            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToDecimal(result);
        }

        // Hulpmethode: haalt de laatste 10 prijsrecords op
        // Inclusief tijdstip en naam van de aanvoerder
        private static async Task<List<PrijsPuntDto>> QueryLast10Async(SqlConnection conn, string soort, int? aanvoerderId)
        {
            var sql = @"
SELECT TOP (10)
    b.Prijs,
    b.Tijdstip,
    av.Naam AS AanvoerderNaam
FROM Biedingen b
JOIN VeilingProducten vp ON vp.Id = b.VeilingProductId
JOIN Aanmeldingen a ON a.Id = vp.AanmeldingId
JOIN Aanvoerders av ON av.Id = a.AanvoerderId
WHERE vp.IsVerkocht = 1
  AND a.Soort = @soort
  AND (@aanvoerderId IS NULL OR a.AanvoerderId = @aanvoerderId)
ORDER BY b.Tijdstip DESC;
";

            // SQL command voorbereiden
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@soort", SqlDbType.NVarChar, 200) { Value = soort });
            cmd.Parameters.Add(new SqlParameter("@aanvoerderId", SqlDbType.Int)
            {
                Value = (object?)aanvoerderId ?? DBNull.Value
            });

            var list = new List<PrijsPuntDto>();

            // Resultaten uitlezen en omzetten naar DTOs
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PrijsPuntDto
                {
                    Prijs = reader.GetDecimal(0),
                    Tijdstip = reader.GetDateTime(1),
                    AanvoerderNaam = reader.IsDBNull(2)
                        ? null
                        : reader.GetString(2)
                });
            }

            // Lijst met prijsdata teruggeven
            return list;
        }
    }
}
