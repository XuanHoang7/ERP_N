using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v71 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "KPI",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleteApproved",
                table: "KPI",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefuseForApproved",
                table: "KPI",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NoteForApprove",
                table: "KPI",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WillApproved",
                table: "KPI",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "KPI");

            migrationBuilder.DropColumn(
                name: "IsCompleteApproved",
                table: "KPI");

            migrationBuilder.DropColumn(
                name: "IsRefuseForApproved",
                table: "KPI");

            migrationBuilder.DropColumn(
                name: "NoteForApprove",
                table: "KPI");

            migrationBuilder.DropColumn(
                name: "WillApproved",
                table: "KPI");
        }
    }
}
