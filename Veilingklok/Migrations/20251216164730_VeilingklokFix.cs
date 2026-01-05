using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veilingklok.Migrations
{
    /// <inheritdoc />
    public partial class VeilingklokFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartPrijs",
                table: "VeilingProducten",
                newName: "MinimumPrijs");

            migrationBuilder.AddColumn<decimal>(
                name: "DalingPerSeconde",
                table: "VeilingProducten",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDoorgedraaid",
                table: "VeilingProducten",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumPrijs",
                table: "VeilingProducten",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ResterendeHoeveelheid",
                table: "VeilingProducten",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Aantal",
                table: "Biedingen",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DalingPerSeconde",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "IsDoorgedraaid",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "MaximumPrijs",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "ResterendeHoeveelheid",
                table: "VeilingProducten");

            migrationBuilder.DropColumn(
                name: "Aantal",
                table: "Biedingen");

            migrationBuilder.RenameColumn(
                name: "MinimumPrijs",
                table: "VeilingProducten",
                newName: "StartPrijs");
        }
    }
}
