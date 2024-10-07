using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoDanhGiaId",
                table: "CapDuyets");

            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoPhongKPIId",
                table: "CapDuyets");

            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_PheDuyetId",
                table: "CapDuyets");

            migrationBuilder.DropIndex(
                name: "IX_CapDuyets_LanhDaoDanhGiaId",
                table: "CapDuyets");

            migrationBuilder.DropIndex(
                name: "IX_CapDuyets_LanhDaoPhongKPIId",
                table: "CapDuyets");

            migrationBuilder.DropColumn(
                name: "LanhDaoDanhGiaId",
                table: "CapDuyets");

            migrationBuilder.DropColumn(
                name: "LanhDaoPhongKPIId",
                table: "CapDuyets");

            migrationBuilder.RenameColumn(
                name: "PheDuyetId",
                table: "CapDuyets",
                newName: "LanhDaoDuyetId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyets_PheDuyetId",
                table: "CapDuyets",
                newName: "IX_CapDuyets_LanhDaoDuyetId");

            migrationBuilder.AddColumn<int>(
                name: "CacCapDuyet",
                table: "CapDuyets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoDuyetId",
                table: "CapDuyets",
                column: "LanhDaoDuyetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoDuyetId",
                table: "CapDuyets");

            migrationBuilder.DropColumn(
                name: "CacCapDuyet",
                table: "CapDuyets");

            migrationBuilder.RenameColumn(
                name: "LanhDaoDuyetId",
                table: "CapDuyets",
                newName: "PheDuyetId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyets_LanhDaoDuyetId",
                table: "CapDuyets",
                newName: "IX_CapDuyets_PheDuyetId");

            migrationBuilder.AddColumn<Guid>(
                name: "LanhDaoDanhGiaId",
                table: "CapDuyets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LanhDaoPhongKPIId",
                table: "CapDuyets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_LanhDaoDanhGiaId",
                table: "CapDuyets",
                column: "LanhDaoDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_LanhDaoPhongKPIId",
                table: "CapDuyets",
                column: "LanhDaoPhongKPIId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoDanhGiaId",
                table: "CapDuyets",
                column: "LanhDaoDanhGiaId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoPhongKPIId",
                table: "CapDuyets",
                column: "LanhDaoPhongKPIId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_PheDuyetId",
                table: "CapDuyets",
                column: "PheDuyetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
