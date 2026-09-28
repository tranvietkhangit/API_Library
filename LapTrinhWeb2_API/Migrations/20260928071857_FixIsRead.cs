using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LapTrinhWeb2_API.Migrations
{
    /// <inheritdoc />
    public partial class FixIsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IssRead",
                table: "Books",
                newName: "IsRead");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "Books",
                newName: "IssRead");
        }
    }
}
