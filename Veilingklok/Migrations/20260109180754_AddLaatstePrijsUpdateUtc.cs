using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class AddLaatstePrijsUpdateUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LaatstePrijsUpdateUtc",
                table: "VeilingProducten",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaatstePrijsUpdateUtc",
                table: "VeilingProducten");
        }
    }
}
