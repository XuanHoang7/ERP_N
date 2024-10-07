using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v90 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KPIDetailChild");

            migrationBuilder.DropTable(
                name: "StreamReviewKPI");

            migrationBuilder.DropTable(
                name: "KPIDetails");

            migrationBuilder.DropTable(
                name: "KPI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KPI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DM_DonViDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleteApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsIndividual = table.Column<bool>(type: "bit", nullable: false),
                    IsRefuse = table.Column<bool>(type: "bit", nullable: false),
                    IsRefuseForApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    NoteForApprove = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForRefuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumberNow = table.Column<byte>(type: "tinyint", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WillApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPI_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPI_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPI_DM_DonViDanhGias_DM_DonViDanhGiaId",
                        column: x => x.DM_DonViDanhGiaId,
                        principalTable: "DM_DonViDanhGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KPIDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucPIChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAddChiTieuNaw = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ThoiDiemDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIDetails_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIDetails_DanhMucPIChiTiet_DanhMucPIChiTietId",
                        column: x => x.DanhMucPIChiTietId,
                        principalTable: "DanhMucPIChiTiet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIDetails_KPI_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StreamReviewKPI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamReviewKPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamReviewKPI_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StreamReviewKPI_KPI_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KPIDetailChild",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PIPhuThuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIDetailChild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIDetailChild_KPIDetails_KPIDetailId",
                        column: x => x.KPIDetailId,
                        principalTable: "KPIDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIDetailChild_PIPhuThuoc_PIPhuThuocId",
                        column: x => x.PIPhuThuocId,
                        principalTable: "PIPhuThuoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KPI_CreatedBy",
                table: "KPI",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KPI_DM_DonViDanhGiaId",
                table: "KPI",
                column: "DM_DonViDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_KPI_UserId",
                table: "KPI",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetailChild_KPIDetailId",
                table: "KPIDetailChild",
                column: "KPIDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetailChild_PIPhuThuocId",
                table: "KPIDetailChild",
                column: "PIPhuThuocId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetails_CreatedBy",
                table: "KPIDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetails_DanhMucPIChiTietId",
                table: "KPIDetails",
                column: "DanhMucPIChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetails_KPIId",
                table: "KPIDetails",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewKPI_KPIId",
                table: "StreamReviewKPI",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewKPI_UserId",
                table: "StreamReviewKPI",
                column: "UserId");
        }
    }
}
