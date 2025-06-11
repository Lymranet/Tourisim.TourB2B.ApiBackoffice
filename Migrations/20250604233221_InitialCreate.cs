using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subcategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Coordinates_Latitude = table.Column<double>(type: "float", nullable: true),
                    Location_Coordinates_Longitude = table.Column<double>(type: "float", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    SeasonalAvailability_StartDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeasonalAvailability_EndDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceInfo_Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceInfo_BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceInfo_MinimumParticipants = table.Column<int>(type: "int", nullable: true),
                    PriceInfo_MaximumParticipants = table.Column<int>(type: "int", nullable: true),
                    Pricing_DefaultCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pricing_TaxIncluded = table.Column<bool>(type: "bit", nullable: false),
                    Pricing_TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Included = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Excluded = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo_Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo_Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CancellationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Media_Images_Header = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Media_Images_Teaser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Media_Images_Gallery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Media_Videos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesAvailability_StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SalesAvailability_EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuestField",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false)
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
                name: "MeetingPoint",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingPoint", x => new { x.ActivityId, x.Id });
                    table.ForeignKey(
                        name: "FK_MeetingPoint_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceCategory",
                columns: table => new
                {
                    ActivityPricingActivityId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinParticipants = table.Column<int>(type: "int", nullable: true),
                    MaxParticipants = table.Column<int>(type: "int", nullable: true),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceCategory", x => new { x.ActivityPricingActivityId, x.Id });
                    table.ForeignKey(
                        name: "FK_PriceCategory_Activities_ActivityPricingActivityId",
                        column: x => x.ActivityPricingActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlot",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot", x => new { x.ActivityId, x.Id });
                    table.ForeignKey(
                        name: "FK_TimeSlot_Activities_ActivityId",
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
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestFieldOptionTranslation");

            migrationBuilder.DropTable(
                name: "GuestFieldTranslation");

            migrationBuilder.DropTable(
                name: "MeetingPoint");

            migrationBuilder.DropTable(
                name: "PriceCategory");

            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.DropTable(
                name: "GuestFieldOption");

            migrationBuilder.DropTable(
                name: "GuestField");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
