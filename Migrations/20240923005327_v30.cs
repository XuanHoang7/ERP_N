using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChieuHuongTot",
                table: "DanhMucPIs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeSoHoanThanhK",
                table: "DanhMucPIs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdDonViDo",
                table: "DanhMucPIs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KetQuaDanhGia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDanhMucPI = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDMKetQuaDanhGia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KhoangGiaTriK = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaDanhGiaSo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaDanhGiaSo_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KetQuaDanhGiaSo_DM_KetQuaDanhGias_IdDMKetQuaDanhGia",
                        column: x => x.IdDMKetQuaDanhGia,
                        principalTable: "DM_KetQuaDanhGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KetQuaDanhGiaSo_DanhMucPIs_IdDanhMucPI",
                        column: x => x.IdDanhMucPI,
                        principalTable: "DanhMucPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIs_IdDonViDo",
                table: "DanhMucPIs",
                column: "IdDonViDo");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDanhGiaSo_CreatedBy",
                table: "KetQuaDanhGia",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDanhGiaSo_IdDanhMucPI",
                table: "KetQuaDanhGia",
                column: "IdDanhMucPI");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDanhGiaSo_IdDMKetQuaDanhGia",
                table: "KetQuaDanhGia",
                column: "IdDMKetQuaDanhGia");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucPIs_DonViDos_IdDonViDo",
                table: "DanhMucPIs",
                column: "IdDonViDo",
                principalTable: "DonViDos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucPIs_DonViDos_IdDonViDo",
                table: "DanhMucPIs");

            migrationBuilder.DropTable(
                name: "KetQuaDanhGia");

            migrationBuilder.DropIndex(
                name: "IX_DanhMucPIs_IdDonViDo",
                table: "DanhMucPIs");

            migrationBuilder.DropColumn(
                name: "ChieuHuongTot",
                table: "DanhMucPIs");

            migrationBuilder.DropColumn(
                name: "HeSoHoanThanhK",
                table: "DanhMucPIs");

            migrationBuilder.DropColumn(
                name: "IdDonViDo",
                table: "DanhMucPIs");
        }
    }
}
