using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class HJ_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusDetails",
                table: "SYS_Task");

            migrationBuilder.DropColumn(
                name: "PlannedHours",
                table: "SYS_Projects");

            migrationBuilder.AddColumn<int>(
                name: "Percentage",
                table: "SYS_Task",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlannedDay",
                table: "SYS_Projects",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "SYS_Task");

            migrationBuilder.DropColumn(
                name: "PlannedDay",
                table: "SYS_Projects");

            migrationBuilder.AddColumn<string>(
                name: "StatusDetails",
                table: "SYS_Task",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PlannedHours",
                table: "SYS_Projects",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
