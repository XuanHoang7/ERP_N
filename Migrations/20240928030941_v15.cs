using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_CreatedBy",
                table: "CapDuyets");

            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoDuyetId",
                table: "CapDuyets");

            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyets_AspNetUsers_NhanVienId",
                table: "CapDuyets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapDuyets",
                table: "CapDuyets");

            migrationBuilder.RenameTable(
                name: "CapDuyets",
                newName: "CapDuyet");

            migrationBuilder.RenameColumn(
                name: "NhanVienId",
                table: "CapDuyet",
                newName: "DanhMucDuyetId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyets_NhanVienId",
                table: "CapDuyet",
                newName: "IX_CapDuyet_DanhMucDuyetId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyets_LanhDaoDuyetId",
                table: "CapDuyet",
                newName: "IX_CapDuyet_LanhDaoDuyetId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyets_CreatedBy",
                table: "CapDuyet",
                newName: "IX_CapDuyet_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapDuyet",
                table: "CapDuyet",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DanhMucDuyets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhanVienId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucDuyets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucDuyets_AspNetUsers_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucDuyets_NhanVienId",
                table: "DanhMucDuyets",
                column: "NhanVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyet_AspNetUsers_CreatedBy",
                table: "CapDuyet",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyet_AspNetUsers_LanhDaoDuyetId",
                table: "CapDuyet",
                column: "LanhDaoDuyetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyet_DanhMucDuyets_DanhMucDuyetId",
                table: "CapDuyet",
                column: "DanhMucDuyetId",
                principalTable: "DanhMucDuyets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyet_AspNetUsers_CreatedBy",
                table: "CapDuyet");

            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyet_AspNetUsers_LanhDaoDuyetId",
                table: "CapDuyet");

            migrationBuilder.DropForeignKey(
                name: "FK_CapDuyet_DanhMucDuyets_DanhMucDuyetId",
                table: "CapDuyet");

            migrationBuilder.DropTable(
                name: "DanhMucDuyets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapDuyet",
                table: "CapDuyet");

            migrationBuilder.RenameTable(
                name: "CapDuyet",
                newName: "CapDuyets");

            migrationBuilder.RenameColumn(
                name: "DanhMucDuyetId",
                table: "CapDuyets",
                newName: "NhanVienId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyet_LanhDaoDuyetId",
                table: "CapDuyets",
                newName: "IX_CapDuyets_LanhDaoDuyetId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyet_DanhMucDuyetId",
                table: "CapDuyets",
                newName: "IX_CapDuyets_NhanVienId");

            migrationBuilder.RenameIndex(
                name: "IX_CapDuyet_CreatedBy",
                table: "CapDuyets",
                newName: "IX_CapDuyets_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapDuyets",
                table: "CapDuyets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_CreatedBy",
                table: "CapDuyets",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_LanhDaoDuyetId",
                table: "CapDuyets",
                column: "LanhDaoDuyetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CapDuyets_AspNetUsers_NhanVienId",
                table: "CapDuyets",
                column: "NhanVienId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
