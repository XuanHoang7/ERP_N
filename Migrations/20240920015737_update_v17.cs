using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class update_v17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DM_DonViDanhGias_vptq_kpi_DonViKPIs_DonViKPIId",
                table: "DM_DonViDanhGias");

            migrationBuilder.DropIndex(
                name: "IX_DM_DonViDanhGias_DonViKPIId",
                table: "DM_DonViDanhGias");

            migrationBuilder.DropColumn(
                name: "DonViKPIId",
                table: "DM_DonViDanhGias");

            migrationBuilder.CreateIndex(
                name: "IX_DM_DonViDanhGias_IdDonViKPI",
                table: "DM_DonViDanhGias",
                column: "IdDonViKPI");

            migrationBuilder.AddForeignKey(
                name: "FK_DM_DonViDanhGias_vptq_kpi_DonViKPIs_IdDonViKPI",
                table: "DM_DonViDanhGias",
                column: "IdDonViKPI",
                principalTable: "vptq_kpi_DonViKPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DM_DonViDanhGias_vptq_kpi_DonViKPIs_IdDonViKPI",
                table: "DM_DonViDanhGias");

            migrationBuilder.DropIndex(
                name: "IX_DM_DonViDanhGias_IdDonViKPI",
                table: "DM_DonViDanhGias");

            migrationBuilder.AddColumn<Guid>(
                name: "DonViKPIId",
                table: "DM_DonViDanhGias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DM_DonViDanhGias_DonViKPIId",
                table: "DM_DonViDanhGias",
                column: "DonViKPIId");

            migrationBuilder.AddForeignKey(
                name: "FK_DM_DonViDanhGias_vptq_kpi_DonViKPIs_DonViKPIId",
                table: "DM_DonViDanhGias",
                column: "DonViKPIId",
                principalTable: "vptq_kpi_DonViKPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
