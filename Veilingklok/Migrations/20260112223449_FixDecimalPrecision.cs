using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class FixDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Gebruikers_KoperId",
                table: "Transacties");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_VeilingProducten_VeilingProductId",
                table: "Transacties");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Gebruikers_KoperId",
                table: "Transacties",
                column: "KoperId",
                principalTable: "Gebruikers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_VeilingProducten_VeilingProductId",
                table: "Transacties",
                column: "VeilingProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Gebruikers_KoperId",
                table: "Transacties");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_VeilingProducten_VeilingProductId",
                table: "Transacties");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_Gebruikers_KoperId",
                table: "Transacties",
                column: "KoperId",
                principalTable: "Gebruikers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacties_VeilingProducten_VeilingProductId",
                table: "Transacties",
                column: "VeilingProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
