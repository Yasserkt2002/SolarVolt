using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarVolt.Migrations
{
    /// <inheritdoc />
    public partial class MakeApplianceIDnullAble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Energy_Input_Items_Appliances_ApplianceID",
                table: "Energy_Input_Items");

            migrationBuilder.AlterColumn<int>(
                name: "ApplianceID",
                table: "Energy_Input_Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Energy_Input_Items_Appliances_ApplianceID",
                table: "Energy_Input_Items",
                column: "ApplianceID",
                principalTable: "Appliances",
                principalColumn: "ApplianceID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Energy_Input_Items_Appliances_ApplianceID",
                table: "Energy_Input_Items");

            migrationBuilder.AlterColumn<int>(
                name: "ApplianceID",
                table: "Energy_Input_Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Energy_Input_Items_Appliances_ApplianceID",
                table: "Energy_Input_Items",
                column: "ApplianceID",
                principalTable: "Appliances",
                principalColumn: "ApplianceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
