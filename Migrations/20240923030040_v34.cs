using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaDanhGia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KetQuaDanhGia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDanhMucPIChiTiet = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDMKetQuaDanhGia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    KhoangGiaTriK = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaDanhGia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaDanhGia_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KetQuaDanhGia_DM_KetQuaDanhGias_IdDMKetQuaDanhGia",
                        column: x => x.IdDMKetQuaDanhGia,
                        principalTable: "DM_KetQuaDanhGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KetQuaDanhGia_DanhMucPIChiTiet_IdDanhMucPIChiTiet",
                        column: x => x.IdDanhMucPIChiTiet,
                        principalTable: "DanhMucPIChiTiet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDanhGia_CreatedBy",
                table: "KetQuaDanhGia",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDanhGia_IdDanhMucPIChiTiet",
                table: "KetQuaDanhGia",
                column: "IdDanhMucPIChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaDanhGia_IdDMKetQuaDanhGia",
                table: "KetQuaDanhGia",
                column: "IdDMKetQuaDanhGia");
        }
    }
}
