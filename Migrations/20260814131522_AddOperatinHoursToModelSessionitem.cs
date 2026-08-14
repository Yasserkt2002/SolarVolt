using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarVolt.Migrations
{
    /// <inheritdoc />
    public partial class AddOperatinHoursToModelSessionitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OperatingHours",
                table: "Energy_Input_Items",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperatingHours",
                table: "Energy_Input_Items");
        }
    }
}
