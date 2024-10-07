using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class update_v21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DoiTuongApDungId",
                table: "ChucDanhs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DanhMucPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDMDonViDanhGia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhienBan = table.Column<int>(type: "int", nullable: false),
                    ApDungDen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongPI = table.Column<int>(type: "int", nullable: false),
                    IdDMLanhDaoDonVi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_DanhMucPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucPIs_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPIs_DM_DonViDanhGias_IdDMDonViDanhGia",
                        column: x => x.IdDMDonViDanhGia,
                        principalTable: "DM_DonViDanhGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPIs_DM_LanhDaoDonVis_IdDMLanhDaoDonVi",
                        column: x => x.IdDMLanhDaoDonVi,
                        principalTable: "DM_LanhDaoDonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucPICauHinhDuyet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauHinhDuyetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_DanhMucPICauHinhDuyet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucPICauHinhDuyet_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPICauHinhDuyet_CauHinhDuyets_CauHinhDuyetId",
                        column: x => x.CauHinhDuyetId,
                        principalTable: "CauHinhDuyets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPICauHinhDuyet_DanhMucPIs_DanhMucPIId",
                        column: x => x.DanhMucPIId,
                        principalTable: "DanhMucPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucPIChiTiet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDanhMucPI = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaSo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdDMTrongYeu = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiSoDanhGia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DuLieuThamDinh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdNguoiThamDinh = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChuKy = table.Column<int>(type: "int", nullable: false),
                    ChiTietChiSoDanhGia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThaiSuDung = table.Column<bool>(type: "bit", nullable: false),
                    KieuDanhGia = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_DanhMucPIChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucPIChiTiet_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPIChiTiet_AspNetUsers_IdNguoiThamDinh",
                        column: x => x.IdNguoiThamDinh,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPIChiTiet_DanhMucPIs_IdDanhMucPI",
                        column: x => x.IdDanhMucPI,
                        principalTable: "DanhMucPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPIChiTiet_DanhMucTrongYeus_IdDMTrongYeu",
                        column: x => x.IdDMTrongYeu,
                        principalTable: "DanhMucTrongYeus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoiTuongApDung",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhomChucDanh = table.Column<int>(type: "int", nullable: false),
                    DanhMucPIChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_DoiTuongApDung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoiTuongApDung_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoiTuongApDung_DanhMucPIChiTiet_DanhMucPIChiTietId",
                        column: x => x.DanhMucPIChiTietId,
                        principalTable: "DanhMucPIChiTiet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanhs_DoiTuongApDungId",
                table: "ChucDanhs",
                column: "DoiTuongApDungId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPICauHinhDuyet_CauHinhDuyetId",
                table: "DanhMucPICauHinhDuyet",
                column: "CauHinhDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPICauHinhDuyet_CreatedBy",
                table: "DanhMucPICauHinhDuyet",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPICauHinhDuyet_DanhMucPIId",
                table: "DanhMucPICauHinhDuyet",
                column: "DanhMucPIId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIChiTiet_CreatedBy",
                table: "DanhMucPIChiTiet",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIChiTiet_IdDanhMucPI",
                table: "DanhMucPIChiTiet",
                column: "IdDanhMucPI");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIChiTiet_IdDMTrongYeu",
                table: "DanhMucPIChiTiet",
                column: "IdDMTrongYeu");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIChiTiet_IdNguoiThamDinh",
                table: "DanhMucPIChiTiet",
                column: "IdNguoiThamDinh");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIs_CreatedBy",
                table: "DanhMucPIs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIs_IdDMDonViDanhGia",
                table: "DanhMucPIs",
                column: "IdDMDonViDanhGia");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPIs_IdDMLanhDaoDonVi",
                table: "DanhMucPIs",
                column: "IdDMLanhDaoDonVi");

            migrationBuilder.CreateIndex(
                name: "IX_DoiTuongApDung_CreatedBy",
                table: "DoiTuongApDung",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DoiTuongApDung_DanhMucPIChiTietId",
                table: "DoiTuongApDung",
                column: "DanhMucPIChiTietId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChucDanhs_DoiTuongApDung_DoiTuongApDungId",
                table: "ChucDanhs",
                column: "DoiTuongApDungId",
                principalTable: "DoiTuongApDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChucDanhs_DoiTuongApDung_DoiTuongApDungId",
                table: "ChucDanhs");

            migrationBuilder.DropTable(
                name: "DanhMucPICauHinhDuyet");

            migrationBuilder.DropTable(
                name: "DoiTuongApDung");

            migrationBuilder.DropTable(
                name: "DanhMucPIChiTiet");

            migrationBuilder.DropTable(
                name: "DanhMucPIs");

            migrationBuilder.DropIndex(
                name: "IX_ChucDanhs_DoiTuongApDungId",
                table: "ChucDanhs");

            migrationBuilder.DropColumn(
                name: "DoiTuongApDungId",
                table: "ChucDanhs");
        }
    }
}
