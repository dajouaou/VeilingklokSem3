using Microsoft.Data.SqlClient;
using System.Data;
using Veilingklok.Features.PrijsHistorie.Dtos;

namespace Veilingklok.Features.PrijsHistorie.Services
{
    public interface IPrijsHistorieService
    {
        Task<PrijsHistorieDto> GetPrijsHistorieAsync(string soort, int? huidigeAanvoerderId);
    }

    public class PrijsHistorieService : IPrijsHistorieService
    {
        private readonly IConfiguration _config;

        public PrijsHistorieService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<PrijsHistorieDto> GetPrijsHistorieAsync(string soort, int? huidigeAanvoerderId)
        {
            var cs = _config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(cs))
                throw new Exception("ConnectionString 'DefaultConnection' ontbreekt.");

            var dto = new PrijsHistorieDto { Soort = soort };

            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();

            // 1) AVG + last10 (alle aanvoerders)
            dto.GemiddeldeAlleAanvoerders = await QueryAvgAsync(conn, soort, null);
            dto.Laatste10AlleAanvoerders = await QueryLast10Async(conn, soort, null);

            // 2) AVG + last10 (huidige aanvoerder)
            if (huidigeAanvoerderId.HasValue && huidigeAanvoerderId.Value > 0)
            {
                dto.GemiddeldeHuidigeAanvoerder = await QueryAvgAsync(conn, soort, huidigeAanvoerderId.Value);
                dto.Laatste10HuidigeAanvoerder = await QueryLast10Async(conn, soort, huidigeAanvoerderId.Value);
            }

            return dto;
        }

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

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@soort", SqlDbType.NVarChar, 200) { Value = soort });
            cmd.Parameters.Add(new SqlParameter("@aanvoerderId", SqlDbType.Int) { Value = (object?)aanvoerderId ?? DBNull.Value });

            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value) return null;

            return Convert.ToDecimal(result);
        }

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

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@soort", SqlDbType.NVarChar, 200) { Value = soort });
            cmd.Parameters.Add(new SqlParameter("@aanvoerderId", SqlDbType.Int) { Value = (object?)aanvoerderId ?? DBNull.Value });

            var list = new List<PrijsPuntDto>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PrijsPuntDto
                {
                    Prijs = reader.GetDecimal(0),
                    Tijdstip = reader.GetDateTime(1),
                    AanvoerderNaam = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }

            return list;
        }
    }
}
