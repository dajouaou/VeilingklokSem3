using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class AddLeverDatumToAanmelding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veilingmeesters_Gebruikers_GebruikerId",
                table: "Veilingmeesters");

            migrationBuilder.RenameColumn(
                name: "Veildatum",
                table: "Aanmeldingen",
                newName: "LeverDatum");

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingmeesters_Gebruikers_GebruikerId",
                table: "Veilingmeesters",
                column: "GebruikerId",
                principalTable: "Gebruikers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veilingmeesters_Gebruikers_GebruikerId",
                table: "Veilingmeesters");

            migrationBuilder.RenameColumn(
                name: "LeverDatum",
                table: "Aanmeldingen",
                newName: "Veildatum");

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingmeesters_Gebruikers_GebruikerId",
                table: "Veilingmeesters",
                column: "GebruikerId",
                principalTable: "Gebruikers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
