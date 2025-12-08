using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Voornaam = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Achternaam = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
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
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GebruikerId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AanvoerderId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PotmaatOfSteellengte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoeveelheidStuks = table.Column<int>(type: "int", nullable: false),
                    MinimumPrijs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KlokLocatie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VeilDatum = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VMId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartTijdUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EindTijdUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Locatie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentVeilingProductId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
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
                    ActivatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoldToKoperId = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "Id", "Achternaam", "CreatedAtUtc", "Email", "PasswordHash", "Role", "Username", "Voornaam" },
                values: new object[,]
                {
                    { 1, "Meester", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), "vm1@example.com", "dm0tc2VlZC1zYWx0LTAwMA==.KRqeXLGmj1DryTFBWCId0sZltd6nURyxHIqdjAmoXIo=", 3, "vm1", "Veiling" },
                    { 2, "Aanvoerder", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), "aanvoerder1@example.com", "YXYtc2VlZC1zYWx0LTAwMA==.tyfrGs1Ns/tRy5ChiEVBJlLHs3wqTLN3d24QNeJYzJs=", 2, "aanvoerder1", "Jan" },
                    { 3, "Koper", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), "koper1@example.com", "a3Atc2VlZC1zYWx0LTAwMA==.2ymdEbIJ9G+5O0/M/fT6jrS2ZRHYV3pzBK/2nW/BYco=", 1, "koper1", "Klaas" }
                });

            migrationBuilder.InsertData(
                table: "Aanvoerders",
                columns: new[] { "Id", "ContactInfo", "GebruikerId", "Naam" },
                values: new object[] { 1, "jan@aanvoerder.com", 2, "Jan Aanvoerder" });

            migrationBuilder.InsertData(
                table: "Kopers",
                columns: new[] { "Id", "GebruikerId", "Naam", "Saldo" },
                values: new object[] { 1, 3, "Klaas Koper", 500m });

            migrationBuilder.InsertData(
                table: "VMs",
                columns: new[] { "Id", "GebruikerId", "Naam" },
                values: new object[] { 1, 1, "Veiling Meester" });

            migrationBuilder.InsertData(
                table: "Producten",
                columns: new[] { "Id", "AanvoerderId", "Beschrijving", "Categorie", "FotoUrl", "HoeveelheidStuks", "KlokLocatie", "MinimumPrijs", "Naam", "PotmaatOfSteellengte", "Soort", "VeilDatum" },
                values: new object[,]
                {
                    { 1, 1, "Frisse mix alstroemeria's", "Bloemen", "/img/products/alstroemeria-mix.jpg", 50, "Aalsmeer", 4m, "Alstroemeria Mix", "60cm", "Bloem", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, "Dieprode anthurium", "Planten", "/img/products/anthurium-rood.jpg", 15, "Aalsmeer", 8m, "Anthurium Rood", "14cm pot", "Plant", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, "Vrolijke mix van gerbera's", "Boeket", "/img/products/boeket-gerbera-mix.jpg", 30, "Aalsmeer", 5m, "Boeket Gerbera Mix", "n.v.t.", "Boeket", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 1, "Luxe ranunculus-mix premium", "Boeket", "/img/products/boeket-ranunculus-mix-premium.jpg", 25, "Aalsmeer", 9m, "Boeket Ranunculus Mix Premium", "n.v.t.", "Boeket", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 1, "Oranje calla lelies", "Bloemen", "/img/products/calla-lily-oranje.jpg", 40, "Aalsmeer", 7m, "Calla Lily Oranje", "55cm", "Bloem", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 1, "Grote ficus lyrata kamerplant", "Planten", "/img/products/ficus-lyrata.jpg", 10, "Aalsmeer", 15m, "Ficus Lyrata", "24cm pot", "Plant", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 1, "Luchtige witte gypsophila", "Bloemen", "/img/products/gypsophila-paniculata-wit.jpg", 40, "Aalsmeer", 3m, "Gypsophila Paniculata Wit", "70cm", "Bloem", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 1, "Elegante witte orchidee", "Planten", "/img/products/orchidee-phalaenopsis-wit.jpg", 20, "Aalsmeer", 12m, "Orchidee Phalaenopsis Wit", "12cm pot", "Plant", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 1, "Sterke sansevieria zeylanica", "Planten", "/img/products/sansevieria-zeylanica.jpg", 25, "Aalsmeer", 10m, "Sansevieria Zeylanica", "17cm pot", "Plant", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 1, "Warm seizoensboeket in herfsttinten", "Boeket", "/img/products/seizoensboeket-herfst-mix.jpg", 30, "Aalsmeer", 14m, "Seizoensboeket Herfst Mix", "n.v.t.", "Boeket", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 1, "Dubbelbloemige tulpenmix", "Bloemen", "/img/products/tulpen-dubbelbloemig-mix.jpg", 50, "Aalsmeer", 6m, "Tulpen Dubbelbloemig Mix", "40cm", "Bloem", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 1, "Licht boeket met witte rozen en lisianthus", "Boeket", "/img/products/White-Rose-Lisianthus-Bouquet.jpg", 20, "Aalsmeer", 18m, "White Rose Lisianthus Boeket", "n.v.t.", "Boeket", new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Veilingen",
                columns: new[] { "Id", "CreatedAtUtc", "CurrentVeilingProductId", "EindTijdUtc", "Locatie", "Naam", "StartTijdUtc", "Status", "VMId" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Aalsmeer", "Ochtendveiling Aalsmeer", new DateTime(2025, 1, 1, 8, 10, 0, 0, DateTimeKind.Utc), 1, 1 });

            migrationBuilder.InsertData(
                table: "VeilingProducten",
                columns: new[] { "Id", "AanvoerderId", "ActivatedAtUtc", "ClosedAtUtc", "Hoeveelheid", "HuidigePrijs", "ProductId", "SoldToKoperId", "StartPrijs", "Status", "VeilingId", "Volgorde" },
                values: new object[,]
                {
                    { 1, 1, null, null, 50, 10m, 1, null, 10m, 0, 1, 1 },
                    { 2, 1, null, null, 15, 15m, 2, null, 15m, 0, 1, 2 },
                    { 3, 1, null, null, 30, 12m, 3, null, 12m, 0, 1, 3 },
                    { 4, 1, null, null, 25, 18m, 4, null, 18m, 0, 1, 4 },
                    { 5, 1, null, null, 40, 14m, 5, null, 14m, 0, 1, 5 },
                    { 6, 1, null, null, 10, 30m, 6, null, 30m, 0, 1, 6 },
                    { 7, 1, null, null, 40, 8m, 7, null, 8m, 0, 1, 7 },
                    { 8, 1, null, null, 20, 24m, 8, null, 24m, 0, 1, 8 },
                    { 9, 1, null, null, 25, 20m, 9, null, 20m, 0, 1, 9 },
                    { 10, 1, null, null, 30, 28m, 10, null, 28m, 0, 1, 10 },
                    { 11, 1, null, null, 50, 16m, 11, null, 16m, 0, 1, 11 },
                    { 12, 1, null, null, 20, 32m, 12, null, 32m, 0, 1, 12 }
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
                name: "IX_AuditEntries_VeilingId_CreatedAtUtc",
                table: "AuditEntries",
                columns: new[] { "VeilingId", "CreatedAtUtc" });

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
                name: "IX_Biedingen_VeilingProductId_PlacedAtUtc",
                table: "Biedingen",
                columns: new[] { "VeilingProductId", "PlacedAtUtc" });

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
                unique: true,
                filter: "[CurrentVeilingProductId] IS NOT NULL");

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
                name: "IX_VeilingProducten_VeilingId_Status_Volgorde",
                table: "VeilingProducten",
                columns: new[] { "VeilingId", "Status", "Volgorde" });

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
                onDelete: ReferentialAction.Restrict);
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
