using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class MP_007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "SYS_ZoneManagement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FillColor",
                table: "SYS_ZoneManagement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "LineWidth",
                table: "SYS_ZoneManagement",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transparancy",
                table: "SYS_ZoneManagement",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "SYS_ZoneManagement");

            migrationBuilder.DropColumn(
                name: "FillColor",
                table: "SYS_ZoneManagement");

            migrationBuilder.DropColumn(
                name: "LineWidth",
                table: "SYS_ZoneManagement");

            migrationBuilder.DropColumn(
                name: "Transparancy",
                table: "SYS_ZoneManagement");
        }
    }
}
