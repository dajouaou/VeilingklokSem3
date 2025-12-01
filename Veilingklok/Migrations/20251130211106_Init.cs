using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruikers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aanvoerders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GebruikerId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aanvoerders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aanvoerders_Gebruikers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kopers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GebruikerId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kopers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kopers_Gebruikers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Producten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AanvoerderId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Beschrijving = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    FotoUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producten_Aanvoerders_AanvoerderId",
                        column: x => x.AanvoerderId,
                        principalTable: "Aanvoerders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeilingId = table.Column<int>(type: "int", nullable: false),
                    ActorGebruikerId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditEntries_Gebruikers_ActorGebruikerId",
                        column: x => x.ActorGebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Biedingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeilingId = table.Column<int>(type: "int", nullable: false),
                    VeilingProductId = table.Column<int>(type: "int", nullable: false),
                    PlacedByGebruikerId = table.Column<int>(type: "int", nullable: false),
                    KoperId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    PlacedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biedingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Biedingen_Gebruikers_PlacedByGebruikerId",
                        column: x => x.PlacedByGebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Biedingen_Kopers_KoperId",
                        column: x => x.KoperId,
                        principalTable: "Kopers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Veilingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartTijdUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EindTijdUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentVeilingProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VeilingProducten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeilingId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AanvoerderId = table.Column<int>(type: "int", nullable: true),
                    Volgorde = table.Column<int>(type: "int", nullable: false),
                    Hoeveelheid = table.Column<int>(type: "int", nullable: false),
                    StartPrijs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HuidigePrijs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SoldToKoperId = table.Column<int>(type: "int", nullable: true),
                    ActivatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeilingProducten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Aanvoerders_AanvoerderId",
                        column: x => x.AanvoerderId,
                        principalTable: "Aanvoerders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Kopers_SoldToKoperId",
                        column: x => x.SoldToKoperId,
                        principalTable: "Kopers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Producten_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Producten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Veilingen_VeilingId",
                        column: x => x.VeilingId,
                        principalTable: "Veilingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aanvoerders_GebruikerId",
                table: "Aanvoerders",
                column: "GebruikerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_ActorGebruikerId",
                table: "AuditEntries",
                column: "ActorGebruikerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_VeilingId",
                table: "AuditEntries",
                column: "VeilingId");

            migrationBuilder.CreateIndex(
                name: "IX_Biedingen_KoperId",
                table: "Biedingen",
                column: "KoperId");

            migrationBuilder.CreateIndex(
                name: "IX_Biedingen_PlacedByGebruikerId",
                table: "Biedingen",
                column: "PlacedByGebruikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Biedingen_VeilingId",
                table: "Biedingen",
                column: "VeilingId");

            migrationBuilder.CreateIndex(
                name: "IX_Biedingen_VeilingProductId",
                table: "Biedingen",
                column: "VeilingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Kopers_GebruikerId",
                table: "Kopers",
                column: "GebruikerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producten_AanvoerderId",
                table: "Producten",
                column: "AanvoerderId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_CurrentVeilingProductId",
                table: "Veilingen",
                column: "CurrentVeilingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_AanvoerderId",
                table: "VeilingProducten",
                column: "AanvoerderId");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_ProductId",
                table: "VeilingProducten",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_SoldToKoperId",
                table: "VeilingProducten",
                column: "SoldToKoperId");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_VeilingId_Volgorde",
                table: "VeilingProducten",
                columns: new[] { "VeilingId", "Volgorde" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditEntries_Veilingen_VeilingId",
                table: "AuditEntries",
                column: "VeilingId",
                principalTable: "Veilingen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Biedingen_VeilingProducten_VeilingProductId",
                table: "Biedingen",
                column: "VeilingProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Biedingen_Veilingen_VeilingId",
                table: "Biedingen",
                column: "VeilingId",
                principalTable: "Veilingen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingen_VeilingProducten_CurrentVeilingProductId",
                table: "Veilingen",
                column: "CurrentVeilingProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aanvoerders_Gebruikers_GebruikerId",
                table: "Aanvoerders");

            migrationBuilder.DropForeignKey(
                name: "FK_Kopers_Gebruikers_GebruikerId",
                table: "Kopers");

            migrationBuilder.DropForeignKey(
                name: "FK_VeilingProducten_Veilingen_VeilingId",
                table: "VeilingProducten");

            migrationBuilder.DropTable(
                name: "AuditEntries");

            migrationBuilder.DropTable(
                name: "Biedingen");

            migrationBuilder.DropTable(
                name: "Gebruikers");

            migrationBuilder.DropTable(
                name: "Veilingen");

            migrationBuilder.DropTable(
                name: "VeilingProducten");

            migrationBuilder.DropTable(
                name: "Kopers");

            migrationBuilder.DropTable(
                name: "Producten");

            migrationBuilder.DropTable(
                name: "Aanvoerders");
        }
    }
}
