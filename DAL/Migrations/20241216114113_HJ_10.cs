using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class HJ_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectColorCode",
                table: "SYS_Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectTemplateId",
                table: "SYS_Projects",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SYS_Projects_ProjectTemplateId",
                table: "SYS_Projects",
                column: "ProjectTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_SYS_Projects_SYS_ProjectTemplate_ProjectTemplateId",
                table: "SYS_Projects",
                column: "ProjectTemplateId",
                principalTable: "SYS_ProjectTemplate",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SYS_Projects_SYS_ProjectTemplate_ProjectTemplateId",
                table: "SYS_Projects");

            migrationBuilder.DropIndex(
                name: "IX_SYS_Projects_ProjectTemplateId",
                table: "SYS_Projects");

            migrationBuilder.DropColumn(
                name: "ProjectColorCode",
                table: "SYS_Projects");

            migrationBuilder.DropColumn(
                name: "ProjectTemplateId",
                table: "SYS_Projects");
        }
    }
}
