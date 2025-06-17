using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Adi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BaslangicSaati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SatisBaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SatisBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CutOffDakika = table.Column<int>(type: "int", nullable: false),
                    HaftaninGunleri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HemenRezerveEdilebilir = table.Column<bool>(type: "bit", nullable: false)
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
                    MinKoltuk = table.Column<int>(type: "int", nullable: false),
                    MaxKoltuk = table.Column<int>(type: "int", nullable: false),
                    Fiyat_Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat_Tutar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat_ParaBirimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YasSiniri_MinYas = table.Column<int>(type: "int", nullable: true),
                    YasSiniri_MaxYas = table.Column<int>(type: "int", nullable: true),
                    OptionId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Options_ActivityId",
                table: "Options",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCategory_OptionId",
                table: "TicketCategory",
                column: "OptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpeningHour");

            migrationBuilder.DropTable(
                name: "TicketCategory");

            migrationBuilder.DropTable(
                name: "Options");
        }
    }
}
