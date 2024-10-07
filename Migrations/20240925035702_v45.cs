using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBrowse",
                table: "DuyetPIStatus");

            migrationBuilder.DropColumn(
                name: "IsBrowse",
                table: "DuyetPIs");

            migrationBuilder.AddColumn<int>(
                name: "ThuTuNow",
                table: "DuyetPIs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThuTuNow",
                table: "DuyetPIs");

            migrationBuilder.AddColumn<bool>(
                name: "IsBrowse",
                table: "DuyetPIStatus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBrowse",
                table: "DuyetPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
