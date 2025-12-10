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
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Voornaam = table.Column<string>(type: "TEXT", nullable: false),
                    Achternaam = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Aanvoerders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
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
                    Naam = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
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
                name: "Veilingmeesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GebruikerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Naam = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingmeesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veilingmeesters_Gebruikers_GebruikerId",
                        column: x => x.GebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aanmeldingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AanvoerderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Soort = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Potmaat = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Steellengte = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Hoeveelheid = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumPrijs = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    KlokLocatie = table.Column<int>(type: "INTEGER", nullable: false),
                    Veildatum = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FotoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    VeilingProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    Beschrijving = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aanmeldingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aanmeldingen_Aanvoerders_AanvoerderId",
                        column: x => x.AanvoerderId,
                        principalTable: "Aanvoerders",
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Biedingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VeilingId = table.Column<int>(type: "INTEGER", nullable: false),
                    VeilingProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    KoperId = table.Column<int>(type: "INTEGER", nullable: false),
                    Prijs = table.Column<decimal>(type: "TEXT", nullable: false),
                    Tijdstip = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biedingen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Biedingen_Kopers_KoperId",
                        column: x => x.KoperId,
                        principalTable: "Kopers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veilingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Datum = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTijd = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EindTijd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    HuidigProductId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VeilingProducten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VeilingId = table.Column<int>(type: "INTEGER", nullable: false),
                    AanmeldingId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartPrijs = table.Column<decimal>(type: "TEXT", nullable: false),
                    HuidigePrijs = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActief = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVerkocht = table.Column<bool>(type: "INTEGER", nullable: false),
                    Volgorde = table.Column<int>(type: "INTEGER", nullable: false),
                    KoperId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeilingProducten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Aanmeldingen_AanmeldingId",
                        column: x => x.AanmeldingId,
                        principalTable: "Aanmeldingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Kopers_KoperId",
                        column: x => x.KoperId,
                        principalTable: "Kopers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VeilingProducten_Veilingen_VeilingId",
                        column: x => x.VeilingId,
                        principalTable: "Veilingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aanmeldingen_AanvoerderId",
                table: "Aanmeldingen",
                column: "AanvoerderId");

            migrationBuilder.CreateIndex(
                name: "IX_Aanvoerders_GebruikerId",
                table: "Aanvoerders",
                column: "GebruikerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Biedingen_KoperId",
                table: "Biedingen",
                column: "KoperId");

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
                name: "IX_Veilingen_HuidigProductId",
                table: "Veilingen",
                column: "HuidigProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingmeesters_GebruikerId",
                table: "Veilingmeesters",
                column: "GebruikerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_AanmeldingId",
                table: "VeilingProducten",
                column: "AanmeldingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_KoperId",
                table: "VeilingProducten",
                column: "KoperId");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_VeilingId",
                table: "VeilingProducten",
                column: "VeilingId");

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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingen_VeilingProducten_HuidigProductId",
                table: "Veilingen",
                column: "HuidigProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aanmeldingen_Aanvoerders_AanvoerderId",
                table: "Aanmeldingen");

            migrationBuilder.DropForeignKey(
                name: "FK_Kopers_Gebruikers_GebruikerId",
                table: "Kopers");

            migrationBuilder.DropForeignKey(
                name: "FK_VeilingProducten_Kopers_KoperId",
                table: "VeilingProducten");

            migrationBuilder.DropForeignKey(
                name: "FK_Veilingen_VeilingProducten_HuidigProductId",
                table: "Veilingen");

            migrationBuilder.DropTable(
                name: "Biedingen");

            migrationBuilder.DropTable(
                name: "Producten");

            migrationBuilder.DropTable(
                name: "Veildagen");

            migrationBuilder.DropTable(
                name: "Veilingmeesters");

            migrationBuilder.DropTable(
                name: "Aanvoerders");

            migrationBuilder.DropTable(
                name: "Gebruikers");

            migrationBuilder.DropTable(
                name: "Kopers");

            migrationBuilder.DropTable(
                name: "VeilingProducten");

            migrationBuilder.DropTable(
                name: "Aanmeldingen");

            migrationBuilder.DropTable(
                name: "Veilingen");
        }
    }
}
