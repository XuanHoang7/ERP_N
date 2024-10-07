using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoAnonymousId",
                table: "CapDuyets");

            migrationBuilder.DropIndex(
                name: "IX_CapDuyets_LanhDaoAnonymousId",
                table: "CapDuyets");

            migrationBuilder.DropColumn(
                name: "LanhDaoAnonymousId",
                table: "CapDuyets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LanhDaoAnonymousId",
                table: "CapDuyets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_LanhDaoAnonymousId",
                table: "CapDuyets",
                column: "LanhDaoAnonymousId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoAnonymousId",
                table: "CapDuyets",
                column: "LanhDaoAnonymousId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
