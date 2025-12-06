using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruikers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Voornaam = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Achternaam = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aanvoerders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", nullable: false),
                    ContactInfo = table.Column<string>(type: "TEXT", nullable: true)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", nullable: false),
                    Saldo = table.Column<decimal>(type: "TEXT", nullable: false)
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
                name: "VMs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VMs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VMs_Gebruikers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VeilingId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActorGebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VeilingId = table.Column<int>(type: "INTEGER", nullable: false),
                    VeilingProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlacedByGebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    KoperId = table.Column<int>(type: "INTEGER", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    PlacedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biedingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Biedingen_Gebruikers_PlacedByGebruikerId",
                        column: x => x.PlacedByGebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VMId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTijdUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EindTijdUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Locatie = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentVeilingProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veilingen_VMs_VMId",
                        column: x => x.VMId,
                        principalTable: "VMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VeilingProducten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VeilingId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    AanvoerderId = table.Column<int>(type: "INTEGER", nullable: true),
                    Volgorde = table.Column<int>(type: "INTEGER", nullable: false),
                    Hoeveelheid = table.Column<int>(type: "INTEGER", nullable: false),
                    StartPrijs = table.Column<decimal>(type: "TEXT", nullable: false),
                    HuidigePrijs = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SoldToKoperId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeilingProducten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Aanvoerders_AanvoerderId",
                        column: x => x.AanvoerderId,
                        principalTable: "Aanvoerders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Kopers_SoldToKoperId",
                        column: x => x.SoldToKoperId,
                        principalTable: "Kopers",
                        principalColumn: "Id");
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
                column: "CurrentVeilingProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_VMId",
                table: "Veilingen",
                column: "VMId");

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
                name: "IX_VeilingProducten_VeilingId",
                table: "VeilingProducten",
                column: "VeilingId");

            migrationBuilder.CreateIndex(
                name: "IX_VMs_GebruikerId",
                table: "VMs",
                column: "GebruikerId",
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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Biedingen_Veilingen_VeilingId",
                table: "Biedingen",
                column: "VeilingId",
                principalTable: "Veilingen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingen_VeilingProducten_CurrentVeilingProductId",
                table: "Veilingen",
                column: "CurrentVeilingProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
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
                name: "FK_VMs_Gebruikers_GebruikerId",
                table: "VMs");

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
                name: "VMs");

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
