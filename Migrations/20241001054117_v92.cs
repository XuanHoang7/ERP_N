using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v92 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_DanhMucPIChiTiet_DanhMucPIChiTietId",
                table: "KPIs");

            migrationBuilder.DropTable(
                name: "KPIChild");

            migrationBuilder.DropIndex(
                name: "IX_KPIs_DanhMucPIChiTietId",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "ChiTieuCanDat",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "DanhMucPIChiTietId",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "DienGiai",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "IsAddChiTieuNam",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "Tytrong",
                table: "KPIs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ChiTieuCanDat",
                table: "KPIs",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DanhMucPIChiTietId",
                table: "KPIs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DienGiai",
                table: "KPIs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAddChiTieuNam",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "Tytrong",
                table: "KPIs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "KPIChild",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PIPhuThuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIChild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIChild_KPIs_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIChild_PIPhuThuoc_PIPhuThuocId",
                        column: x => x.PIPhuThuocId,
                        principalTable: "PIPhuThuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_DanhMucPIChiTietId",
                table: "KPIs",
                column: "DanhMucPIChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIChild_KPIId",
                table: "KPIChild",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIChild_PIPhuThuocId",
                table: "KPIChild",
                column: "PIPhuThuocId");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_DanhMucPIChiTiet_DanhMucPIChiTietId",
                table: "KPIs",
                column: "DanhMucPIChiTietId",
                principalTable: "DanhMucPIChiTiet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
