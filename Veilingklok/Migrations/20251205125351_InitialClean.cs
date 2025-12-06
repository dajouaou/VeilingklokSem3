using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class InitialClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aanmeldingen_VeilingProducten_VeilingProductId1",
                table: "Aanmeldingen");

            migrationBuilder.DropIndex(
                name: "IX_VeilingProducten_AanmeldingId",
                table: "VeilingProducten");

            migrationBuilder.DropIndex(
                name: "IX_Aanmeldingen_VeilingProductId1",
                table: "Aanmeldingen");

            migrationBuilder.DropColumn(
                name: "VeilingProductId1",
                table: "Aanmeldingen");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_AanmeldingId",
                table: "VeilingProducten",
                column: "AanmeldingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VeilingProducten_AanmeldingId",
                table: "VeilingProducten");

            migrationBuilder.AddColumn<int>(
                name: "VeilingProductId1",
                table: "Aanmeldingen",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_AanmeldingId",
                table: "VeilingProducten",
                column: "AanmeldingId");

            migrationBuilder.CreateIndex(
                name: "IX_Aanmeldingen_VeilingProductId1",
                table: "Aanmeldingen",
                column: "VeilingProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Aanmeldingen_VeilingProducten_VeilingProductId1",
                table: "Aanmeldingen",
                column: "VeilingProductId1",
                principalTable: "VeilingProducten",
                principalColumn: "Id");
        }
    }
}
