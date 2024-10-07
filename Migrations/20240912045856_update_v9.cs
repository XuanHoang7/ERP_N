using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class update_v9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DM_LanhDaoDonVis_DonVis_DonViID",
                table: "DM_LanhDaoDonVis");

            migrationBuilder.DropForeignKey(
                name: "FK_NSKhongDanhGias_DonVis_DonViID",
                table: "NSKhongDanhGias");

            migrationBuilder.AddForeignKey(
                name: "FK_DM_LanhDaoDonVis_vptq_kpi_DonViKPIs_DonViID",
                table: "DM_LanhDaoDonVis",
                column: "DonViID",
                principalTable: "vptq_kpi_DonViKPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NSKhongDanhGias_vptq_kpi_DonViKPIs_DonViID",
                table: "NSKhongDanhGias",
                column: "DonViID",
                principalTable: "vptq_kpi_DonViKPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DM_LanhDaoDonVis_vptq_kpi_DonViKPIs_DonViID",
                table: "DM_LanhDaoDonVis");

            migrationBuilder.DropForeignKey(
                name: "FK_NSKhongDanhGias_vptq_kpi_DonViKPIs_DonViID",
                table: "NSKhongDanhGias");

            migrationBuilder.AddForeignKey(
                name: "FK_DM_LanhDaoDonVis_DonVis_DonViID",
                table: "DM_LanhDaoDonVis",
                column: "DonViID",
                principalTable: "DonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NSKhongDanhGias_DonVis_DonViID",
                table: "NSKhongDanhGias",
                column: "DonViID",
                principalTable: "DonVis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
