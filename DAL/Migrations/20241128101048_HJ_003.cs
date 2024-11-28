using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class HJ_003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SYS_GroupMatrix",
                columns: table => new
                {
                    GMID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentGMID = table.Column<long>(type: "bigint", nullable: true),
                    GMDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompany = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYS_GroupMatrix", x => x.GMID);
                });

            migrationBuilder.CreateTable(
                name: "SYS_GroupMatrixUser",
                columns: table => new
                {
                    MID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IID = table.Column<long>(type: "bigint", nullable: false),
                    GMID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYS_GroupMatrixUser", x => x.MID);
                });

            migrationBuilder.CreateTable(
                name: "SYS_RowAccess",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<long>(type: "bigint", nullable: false),
                    ModuleName = table.Column<int>(type: "int", nullable: false),
                    Struc = table.Column<int>(type: "int", nullable: false),
                    StrucId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYS_RowAccess", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SYS_GroupMatrix");

            migrationBuilder.DropTable(
                name: "SYS_GroupMatrixUser");

            migrationBuilder.DropTable(
                name: "SYS_RowAccess");
        }
    }
}
