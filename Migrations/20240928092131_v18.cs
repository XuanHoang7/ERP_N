using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "NhanVienId",
                table: "DanhMucDuyets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "DM_DonViDanhGiaId",
                table: "DanhMucDuyets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCaNhan",
                table: "DanhMucDuyets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucDuyets_DM_DonViDanhGiaId",
                table: "DanhMucDuyets",
                column: "DM_DonViDanhGiaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucDuyets_DM_DonViDanhGias_DM_DonViDanhGiaId",
                table: "DanhMucDuyets",
                column: "DM_DonViDanhGiaId",
                principalTable: "DM_DonViDanhGias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucDuyets_DM_DonViDanhGias_DM_DonViDanhGiaId",
                table: "DanhMucDuyets");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucDuyets_DM_DonViDanhGiaId",
                table: "DanhMucDuyets");

            migrationBuilder.DropColumn(
                name: "DM_DonViDanhGiaId",
                table: "DanhMucDuyets");

            migrationBuilder.DropColumn(
                name: "IsCaNhan",
                table: "DanhMucDuyets");

            migrationBuilder.AlterColumn<Guid>(
                name: "NhanVienId",
                table: "DanhMucDuyets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
