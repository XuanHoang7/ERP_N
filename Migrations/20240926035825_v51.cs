using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v51 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PhanQuyenDonViKPIs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PhanQuyenDonViKPIs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "PhanQuyenDonViKPIs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "PhanQuyenDonViKPIs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PhanQuyenDonViKPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "PhanQuyenDonViKPIs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "PhanQuyenDonViKPIs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyenDonViKPIs_CreatedBy",
                table: "PhanQuyenDonViKPIs",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanQuyenDonViKPIs_AspNetUsers_CreatedBy",
                table: "PhanQuyenDonViKPIs",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhanQuyenDonViKPIs_AspNetUsers_CreatedBy",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropIndex(
                name: "IX_PhanQuyenDonViKPIs_CreatedBy",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PhanQuyenDonViKPIs");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "PhanQuyenDonViKPIs");
        }
    }
}
