using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class AddVeilingMeesterEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VeilingmeesterId",
                table: "Veilingen",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Veilingmeesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingmeesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veilingmeesters_Gebruikers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_VeilingmeesterId",
                table: "Veilingen",
                column: "VeilingmeesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingmeesters_GebruikerId",
                table: "Veilingmeesters",
                column: "GebruikerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingen_Veilingmeesters_VeilingmeesterId",
                table: "Veilingen",
                column: "VeilingmeesterId",
                principalTable: "Veilingmeesters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veilingen_Veilingmeesters_VeilingmeesterId",
                table: "Veilingen");

            migrationBuilder.DropTable(
                name: "Veilingmeesters");

            migrationBuilder.DropIndex(
                name: "IX_Veilingen_VeilingmeesterId",
                table: "Veilingen");

            migrationBuilder.DropColumn(
                name: "VeilingmeesterId",
                table: "Veilingen");
        }
    }
}
