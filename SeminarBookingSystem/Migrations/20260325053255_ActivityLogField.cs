using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeminarBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class ActivityLogField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Participant",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Participant",
                newName: "FullName");

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingDate",
                table: "Participant",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Participant",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeminarBookingId",
                table: "Participant",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Participant_SeminarBookingId",
                table: "Participant",
                column: "SeminarBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Seminar_SeminarBookingId",
                table: "Participant",
                column: "SeminarBookingId",
                principalTable: "Seminar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Seminar_SeminarBookingId",
                table: "Participant");

            migrationBuilder.DropTable(
                name: "ActivityLog");

            migrationBuilder.DropIndex(
                name: "IX_Participant_SeminarBookingId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "BookingDate",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "SeminarBookingId",
                table: "Participant");

            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "Participant",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Participant",
                newName: "FirstName");
        }
    }
}
