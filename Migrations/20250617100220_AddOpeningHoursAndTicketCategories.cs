using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOpeningHoursAndTicketCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpeningHour");

            migrationBuilder.DropTable(
                name: "TicketCategory");

            migrationBuilder.DropColumn(
                name: "SatisBaslangicTarihi",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "SatisBitisTarihi",
                table: "Options");

            migrationBuilder.RenameColumn(
                name: "HemenRezerveEdilebilir",
                table: "Options",
                newName: "CanBeBookedAfterStartTime");

            migrationBuilder.RenameColumn(
                name: "HaftaninGunleri",
                table: "Options",
                newName: "Weekdays");

            migrationBuilder.RenameColumn(
                name: "CutOffDakika",
                table: "Options",
                newName: "CutOff");

            migrationBuilder.RenameColumn(
                name: "BaslangicSaati",
                table: "Options",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "Adi",
                table: "Options",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Options",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "Options",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UntilDate",
                table: "Options",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Activities",
                type: "decimal(4,2)",
                precision: 4,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Categories",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationCode",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationName",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DetailsPageUrl",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Exclusions",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Highlights",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImportantInfo",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Inclusions",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeCancellation",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Itinerary",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalRatingCount",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OpeningHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningHours_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    MinDurationBeforeStartTimeSec = table.Column<int>(type: "int", nullable: false),
                    RefundPercentage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundCondition", x => new { x.ActivityId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefundCondition_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinSeats = table.Column<int>(type: "int", nullable: false),
                    MaxSeats = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: true),
                    MaxAge = table.Column<int>(type: "int", nullable: true),
                    OptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCategories_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHours_OptionId",
                table: "OpeningHours",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_OptionId",
                table: "TicketCategories",
                column: "OptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpeningHours");

            migrationBuilder.DropTable(
                name: "RefundCondition");

            migrationBuilder.DropTable(
                name: "TicketCategories");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "UntilDate",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DestinationCode",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DestinationName",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DetailsPageUrl",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Exclusions",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Highlights",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ImportantInfo",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Inclusions",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "IsFreeCancellation",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Itinerary",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TotalRatingCount",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "Weekdays",
                table: "Options",
                newName: "HaftaninGunleri");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Options",
                newName: "BaslangicSaati");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Options",
                newName: "Adi");

            migrationBuilder.RenameColumn(
                name: "CutOff",
                table: "Options",
                newName: "CutOffDakika");

            migrationBuilder.RenameColumn(
                name: "CanBeBookedAfterStartTime",
                table: "Options",
                newName: "HemenRezerveEdilebilir");

            migrationBuilder.AddColumn<DateTime>(
                name: "SatisBaslangicTarihi",
                table: "Options",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SatisBitisTarihi",
                table: "Options",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OpeningHour",
                columns: table => new
                {
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslangic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bitis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningHour", x => new { x.OptionId, x.Id });
                    table.ForeignKey(
                        name: "FK_OpeningHour_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxKoltuk = table.Column<int>(type: "int", nullable: false),
                    MinKoltuk = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat_ParaBirimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat_Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat_Tutar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YasSiniri_MaxYas = table.Column<int>(type: "int", nullable: true),
                    YasSiniri_MinYas = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCategory_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategory_OptionId",
                table: "TicketCategory",
                column: "OptionId");
        }
    }
}
