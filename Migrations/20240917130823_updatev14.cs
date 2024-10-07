using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class updatev14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ChiTieuTyTrong",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ChiTieuTyTrong",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ChiTieuTyTrong",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ChiTieuTyTrong",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChiTieuTyTrong",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "ChiTieuTyTrong",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ChiTieuTyTrong",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTieuTyTrong_CreatedBy",
                table: "ChiTieuTyTrong",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTieuTyTrong_AspNetUsers_CreatedBy",
                table: "ChiTieuTyTrong",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTieuTyTrong_AspNetUsers_CreatedBy",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropIndex(
                name: "IX_ChiTieuTyTrong_CreatedBy",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ChiTieuTyTrong");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ChiTieuTyTrong");
        }
    }
}
