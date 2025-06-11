using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAddonsToActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price_Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price_Currency = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addon", x => new { x.ActivityId, x.Id });
                    table.ForeignKey(
                        name: "FK_Addon_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddonTranslation",
                columns: table => new
                {
                    AddonActivityId = table.Column<int>(type: "int", nullable: false),
                    AddonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddonTranslation", x => new { x.AddonActivityId, x.AddonId, x.Id });
                    table.ForeignKey(
                        name: "FK_AddonTranslation_Addon_AddonActivityId_AddonId",
                        columns: x => new { x.AddonActivityId, x.AddonId },
                        principalTable: "Addon",
                        principalColumns: new[] { "ActivityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddonTranslation");

            migrationBuilder.DropTable(
                name: "Addon");
        }
    }
}
