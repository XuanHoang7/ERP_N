using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMGiaoChiTieus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViKPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMGiaoChiTieus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DMGiaoChiTieus_vptq_kpi_DonViKPIs_DonViKPIId",
                        column: x => x.DonViKPIId,
                        principalTable: "vptq_kpi_DonViKPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DuocGiaoChiTieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DMGiaoChiTieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuocGiaoChiTieu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuocGiaoChiTieu_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuocGiaoChiTieu_DMGiaoChiTieus_DMGiaoChiTieuId",
                        column: x => x.DMGiaoChiTieuId,
                        principalTable: "DMGiaoChiTieus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMGiaoChiTieus_DonViKPIId",
                table: "DMGiaoChiTieus",
                column: "DonViKPIId");

            migrationBuilder.CreateIndex(
                name: "IX_DuocGiaoChiTieu_DMGiaoChiTieuId",
                table: "DuocGiaoChiTieu",
                column: "DMGiaoChiTieuId");

            migrationBuilder.CreateIndex(
                name: "IX_DuocGiaoChiTieu_UserId",
                table: "DuocGiaoChiTieu",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuocGiaoChiTieu");

            migrationBuilder.DropTable(
                name: "DMGiaoChiTieus");
        }
    }
}
