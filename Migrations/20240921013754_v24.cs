using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChucDanhs_DoiTuongApDung_DoiTuongApDungId",
                table: "ChucDanhs");

            migrationBuilder.DropIndex(
                name: "IX_ChucDanhs_DoiTuongApDungId",
                table: "ChucDanhs");

            migrationBuilder.DropColumn(
                name: "DoiTuongApDungId",
                table: "ChucDanhs");

            migrationBuilder.AddColumn<Guid>(
                name: "ChucDanhId",
                table: "DoiTuongApDung",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DoiTuongApDung_ChucDanhId",
                table: "DoiTuongApDung",
                column: "ChucDanhId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoiTuongApDung_ChucDanhs_ChucDanhId",
                table: "DoiTuongApDung",
                column: "ChucDanhId",
                principalTable: "ChucDanhs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoiTuongApDung_ChucDanhs_ChucDanhId",
                table: "DoiTuongApDung");

            migrationBuilder.DropIndex(
                name: "IX_DoiTuongApDung_ChucDanhId",
                table: "DoiTuongApDung");

            migrationBuilder.DropColumn(
                name: "ChucDanhId",
                table: "DoiTuongApDung");

            migrationBuilder.AddColumn<Guid>(
                name: "DoiTuongApDungId",
                table: "ChucDanhs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanhs_DoiTuongApDungId",
                table: "ChucDanhs",
                column: "DoiTuongApDungId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChucDanhs_DoiTuongApDung_DoiTuongApDungId",
                table: "ChucDanhs",
                column: "DoiTuongApDungId",
                principalTable: "DoiTuongApDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
