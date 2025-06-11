using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMinMaxAgeToPriceCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAge",
                table: "PriceCategory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinAge",
                table: "PriceCategory",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "Activities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceInfo_MaxAge",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceInfo_MinAge",
                table: "Activities",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAge",
                table: "PriceCategory");

            migrationBuilder.DropColumn(
                name: "MinAge",
                table: "PriceCategory");

            migrationBuilder.DropColumn(
                name: "PriceInfo_MaxAge",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "PriceInfo_MinAge",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "Subcategory",
                table: "Activities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
