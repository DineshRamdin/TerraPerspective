using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class HJ_004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "SYS_RowAccess",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SYS_RowAccess",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "DeleteStatus",
                table: "SYS_RowAccess",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "SYS_RowAccess",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SYS_RowAccess",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "SYS_GroupMatrixUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SYS_GroupMatrixUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "DeleteStatus",
                table: "SYS_GroupMatrixUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "SYS_GroupMatrixUser",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SYS_GroupMatrixUser",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "SYS_GroupMatrix",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SYS_GroupMatrix",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "DeleteStatus",
                table: "SYS_GroupMatrix",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "SYS_GroupMatrix",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SYS_GroupMatrix",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SYS_RowAccess");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SYS_RowAccess");

            migrationBuilder.DropColumn(
                name: "DeleteStatus",
                table: "SYS_RowAccess");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SYS_RowAccess");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SYS_RowAccess");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SYS_GroupMatrixUser");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SYS_GroupMatrixUser");

            migrationBuilder.DropColumn(
                name: "DeleteStatus",
                table: "SYS_GroupMatrixUser");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SYS_GroupMatrixUser");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SYS_GroupMatrixUser");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SYS_GroupMatrix");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SYS_GroupMatrix");

            migrationBuilder.DropColumn(
                name: "DeleteStatus",
                table: "SYS_GroupMatrix");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SYS_GroupMatrix");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SYS_GroupMatrix");
        }
    }
}
