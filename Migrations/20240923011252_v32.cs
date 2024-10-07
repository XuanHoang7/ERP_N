using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPIs_DonViDos_IdDonViDo",
                table: "DanhMucPIs");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucPIs_IdDonViDo",
                table: "DanhMucPIs");

            migrationBuilder.DropColumn(
                name: "ChieuHuongTot",
                table: "DanhMucPIs");

            migrationBuilder.DropColumn(
                name: "HeSoHoanThanhK",
                table: "DanhMucPIs");

            migrationBuilder.DropColumn(
                name: "IdDonViDo",
                table: "DanhMucPIs");

            migrationBuilder.AddColumn<Guid>(
                name: "DanhMucPIChiTietId",
                table: "KetQuaDanhGia",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChieuHuongTot",
                table: "DanhMucPIChiTiet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeSoHoanThanhK",
                table: "DanhMucPIChiTiet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdDonViDo",
                table: "DanhMucPIChiTiet",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDanhGia_DanhMucPIChiTietId",
                table: "KetQuaDanhGia",
                column: "DanhMucPIChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIChiTiet_IdDonViDo",
                table: "DanhMucPIChiTiet",
                column: "IdDonViDo");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPIChiTiet_DonViDos_IdDonViDo",
                table: "DanhMucPIChiTiet",
                column: "IdDonViDo",
                principalTable: "DonViDos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KetQuaDanhGia_DanhMucPIChiTiet_DanhMucPIChiTietId",
                table: "KetQuaDanhGia",
                column: "DanhMucPIChiTietId",
                principalTable: "DanhMucPIChiTiet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPIChiTiet_DonViDos_IdDonViDo",
                table: "DanhMucPIChiTiet");

            migrationBuilder.DropForeignKey(
                name: "FK_KetQuaDanhGia_DanhMucPIChiTiet_DanhMucPIChiTietId",
                table: "KetQuaDanhGia");

            migrationBuilder.DropIndex(
                name: "IX_KetQuaDanhGia_DanhMucPIChiTietId",
                table: "KetQuaDanhGia");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucPIChiTiet_IdDonViDo",
                table: "DanhMucPIChiTiet");

            migrationBuilder.DropColumn(
                name: "DanhMucPIChiTietId",
                table: "KetQuaDanhGia");

            migrationBuilder.DropColumn(
                name: "ChieuHuongTot",
                table: "DanhMucPIChiTiet");

            migrationBuilder.DropColumn(
                name: "HeSoHoanThanhK",
                table: "DanhMucPIChiTiet");

            migrationBuilder.DropColumn(
                name: "IdDonViDo",
                table: "DanhMucPIChiTiet");

            migrationBuilder.AddColumn<int>(
                name: "ChieuHuongTot",
                table: "DanhMucPIs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeSoHoanThanhK",
                table: "DanhMucPIs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdDonViDo",
                table: "DanhMucPIs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIs_IdDonViDo",
                table: "DanhMucPIs",
                column: "IdDonViDo");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPIs_DonViDos_IdDonViDo",
                table: "DanhMucPIs",
                column: "IdDonViDo",
                principalTable: "DonViDos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
