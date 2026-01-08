using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeilingId = table.Column<int>(type: "int", nullable: false),
                    VeilingProductId = table.Column<int>(type: "int", nullable: false),
                    KoperId = table.Column<int>(type: "int", nullable: true),
                    Aantal = table.Column<int>(type: "int", nullable: false),
                    Prijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tijdstip = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactie_Gebruikers_KoperId",
                        column: x => x.KoperId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactie_VeilingProducten_VeilingProductId",
                        column: x => x.VeilingProductId,
                        principalTable: "VeilingProducten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactie_KoperId",
                table: "Transactie",
                column: "KoperId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactie_VeilingProductId",
                table: "Transactie",
                column: "VeilingProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactie");
        }
    }
}
