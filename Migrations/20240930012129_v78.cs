using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v78 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "KPIDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "KPIDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "KPIDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "KPIDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "KPIDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "KPIDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "KPIDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetails_CreatedBy",
                table: "KPIDetails",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIDetails_AspNetUsers_CreatedBy",
                table: "KPIDetails",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIDetails_AspNetUsers_CreatedBy",
                table: "KPIDetails");

            migrationBuilder.DropIndex(
                name: "IX_KPIDetails_CreatedBy",
                table: "KPIDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "KPIDetails");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "KPIDetails");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "KPIDetails");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "KPIDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "KPIDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "KPIDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "KPIDetails");
        }
    }
}
