using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdNhomPI",
                table: "DanhMucPIChiTiet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIChiTiet_IdNhomPI",
                table: "DanhMucPIChiTiet",
                column: "IdNhomPI");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPIChiTiet_DanhMucNhomPIs_IdNhomPI",
                table: "DanhMucPIChiTiet",
                column: "IdNhomPI",
                principalTable: "DanhMucNhomPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPIChiTiet_DanhMucNhomPIs_IdNhomPI",
                table: "DanhMucPIChiTiet");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucPIChiTiet_IdNhomPI",
                table: "DanhMucPIChiTiet");

            migrationBuilder.DropColumn(
                name: "IdNhomPI",
                table: "DanhMucPIChiTiet");
        }
    }
}
