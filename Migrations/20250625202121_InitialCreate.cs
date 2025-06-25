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
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subcategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ContactInfo_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactInfo_Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Media_Videos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContactInfoIsNull = table.Column<bool>(type: "bit", nullable: true),
                    Categories = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    DestinationCode = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    DestinationName = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Exclusions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Highlights = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportantInfo = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    Inclusions = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Itinerary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalRatingCount = table.Column<int>(type: "int", nullable: true),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GalleryImages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviewImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExclusionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    GuestFieldsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    ImportantInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    InclusionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    GuestFields = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    DetailsUrl = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: ""),
                    PartnerSupplierId = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: ""),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    IsFreeCancellation = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLanguage_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "TRY")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Addons__3214EC07FF36CB8D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addons_Activities",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CancellationPolicyConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    MinDurationBeforeStartTimeSec = table.Column<int>(type: "int", nullable: false),
                    RefundPercentage = table.Column<int>(type: "int", nullable: false),
                    IsFreeCancellation = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cancella__3214EC070325041F", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancellationPolicyConditions_Activity",
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
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CutOff = table.Column<int>(type: "int", nullable: false),
                    Weekdays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanBeBookedAfterStartTime = table.Column<bool>(type: "bit", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UntilDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Options_Activities_ActivityId",
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
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxAge = table.Column<int>(type: "int", nullable: true),
                    MinAge = table.Column<int>(type: "int", nullable: true)
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
                name: "RoutePoint",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePoint", x => new { x.ActivityId, x.Id });
                    table.ForeignKey(
                        name: "FK_RoutePoint_Activities_ActivityId",
                        column: x => x.ActivityId,
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
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Highlights = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Itinerary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InclusionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExclusionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImportantInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Translat__3214EC07A96AFBF4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Activities",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AddonTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddonId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AddonTra__3214EC07325ED02A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddonTranslations_Addons",
                        column: x => x.AddonId,
                        principalTable: "Addons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerSupplierId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AvailableCapacity = table.Column<int>(type: "int", nullable: false),
                    MaximumCapacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Availabi__3214EC0722F78A97", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Availabilities_Availabilities",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Availabilities_Options",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id");
                });

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
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuestCount = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExperienceBankBookingId = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: ""),
                    MarketplaceBookingId = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    MarketplaceId = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    PartnerBookingId = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    PartnerSupplierId = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id");
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
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "ReservationGuests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    GuestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    AdditionalFieldsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    AddonsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Occupancy = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    TicketCategory = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    TicketId = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationGuests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationGuests_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperienceBankTicketId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InternalTicketId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TicketCodeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketCategoryCapacities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvailabilityId = table.Column<int>(type: "int", nullable: false),
                    TicketCategoryId = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TicketCa__3214EC07C98BD1B7", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCategoryCapacities_TicketCategories",
                        column: x => x.TicketCategoryId,
                        principalTable: "TicketCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__TicketCat__Avail__69FBBC1F",
                        column: x => x.AvailabilityId,
                        principalTable: "Availabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLanguage_ActivityId",
                table: "ActivityLanguages",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addons_ActivityId",
                table: "Addons",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AddonTranslations_AddonId",
                table: "AddonTranslations",
                column: "AddonId");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_ActivityId",
                table: "Availabilities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_OptionId",
                table: "Availabilities",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationPolicyConditions_ActivityId",
                table: "CancellationPolicyConditions",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHours_OptionId",
                table: "OpeningHours",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_ActivityId",
                table: "Options",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationGuests_ReservationId",
                table: "ReservationGuests",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ActivityId",
                table: "Reservations",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ExperienceBankBookingId",
                table: "Reservations",
                column: "ExperienceBankBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_OptionId",
                table: "Reservations",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ReservationId",
                table: "Ticket",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategories_OptionId",
                table: "TicketCategories",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategoryCapacities_AvailabilityId",
                table: "TicketCategoryCapacities",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategoryCapacities_TicketCategoryId",
                table: "TicketCategoryCapacities",
                column: "TicketCategoryId");

            migrationBuilder.CreateIndex(
                name: "UX_Translation_Activity_Language",
                table: "Translations",
                columns: new[] { "ActivityId", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLanguages");

            migrationBuilder.DropTable(
                name: "AddonTranslations");

            migrationBuilder.DropTable(
                name: "CancellationPolicyConditions");

            migrationBuilder.DropTable(
                name: "MeetingPoint");

            migrationBuilder.DropTable(
                name: "OpeningHours");

            migrationBuilder.DropTable(
                name: "PriceCategory");

            migrationBuilder.DropTable(
                name: "ReservationGuests");

            migrationBuilder.DropTable(
                name: "RoutePoint");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "TicketCategoryCapacities");

            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "Addons");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "TicketCategories");

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
