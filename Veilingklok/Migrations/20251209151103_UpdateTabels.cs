using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Beschrijving",
                table: "Aanmeldingen",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Producten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AanvoerderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", nullable: false),
                    Categorie = table.Column<string>(type: "TEXT", nullable: true),
                    Beschrijving = table.Column<string>(type: "TEXT", nullable: true),
                    FotoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Soort = table.Column<string>(type: "TEXT", nullable: false),
                    PotmaatOfSteellengte = table.Column<string>(type: "TEXT", nullable: true),
                    HoeveelheidStuks = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumPrijs = table.Column<decimal>(type: "TEXT", nullable: false),
                    KlokLocatie = table.Column<string>(type: "TEXT", nullable: false),
                    VeilDatum = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producten_Aanvoerders_AanvoerderId",
                        column: x => x.AanvoerderId,
                        principalTable: "Aanvoerders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Producten_AanvoerderId",
                table: "Producten",
                column: "AanvoerderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Producten");

            migrationBuilder.DropColumn(
                name: "Beschrijving",
                table: "Aanmeldingen");
        }
    }
}
