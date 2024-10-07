using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v94 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "IsCompleteApproved",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "IsRefuseForApproved",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "NoteForApprove",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "WillApproved",
                table: "KPIs");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "KPIDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleteApproved",
                table: "KPIDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefuseForApproved",
                table: "KPIDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NoteForApprove",
                table: "KPIDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WillApproved",
                table: "KPIDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "IsCompleteApproved",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "IsRefuseForApproved",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "NoteForApprove",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "WillApproved",
                table: "KPIDetail");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleteApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefuseForApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NoteForApprove",
                table: "KPIs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WillApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
