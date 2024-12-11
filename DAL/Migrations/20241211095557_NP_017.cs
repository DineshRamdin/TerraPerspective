using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class NP_017 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Follow5",
                table: "SYS_Company",
                newName: "Colour5");

            migrationBuilder.RenameColumn(
                name: "Follow4",
                table: "SYS_Company",
                newName: "Colour4");

            migrationBuilder.RenameColumn(
                name: "Follow3",
                table: "SYS_Company",
                newName: "Colour3");

            migrationBuilder.RenameColumn(
                name: "Follow2",
                table: "SYS_Company",
                newName: "Colour2");

            migrationBuilder.RenameColumn(
                name: "Follow1",
                table: "SYS_Company",
                newName: "Colour1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Colour5",
                table: "SYS_Company",
                newName: "Follow5");

            migrationBuilder.RenameColumn(
                name: "Colour4",
                table: "SYS_Company",
                newName: "Follow4");

            migrationBuilder.RenameColumn(
                name: "Colour3",
                table: "SYS_Company",
                newName: "Follow3");

            migrationBuilder.RenameColumn(
                name: "Colour2",
                table: "SYS_Company",
                newName: "Follow2");

            migrationBuilder.RenameColumn(
                name: "Colour1",
                table: "SYS_Company",
                newName: "Follow1");
        }
    }
}
