using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v91 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsIndividual = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DM_DonViDanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    IsRefuse = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForRefuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WillApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleteApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRefuseForApproved = table.Column<bool>(type: "bit", nullable: false),
                    NoteForApprove = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumberNow = table.Column<byte>(type: "tinyint", nullable: false),
                    ThoiDiemDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhMucPIChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: true),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAddChiTieuNam = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_KPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIs_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIs_DM_DonViDanhGias_DM_DonViDanhGiaId",
                        column: x => x.DM_DonViDanhGiaId,
                        principalTable: "DM_DonViDanhGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIs_DanhMucPIChiTiet_DanhMucPIChiTietId",
                        column: x => x.DanhMucPIChiTietId,
                        principalTable: "DanhMucPIChiTiet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KPIChild",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PIPhuThuocId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIChild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIChild_KPIs_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIChild_PIPhuThuoc_PIPhuThuocId",
                        column: x => x.PIPhuThuocId,
                        principalTable: "PIPhuThuoc",
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
                        name: "FK_StreamReviewKPI_KPIs_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KPIChild_KPIId",
                table: "KPIChild",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIChild_PIPhuThuocId",
                table: "KPIChild",
                column: "PIPhuThuocId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_CreatedBy",
                table: "KPIs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_DanhMucPIChiTietId",
                table: "KPIs",
                column: "DanhMucPIChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_DM_DonViDanhGiaId",
                table: "KPIs",
                column: "DM_DonViDanhGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_UserId",
                table: "KPIs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewKPI_KPIId",
                table: "StreamReviewKPI",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewKPI_UserId",
                table: "StreamReviewKPI",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KPIChild");

            migrationBuilder.DropTable(
                name: "StreamReviewKPI");

            migrationBuilder.DropTable(
                name: "KPIs");
        }
    }
}
