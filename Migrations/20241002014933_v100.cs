using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDuyet",
                table: "StreamReviewKPI",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRefuse",
                table: "KPIs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRefuseForApproved",
                table: "KPIDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDuyet",
                table: "DuyetPIStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRefuse",
                table: "DuyetPIs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDuyet",
                table: "StreamReviewKPI");

            migrationBuilder.DropColumn(
                name: "DateRefuse",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "DateRefuseForApproved",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "DateDuyet",
                table: "DuyetPIStatus");

            migrationBuilder.DropColumn(
                name: "DateRefuse",
                table: "DuyetPIs");
        }
    }
}
