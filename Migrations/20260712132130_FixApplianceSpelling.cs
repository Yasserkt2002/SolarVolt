using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarVolt.Migrations
{
    /// <inheritdoc />
    public partial class FixApplianceSpelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefalutWattage",
                table: "Appliances",
                newName: "DefaultWattage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultWattage",
                table: "Appliances",
                newName: "DefalutWattage");
        }
    }
}
