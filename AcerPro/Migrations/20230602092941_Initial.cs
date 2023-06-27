using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcerPro.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AppName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    AppUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interval = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DbUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbUsers_UserName",
                table: "DbUsers",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbApplications");

            migrationBuilder.DropTable(
                name: "DbUsers");
        }
    }
}
