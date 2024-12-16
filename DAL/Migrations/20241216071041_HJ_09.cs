using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class HJ_09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentTask",
                table: "SYS_Task",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SYS_Task_ParentTask",
                table: "SYS_Task",
                column: "ParentTask");

            migrationBuilder.AddForeignKey(
                name: "FK_SYS_Task_SYS_Task_ParentTask",
                table: "SYS_Task",
                column: "ParentTask",
                principalTable: "SYS_Task",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SYS_Task_SYS_Task_ParentTask",
                table: "SYS_Task");

            migrationBuilder.DropIndex(
                name: "IX_SYS_Task_ParentTask",
                table: "SYS_Task");

            migrationBuilder.DropColumn(
                name: "ParentTask",
                table: "SYS_Task");
        }
    }
}
