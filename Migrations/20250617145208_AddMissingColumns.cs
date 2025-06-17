using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestFieldOptionTranslation");

            migrationBuilder.DropTable(
                name: "GuestFieldTranslation");

            migrationBuilder.DropTable(
                name: "GuestFieldOption");

            migrationBuilder.DropTable(
                name: "GuestField");

            migrationBuilder.DropColumn(
                name: "Excluded",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "Inclusions",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "'[]'",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImportantInfo",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "'[]'",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "GuestFields",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "'[]'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestFields",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "Inclusions",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "'[]'");

            migrationBuilder.AlterColumn<string>(
                name: "ImportantInfo",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "'[]'");

            migrationBuilder.AddColumn<string>(
                name: "Excluded",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "GuestField",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestField", x => new { x.ActivityId, x.Id });
                    table.ForeignKey(
                        name: "FK_GuestField_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestFieldOption",
                columns: table => new
                {
                    GuestFieldActivityId = table.Column<int>(type: "int", nullable: false),
                    GuestFieldId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestFieldOption", x => new { x.GuestFieldActivityId, x.GuestFieldId, x.Id });
                    table.ForeignKey(
                        name: "FK_GuestFieldOption_GuestField_GuestFieldActivityId_GuestFieldId",
                        columns: x => new { x.GuestFieldActivityId, x.GuestFieldId },
                        principalTable: "GuestField",
                        principalColumns: new[] { "ActivityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestFieldTranslation",
                columns: table => new
                {
                    GuestFieldActivityId = table.Column<int>(type: "int", nullable: false),
                    GuestFieldId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestFieldTranslation", x => new { x.GuestFieldActivityId, x.GuestFieldId, x.Id });
                    table.ForeignKey(
                        name: "FK_GuestFieldTranslation_GuestField_GuestFieldActivityId_GuestFieldId",
                        columns: x => new { x.GuestFieldActivityId, x.GuestFieldId },
                        principalTable: "GuestField",
                        principalColumns: new[] { "ActivityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestFieldOptionTranslation",
                columns: table => new
                {
                    GuestFieldOptionGuestFieldActivityId = table.Column<int>(type: "int", nullable: false),
                    GuestFieldOptionGuestFieldId = table.Column<int>(type: "int", nullable: false),
                    GuestFieldOptionId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestFieldOptionTranslation", x => new { x.GuestFieldOptionGuestFieldActivityId, x.GuestFieldOptionGuestFieldId, x.GuestFieldOptionId, x.Id });
                    table.ForeignKey(
                        name: "FK_GuestFieldOptionTranslation_GuestFieldOption_GuestFieldOptionGuestFieldActivityId_GuestFieldOptionGuestFieldId_GuestFieldOpt~",
                        columns: x => new { x.GuestFieldOptionGuestFieldActivityId, x.GuestFieldOptionGuestFieldId, x.GuestFieldOptionId },
                        principalTable: "GuestFieldOption",
                        principalColumns: new[] { "GuestFieldActivityId", "GuestFieldId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
