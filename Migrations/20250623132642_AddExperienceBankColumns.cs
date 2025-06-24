using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddExperienceBankColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExperienceBankBookingId",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MarketplaceBookingId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MarketplaceId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PartnerBookingId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PartnerSupplierId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalFieldsJson",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddonsJson",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Occupancy",
                table: "ReservationGuests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TicketCategory",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TicketId",
                table: "ReservationGuests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DetailsUrl",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PartnerSupplierId",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ExperienceBankBookingId",
                table: "Reservations",
                column: "ExperienceBankBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ReservationId",
                table: "Ticket",
                column: "ReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ExperienceBankBookingId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ExperienceBankBookingId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "MarketplaceBookingId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "MarketplaceId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PartnerBookingId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PartnerSupplierId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "AdditionalFieldsJson",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "AddonsJson",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "Occupancy",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "TicketCategory",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "ReservationGuests");

            migrationBuilder.DropColumn(
                name: "DetailsUrl",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "PartnerSupplierId",
                table: "Activities");
        }
    }
}
