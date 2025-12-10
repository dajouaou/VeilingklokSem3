using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class FixEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PotmaatOfSteellengte",
                table: "Aanmeldingen",
                newName: "Steellengte");

            migrationBuilder.AddColumn<string>(
                name: "Potmaat",
                table: "Aanmeldingen",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Veildagen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Datum = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veildagen", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Veildagen");

            migrationBuilder.DropColumn(
                name: "Potmaat",
                table: "Aanmeldingen");

            migrationBuilder.RenameColumn(
                name: "Steellengte",
                table: "Aanmeldingen",
                newName: "PotmaatOfSteellengte");
        }
    }
}
