using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class updatedanhmuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhDuyets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhanVienId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    CapDuyet = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
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
                    table.PrimaryKey("PK_CauHinhDuyets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHinhDuyets_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CauHinhDuyets_AspNetUsers_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucNhomPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDanhMucNhomPI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenDanhMucNhomPI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_DanhMucNhomPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucNhomPIs_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucTyTrongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChuKyDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NhomChiTieuSXKD = table.Column<double>(type: "float", nullable: false),
                    NhomChiTieuCTQT = table.Column<double>(type: "float", nullable: false),
                    BatBuocDung = table.Column<bool>(type: "bit", nullable: false),
                    IsKhong = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_DanhMucTyTrongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucTyTrongs_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucUyQuyens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanhDaoUpQuyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_DanhMucUyQuyens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucUyQuyens_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucUyQuyens_AspNetUsers_LanhDaoUpQuyenId",
                        column: x => x.LanhDaoUpQuyenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DM_LanhDaoDonVis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanhDaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_DM_LanhDaoDonVis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DM_LanhDaoDonVis_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DM_LanhDaoDonVis_AspNetUsers_LanhDaoId",
                        column: x => x.LanhDaoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DM_LanhDaoDonVis_DonVis_DonViID",
                        column: x => x.DonViID,
                        principalTable: "DonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonViDos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDonViDo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenDonViDo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_DonViDos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonViDos_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhomChucDanh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenNhomChucDanh = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomChucDanh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NSKhongDanhGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChuKyDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiDiemDanhGia = table.Column<int>(type: "int", nullable: false),
                    DonViID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_NSKhongDanhGias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NSKhongDanhGias_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NSKhongDanhGias_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NSKhongDanhGias_DonVis_DonViID",
                        column: x => x.DonViID,
                        principalTable: "DonVis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DuocUyQuyen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanhDaoDuocUyQuyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucUyQuyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_DuocUyQuyen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuocUyQuyen_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuocUyQuyen_AspNetUsers_LanhDaoDuocUyQuyenId",
                        column: x => x.LanhDaoDuocUyQuyenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuocUyQuyen_DanhMucUyQuyens_DanhMucUyQuyenId",
                        column: x => x.DanhMucUyQuyenId,
                        principalTable: "DanhMucUyQuyens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChucDanh_NhomChucDanh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChucDanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhomChucDanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTyTrongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ChucDanh_NhomChucDanh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_ChucDanhs_ChucDanhId",
                        column: x => x.ChucDanhId,
                        principalTable: "ChucDanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_DanhMucTyTrongs_DanhMucTyTrongId",
                        column: x => x.DanhMucTyTrongId,
                        principalTable: "DanhMucTyTrongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_NhomChucDanh_NhomChucDanhId",
                        column: x => x.NhomChucDanhId,
                        principalTable: "NhomChucDanh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhDuyets_CreatedBy",
                table: "CauHinhDuyets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhDuyets_NhanVienId",
                table: "CauHinhDuyets",
                column: "NhanVienId");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_ChucDanhId",
                table: "ChucDanh_NhomChucDanh",
                column: "ChucDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_CreatedBy",
                table: "ChucDanh_NhomChucDanh",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_DanhMucTyTrongId",
                table: "ChucDanh_NhomChucDanh",
                column: "DanhMucTyTrongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_NhomChucDanhId",
                table: "ChucDanh_NhomChucDanh",
                column: "NhomChucDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucNhomPIs_CreatedBy",
                table: "DanhMucNhomPIs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucTyTrongs_CreatedBy",
                table: "DanhMucTyTrongs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucUyQuyens_CreatedBy",
                table: "DanhMucUyQuyens",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucUyQuyens_LanhDaoUpQuyenId",
                table: "DanhMucUyQuyens",
                column: "LanhDaoUpQuyenId");

            migrationBuilder.CreateIndex(
                name: "IX_DM_LanhDaoDonVis_CreatedBy",
                table: "DM_LanhDaoDonVis",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DM_LanhDaoDonVis_DonViID",
                table: "DM_LanhDaoDonVis",
                column: "DonViID");

            migrationBuilder.CreateIndex(
                name: "IX_DM_LanhDaoDonVis_LanhDaoId",
                table: "DM_LanhDaoDonVis",
                column: "LanhDaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DonViDos_CreatedBy",
                table: "DonViDos",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DuocUyQuyen_CreatedBy",
                table: "DuocUyQuyen",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DuocUyQuyen_DanhMucUyQuyenId",
                table: "DuocUyQuyen",
                column: "DanhMucUyQuyenId");

            migrationBuilder.CreateIndex(
                name: "IX_DuocUyQuyen_LanhDaoDuocUyQuyenId",
                table: "DuocUyQuyen",
                column: "LanhDaoDuocUyQuyenId");

            migrationBuilder.CreateIndex(
                name: "IX_NSKhongDanhGias_ApplicationUserId",
                table: "NSKhongDanhGias",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NSKhongDanhGias_CreatedBy",
                table: "NSKhongDanhGias",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NSKhongDanhGias_DonViID",
                table: "NSKhongDanhGias",
                column: "DonViID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhDuyets");

            migrationBuilder.DropTable(
                name: "ChucDanh_NhomChucDanh");

            migrationBuilder.DropTable(
                name: "DanhMucNhomPIs");

            migrationBuilder.DropTable(
                name: "DM_LanhDaoDonVis");

            migrationBuilder.DropTable(
                name: "DonViDos");

            migrationBuilder.DropTable(
                name: "DuocUyQuyen");

            migrationBuilder.DropTable(
                name: "NSKhongDanhGias");

            migrationBuilder.DropTable(
                name: "DanhMucTyTrongs");

            migrationBuilder.DropTable(
                name: "NhomChucDanh");

            migrationBuilder.DropTable(
                name: "DanhMucUyQuyens");
        }
    }
}
