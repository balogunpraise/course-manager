using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SessionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AcademicSessionId",
                table: "Schedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcademicSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_AcademicSessionId",
                table: "Schedules",
                column: "AcademicSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_AcademicSessions_AcademicSessionId",
                table: "Schedules",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_AcademicSessions_AcademicSessionId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "AcademicSessions");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_AcademicSessionId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "Schedules");
        }
    }
}
