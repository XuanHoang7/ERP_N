using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyet_AspNetUsers_CreatedBy",
                table: "CapDuyet");

            migrationBuilder.DropIndex(
                name: "IX_CapDuyet_CreatedBy",
                table: "CapDuyet");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CapDuyet");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CapDuyet");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CapDuyet");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "CapDuyet");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CapDuyet");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CapDuyet");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "CapDuyet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "CapDuyet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CapDuyet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "CapDuyet",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "CapDuyet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CapDuyet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "CapDuyet",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "CapDuyet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyet_CreatedBy",
                table: "CapDuyet",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyet_AspNetUsers_CreatedBy",
                table: "CapDuyet",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
