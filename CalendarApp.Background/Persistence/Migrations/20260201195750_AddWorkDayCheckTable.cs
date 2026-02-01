using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarApp.Background.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkDayCheckTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkDayChecks",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsWorkDay = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkDayChecks", x => new { x.Date, x.CountryCode });
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkDayChecks_Date_CountryCode",
                table: "WorkDayChecks",
                columns: new[] { "Date", "CountryCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkDayChecks");
        }
    }
}
