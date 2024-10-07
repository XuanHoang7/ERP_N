using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class updatev13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NhomChiTieuCTQT",
                table: "DanhMucTyTrongs");

            migrationBuilder.DropColumn(
                name: "NhomChiTieuSXKD",
                table: "DanhMucTyTrongs");

            migrationBuilder.CreateTable(
                name: "ChiTieuTyTrong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucNhomPiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTyTrongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTieu = table.Column<float>(type: "real", nullable: false),
                    ToanTu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTieuTyTrong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTieuTyTrong_DanhMucNhomPIs_DanhMucNhomPiId",
                        column: x => x.DanhMucNhomPiId,
                        principalTable: "DanhMucNhomPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTieuTyTrong_DanhMucTyTrongs_DanhMucTyTrongId",
                        column: x => x.DanhMucTyTrongId,
                        principalTable: "DanhMucTyTrongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTieuTyTrong_DanhMucNhomPiId",
                table: "ChiTieuTyTrong",
                column: "DanhMucNhomPiId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTieuTyTrong_DanhMucTyTrongId",
                table: "ChiTieuTyTrong",
                column: "DanhMucTyTrongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTieuTyTrong");

            migrationBuilder.AddColumn<double>(
                name: "NhomChiTieuCTQT",
                table: "DanhMucTyTrongs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NhomChiTieuSXKD",
                table: "DanhMucTyTrongs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
