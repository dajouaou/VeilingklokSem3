using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producten_Aanvoerders_AanvoerderId",
                table: "Producten");

            migrationBuilder.DropForeignKey(
                name: "FK_Veilingen_VeilingProducten_HuidigProductId",
                table: "Veilingen");

            migrationBuilder.DropForeignKey(
                name: "FK_VeilingProducten_Kopers_KoperId",
                table: "VeilingProducten");

            migrationBuilder.DropTable(
                name: "Biedingen");

            migrationBuilder.DropIndex(
                name: "IX_VeilingProducten_VeilingId",
                table: "VeilingProducten");

            migrationBuilder.DropIndex(
                name: "IX_Aanmeldingen_AanvoerderId",
                table: "Aanmeldingen");

            migrationBuilder.DropColumn(
                name: "IsActief",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "IsVerkocht",
                table: "VeilingProducten");

            migrationBuilder.RenameColumn(
                name: "HuidigProductId",
                table: "Veilingen",
                newName: "VMId");

            migrationBuilder.RenameColumn(
                name: "EindTijd",
                table: "Veilingen",
                newName: "StartTijdUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Veilingen_HuidigProductId",
                table: "Veilingen",
                newName: "IX_Veilingen_VMId");

            migrationBuilder.AddColumn<int>(
                name: "AanvoerderId",
                table: "VeilingProducten",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedAtUtc",
                table: "VeilingProducten",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAtUtc",
                table: "VeilingProducten",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "VeilingProducten",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Hoeveelheid",
                table: "VeilingProducten",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPrijs",
                table: "VeilingProducten",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "VeilingProducten",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "VeilingProducten",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "VeilingProducten",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Veilingmeesters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<int>(
                name: "CurrentVeilingProductId",
                table: "Veilingen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EindTijdUtc",
                table: "Veilingen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Locatie",
                table: "Veilingen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Naam",
                table: "Veilingen",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Soort",
                table: "Producten",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PotmaatOfSteellengte",
                table: "Producten",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Producten",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "KlokLocatie",
                table: "Producten",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FotoUrl",
                table: "Producten",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categorie",
                table: "Producten",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Beschrijving",
                table: "Producten",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Kopers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Voornaam",
                table: "Gebruikers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Gebruikers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Gebruikers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Achternaam",
                table: "Gebruikers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Aanvoerders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Soort",
                table: "Aanmeldingen",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FotoUrl",
                table: "Aanmeldingen",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Beschrijving",
                table: "Aanmeldingen",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeilingId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActorGebruikerId = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_AuditEntries_Veilingen_VeilingId",
                        column: x => x.VeilingId,
                        principalTable: "Veilingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeilingId = table.Column<int>(type: "int", nullable: false),
                    VeilingProductId = table.Column<int>(type: "int", nullable: false),
                    KoperId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlacedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bids_Kopers_KoperId",
                        column: x => x.KoperId,
                        principalTable: "Kopers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bids_VeilingProducten_VeilingProductId",
                        column: x => x.VeilingProductId,
                        principalTable: "VeilingProducten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bids_Veilingen_VeilingId",
                        column: x => x.VeilingId,
                        principalTable: "Veilingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "Id", "Achternaam", "CreatedAtUtc", "Email", "PasswordHash", "Rol", "Voornaam" },
                values: new object[,]
                {
                    { 1, "1", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "koper1@email.com", "DEMO_HASH", 1, "Koper" },
                    { 2, "2", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "koper2@email.com", "DEMO_HASH", 1, "Koper" },
                    { 3, "1", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "vm1@email.com", "DEMO_HASH", 3, "VM" },
                    { 4, "2", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "vm2@email.com", "DEMO_HASH", 3, "VM" },
                    { 5, "1", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "aanvoerder1@email.com", "DEMO_HASH", 2, "Aanvoerder" },
                    { 6, "2", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "aanvoerder2@email.com", "DEMO_HASH", 2, "Aanvoerder" }
                });

            migrationBuilder.InsertData(
                table: "Veildagen",
                columns: new[] { "Id", "Datum" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Aanvoerders",
                columns: new[] { "Id", "GebruikerId", "Naam" },
                values: new object[,]
                {
                    { 1, 5, "Aanvoerder 1" },
                    { 2, 6, "Aanvoerder 2" }
                });

            migrationBuilder.InsertData(
                table: "Kopers",
                columns: new[] { "Id", "GebruikerId", "Naam" },
                values: new object[,]
                {
                    { 1, 1, "Koper 1 BV" },
                    { 2, 2, "Koper 2 BV" }
                });

            migrationBuilder.InsertData(
                table: "Veilingmeesters",
                columns: new[] { "Id", "GebruikerId", "Naam" },
                values: new object[,]
                {
                    { 1, 3, "Veilingmeester 1" },
                    { 2, 4, "Veilingmeester 2" }
                });

            migrationBuilder.InsertData(
                table: "Aanmeldingen",
                columns: new[] { "Id", "AanvoerderId", "Beschrijving", "FotoUrl", "Hoeveelheid", "KlokLocatie", "LeverDatum", "MinimumPrijs", "Potmaat", "Soort", "Steellengte", "VeilingProductId" },
                values: new object[,]
                {
                    { 1, 1, "Aanmelding A1-1", "/AIimg/ai_self_made_pic_1.jpg.png", 100, 1, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1.50m, null, "Roos", "60cm", null },
                    { 2, 1, "Aanmelding A1-2", "/AIimg/ai_self_made_pic_2.jpg.png", 80, 1, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1.20m, null, "Tulp", "40cm", null },
                    { 3, 2, "Aanmelding A2-1", "/AIimg/ai_self_made_pic_3.jpg.png", 60, 2, new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.00m, "12cm", "Orchidee", null, null },
                    { 4, 2, "Aanmelding A2-2", "/AIimg/ai_self_made_pic_4.jpg.png", 50, 2, new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.50m, null, "Lelie", "70cm", null }
                });

            migrationBuilder.InsertData(
                table: "Producten",
                columns: new[] { "Id", "AanvoerderId", "Beschrijving", "Categorie", "FotoUrl", "HoeveelheidStuks", "KlokLocatie", "MinimumPrijs", "Naam", "PotmaatOfSteellengte", "Soort", "VeilDatum" },
                values: new object[,]
                {
                    { 1, 1, "Demo product van Aanvoerder 1", "Bloemen", "/AIimg/ai_self_made_pic_1.jpg.png", 100, "Naaldwijk", 1.50m, "Product A1-1", "60cm", "Roos", new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, "Demo product van Aanvoerder 1", "Bloemen", "/AIimg/ai_self_made_pic_2.jpg.png", 80, "Naaldwijk", 1.20m, "Product A1-2", "40cm", "Tulp", new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 2, "Demo product van Aanvoerder 2", "Planten", "/AIimg/ai_self_made_pic_3.jpg.png", 60, "Aalsmeer", 2.00m, "Product A2-1", "12cm", "Orchidee", new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 2, "Demo product van Aanvoerder 2", "Planten", "/AIimg/ai_self_made_pic_4.jpg.png", 50, "Aalsmeer", 2.50m, "Product A2-2", "70cm", "Lelie", new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Veilingen",
                columns: new[] { "Id", "CurrentVeilingProductId", "Datum", "EindTijdUtc", "Locatie", "Naam", "StartTijd", "StartTijdUtc", "Status", "VMId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "Veiling 1", new TimeSpan(0, 9, 0, 0, 0), null, 2, 1 },
                    { 2, null, new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, "Veiling 2", new TimeSpan(0, 10, 0, 0, 0), null, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "AuditEntries",
                columns: new[] { "Id", "Action", "ActorGebruikerId", "CreatedAtUtc", "VeilingId" },
                values: new object[,]
                {
                    { 1, "Seed: Veiling 1 aangemaakt", 3, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, "Seed: Veiling 2 aangemaakt", 4, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "VeilingProducten",
                columns: new[] { "Id", "AanmeldingId", "AanvoerderId", "ActivatedAtUtc", "ClosedAtUtc", "DurationSeconds", "Hoeveelheid", "HuidigePrijs", "KoperId", "MinimumPrijs", "ProductId", "StartPrijs", "Status", "VeilingId", "Volgorde" },
                values: new object[,]
                {
                    { 1, 1, 1, null, null, 20, 100, 3.00m, null, 1.50m, 1, 3.00m, 0, 1, 1 },
                    { 2, 2, 1, new DateTime(2025, 1, 12, 9, 5, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 12, 9, 5, 12, 0, DateTimeKind.Utc), 20, 80, 2.10m, 1, 1.20m, 2, 2.50m, 2, 1, 2 },
                    { 3, 3, 2, null, null, 20, 60, 4.00m, null, 2.00m, 3, 4.00m, 0, 2, 1 },
                    { 4, 4, 2, new DateTime(2025, 1, 13, 10, 2, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 13, 10, 2, 9, 0, DateTimeKind.Utc), 20, 50, 3.80m, 2, 2.50m, 4, 5.00m, 2, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Bids",
                columns: new[] { "Id", "Amount", "KoperId", "PlacedAtUtc", "VeilingId", "VeilingProductId" },
                values: new object[,]
                {
                    { 1, 2.10m, 1, new DateTime(2025, 1, 12, 9, 5, 10, 0, DateTimeKind.Utc), 1, 2 },
                    { 2, 3.80m, 2, new DateTime(2025, 1, 13, 10, 2, 8, 0, DateTimeKind.Utc), 2, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_AanvoerderId",
                table: "VeilingProducten",
                column: "AanvoerderId");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_ProductId",
                table: "VeilingProducten",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_VeilingId_Status",
                table: "VeilingProducten",
                columns: new[] { "VeilingId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_VeilingId_Volgorde",
                table: "VeilingProducten",
                columns: new[] { "VeilingId", "Volgorde" });

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_CurrentVeilingProductId",
                table: "Veilingen",
                column: "CurrentVeilingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_Datum_StartTijd",
                table: "Veilingen",
                columns: new[] { "Datum", "StartTijd" });

            migrationBuilder.CreateIndex(
                name: "IX_Veilingen_Status",
                table: "Veilingen",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Veildagen_Datum",
                table: "Veildagen",
                column: "Datum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gebruikers_Email",
                table: "Gebruikers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aanmeldingen_AanvoerderId_LeverDatum",
                table: "Aanmeldingen",
                columns: new[] { "AanvoerderId", "LeverDatum" });

            migrationBuilder.CreateIndex(
                name: "IX_Aanmeldingen_VeilingProductId",
                table: "Aanmeldingen",
                column: "VeilingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_ActorGebruikerId",
                table: "AuditEntries",
                column: "ActorGebruikerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_VeilingId_CreatedAtUtc",
                table: "AuditEntries",
                columns: new[] { "VeilingId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Bids_KoperId_PlacedAtUtc",
                table: "Bids",
                columns: new[] { "KoperId", "PlacedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Bids_VeilingId_VeilingProductId_PlacedAtUtc",
                table: "Bids",
                columns: new[] { "VeilingId", "VeilingProductId", "PlacedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Bids_VeilingProductId",
                table: "Bids",
                column: "VeilingProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Producten_Aanvoerders_AanvoerderId",
                table: "Producten",
                column: "AanvoerderId",
                principalTable: "Aanvoerders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingen_VeilingProducten_CurrentVeilingProductId",
                table: "Veilingen",
                column: "CurrentVeilingProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingen_Veilingmeesters_VMId",
                table: "Veilingen",
                column: "VMId",
                principalTable: "Veilingmeesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VeilingProducten_Aanvoerders_AanvoerderId",
                table: "VeilingProducten",
                column: "AanvoerderId",
                principalTable: "Aanvoerders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VeilingProducten_Kopers_KoperId",
                table: "VeilingProducten",
                column: "KoperId",
                principalTable: "Kopers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VeilingProducten_Producten_ProductId",
                table: "VeilingProducten",
                column: "ProductId",
                principalTable: "Producten",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producten_Aanvoerders_AanvoerderId",
                table: "Producten");

            migrationBuilder.DropForeignKey(
                name: "FK_Veilingen_VeilingProducten_CurrentVeilingProductId",
                table: "Veilingen");

            migrationBuilder.DropForeignKey(
                name: "FK_Veilingen_Veilingmeesters_VMId",
                table: "Veilingen");

            migrationBuilder.DropForeignKey(
                name: "FK_VeilingProducten_Aanvoerders_AanvoerderId",
                table: "VeilingProducten");

            migrationBuilder.DropForeignKey(
                name: "FK_VeilingProducten_Kopers_KoperId",
                table: "VeilingProducten");

            migrationBuilder.DropForeignKey(
                name: "FK_VeilingProducten_Producten_ProductId",
                table: "VeilingProducten");

            migrationBuilder.DropTable(
                name: "AuditEntries");

            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropIndex(
                name: "IX_VeilingProducten_AanvoerderId",
                table: "VeilingProducten");

            migrationBuilder.DropIndex(
                name: "IX_VeilingProducten_ProductId",
                table: "VeilingProducten");

            migrationBuilder.DropIndex(
                name: "IX_VeilingProducten_VeilingId_Status",
                table: "VeilingProducten");

            migrationBuilder.DropIndex(
                name: "IX_VeilingProducten_VeilingId_Volgorde",
                table: "VeilingProducten");

            migrationBuilder.DropIndex(
                name: "IX_Veilingen_CurrentVeilingProductId",
                table: "Veilingen");

            migrationBuilder.DropIndex(
                name: "IX_Veilingen_Datum_StartTijd",
                table: "Veilingen");

            migrationBuilder.DropIndex(
                name: "IX_Veilingen_Status",
                table: "Veilingen");

            migrationBuilder.DropIndex(
                name: "IX_Veildagen_Datum",
                table: "Veildagen");

            migrationBuilder.DropIndex(
                name: "IX_Gebruikers_Email",
                table: "Gebruikers");

            migrationBuilder.DropIndex(
                name: "IX_Aanmeldingen_AanvoerderId_LeverDatum",
                table: "Aanmeldingen");

            migrationBuilder.DropIndex(
                name: "IX_Aanmeldingen_VeilingProductId",
                table: "Aanmeldingen");

            migrationBuilder.DeleteData(
                table: "Veildagen",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Veildagen",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VeilingProducten",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VeilingProducten",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VeilingProducten",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "VeilingProducten",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Aanmeldingen",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Aanmeldingen",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Aanmeldingen",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Aanmeldingen",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Kopers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Kopers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Producten",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Producten",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Producten",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Producten",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Veilingen",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Veilingen",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Aanvoerders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Aanvoerders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Veilingmeesters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Veilingmeesters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Gebruikers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "AanvoerderId",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "ActivatedAtUtc",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "ClosedAtUtc",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "Hoeveelheid",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "MinimumPrijs",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "CurrentVeilingProductId",
                table: "Veilingen");

            migrationBuilder.DropColumn(
                name: "EindTijdUtc",
                table: "Veilingen");

            migrationBuilder.DropColumn(
                name: "Locatie",
                table: "Veilingen");

            migrationBuilder.DropColumn(
                name: "Naam",
                table: "Veilingen");

            migrationBuilder.RenameColumn(
                name: "VMId",
                table: "Veilingen",
                newName: "HuidigProductId");

            migrationBuilder.RenameColumn(
                name: "StartTijdUtc",
                table: "Veilingen",
                newName: "EindTijd");

            migrationBuilder.RenameIndex(
                name: "IX_Veilingen_VMId",
                table: "Veilingen",
                newName: "IX_Veilingen_HuidigProductId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActief",
                table: "VeilingProducten",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerkocht",
                table: "VeilingProducten",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Veilingmeesters",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Soort",
                table: "Producten",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "PotmaatOfSteellengte",
                table: "Producten",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Producten",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "KlokLocatie",
                table: "Producten",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FotoUrl",
                table: "Producten",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categorie",
                table: "Producten",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Beschrijving",
                table: "Producten",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Kopers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Voornaam",
                table: "Gebruikers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Gebruikers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Gebruikers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Achternaam",
                table: "Gebruikers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Aanvoerders",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Soort",
                table: "Aanmeldingen",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FotoUrl",
                table: "Aanmeldingen",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Beschrijving",
                table: "Aanmeldingen",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Biedingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KoperId = table.Column<int>(type: "int", nullable: false),
                    VeilingId = table.Column<int>(type: "int", nullable: false),
                    VeilingProductId = table.Column<int>(type: "int", nullable: false),
                    Prijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tijdstip = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Biedingen_VeilingProducten_VeilingProductId",
                        column: x => x.VeilingProductId,
                        principalTable: "VeilingProducten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Biedingen_Veilingen_VeilingId",
                        column: x => x.VeilingId,
                        principalTable: "Veilingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VeilingProducten_VeilingId",
                table: "VeilingProducten",
                column: "VeilingId");

            migrationBuilder.CreateIndex(
                name: "IX_Aanmeldingen_AanvoerderId",
                table: "Aanmeldingen",
                column: "AanvoerderId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Producten_Aanvoerders_AanvoerderId",
                table: "Producten",
                column: "AanvoerderId",
                principalTable: "Aanvoerders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veilingen_VeilingProducten_HuidigProductId",
                table: "Veilingen",
                column: "HuidigProductId",
                principalTable: "VeilingProducten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VeilingProducten_Kopers_KoperId",
                table: "VeilingProducten",
                column: "KoperId",
                principalTable: "Kopers",
                principalColumn: "Id");
        }
    }
}
