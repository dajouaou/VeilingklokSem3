using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations;

public partial class Historie : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE INDEX IX_Biedingen_VeilingProductId_Tijdstip
            ON dbo.Biedingen (VeilingProductId, Tijdstip DESC)
            INCLUDE (Prijs);
        ");

        migrationBuilder.Sql(@"
            CREATE INDEX IX_Aanmeldingen_Soort_AanvoerderId
            ON dbo.Aanmeldingen (Soort, AanvoerderId)
            INCLUDE (Id);
        ");

        migrationBuilder.Sql(@"
            CREATE INDEX IX_VeilingProducten_Verkocht_AanmeldingId
            ON dbo.VeilingProducten (AanmeldingId)
            INCLUDE (Id)
            WHERE IsVerkocht = 1;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_Biedingen_VeilingProductId_Tijdstip ON dbo.Biedingen;");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_Aanmeldingen_Soort_AanvoerderId ON dbo.Aanmeldingen;");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_VeilingProducten_Verkocht_AanmeldingId ON dbo.VeilingProducten;");
    }
}
