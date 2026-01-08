using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class AddVeilingAfgeslotenOpUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactie_Gebruikers_KoperId",
                table: "Transactie");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactie_VeilingProducten_VeilingProductId",
                table: "Transactie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactie",
                table: "Transactie");

            migrationBuilder.RenameTable(
                name: "Transactie",
                newName: "Transacties");

            migrationBuilder.RenameIndex(
                name: "IX_Transactie_VeilingProductId",
                table: "Transacties",
                newName: "IX_Transacties_VeilingProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactie_KoperId",
                table: "Transacties",
                newName: "IX_Transacties_KoperId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transacties",
                table: "Transacties",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_Gebruikers_KoperId",
                table: "Transacties");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacties_VeilingProducten_VeilingProductId",
                table: "Transacties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transacties",
                table: "Transacties");

            migrationBuilder.RenameTable(
                name: "Transacties",
                newName: "Transactie");

            migrationBuilder.RenameIndex(
                name: "IX_Transacties_VeilingProductId",
                table: "Transactie",
                newName: "IX_Transactie_VeilingProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Transacties_KoperId",
                table: "Transactie",
                newName: "IX_Transactie_KoperId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactie",
                table: "Transactie",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactie_Gebruikers_KoperId",
                table: "Transactie",
                column: "KoperId",
                principalTable: "Gebruikers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactie_VeilingProducten_VeilingProductId",
                table: "Transactie",
                column: "VeilingProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
