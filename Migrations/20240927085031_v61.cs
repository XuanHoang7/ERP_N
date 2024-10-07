using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v61 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CapDuyets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhanVienId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanhDaoDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanhDaoAnonymousId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanhDaoPhongKPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PheDuyetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_CapDuyets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapDuyets_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapDuyets_AspNetUsers_LanhDaoAnonymousId",
                        column: x => x.LanhDaoAnonymousId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapDuyets_AspNetUsers_LanhDaoDanhGiaId",
                        column: x => x.LanhDaoDanhGiaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapDuyets_AspNetUsers_LanhDaoPhongKPIId",
                        column: x => x.LanhDaoPhongKPIId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapDuyets_AspNetUsers_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapDuyets_AspNetUsers_PheDuyetId",
                        column: x => x.PheDuyetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KPI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsIndividual = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DM_DonViDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    IsRefuse = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForRefuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumberNow = table.Column<byte>(type: "tinyint", nullable: false),
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
                name: "ApproveKPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRefuse = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForRefuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumberNow = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveKPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApproveKPIs_KPI_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KPIDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chuky = table.Column<int>(type: "int", nullable: false),
                    ThoiDiemDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhMucPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAddChiTieuNaw = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIDetails_DanhMucPIs_DanhMucPIId",
                        column: x => x.DanhMucPIId,
                        principalTable: "DanhMucPIs",
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
                    ApproveKPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamReviewOrApproveKPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamReviewOrApproveKPI_ApproveKPIs_ApproveKPIId",
                        column: x => x.ApproveKPIId,
                        principalTable: "ApproveKPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StreamReviewOrApproveKPI_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StreamReviewOrApproveKPI_KPI_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApproveKPIs_KPIId",
                table: "ApproveKPIs",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_CreatedBy",
                table: "CapDuyets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_LanhDaoAnonymousId",
                table: "CapDuyets",
                column: "LanhDaoAnonymousId");

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_LanhDaoDanhGiaId",
                table: "CapDuyets",
                column: "LanhDaoDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_LanhDaoPhongKPIId",
                table: "CapDuyets",
                column: "LanhDaoPhongKPIId");

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_NhanVienId",
                table: "CapDuyets",
                column: "NhanVienId");

            migrationBuilder.CreateIndex(
                name: "IX_CapDuyets_PheDuyetId",
                table: "CapDuyets",
                column: "PheDuyetId");

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
                name: "IX_KPIDetails_DanhMucPIId",
                table: "KPIDetails",
                column: "DanhMucPIId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetails_KPIId",
                table: "KPIDetails",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewOrApproveKPI_ApproveKPIId",
                table: "StreamReviewKPI",
                column: "ApproveKPIId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewOrApproveKPI_KPIId",
                table: "StreamReviewKPI",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewOrApproveKPI_UserId",
                table: "StreamReviewKPI",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapDuyets");

            migrationBuilder.DropTable(
                name: "KPIDetails");

            migrationBuilder.DropTable(
                name: "StreamReviewKPI");

            migrationBuilder.DropTable(
                name: "ApproveKPIs");

            migrationBuilder.DropTable(
                name: "KPI");
        }
    }
}
