using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v93 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KPIDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucPIChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAddChiTieuNaw = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_KPIDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIDetail_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIDetail_DanhMucPIChiTiet_DanhMucPIChiTietId",
                        column: x => x.DanhMucPIChiTietId,
                        principalTable: "DanhMucPIChiTiet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KPIDetail_KPIs_KPIId",
                        column: x => x.KPIId,
                        principalTable: "KPIs",
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
                    Tytrong = table.Column<byte>(type: "tinyint", nullable: false),
                    ChiTieuCanDat = table.Column<float>(type: "real", nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIDetailChild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIDetailChild_KPIDetail_KPIDetailId",
                        column: x => x.KPIDetailId,
                        principalTable: "KPIDetail",
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
                name: "IX_KPIDetail_CreatedBy",
                table: "KPIDetail",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetail_DanhMucPIChiTietId",
                table: "KPIDetail",
                column: "DanhMucPIChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetail_KPIId",
                table: "KPIDetail",
                column: "KPIId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetailChild_KPIDetailId",
                table: "KPIDetailChild",
                column: "KPIDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIDetailChild_PIPhuThuocId",
                table: "KPIDetailChild",
                column: "PIPhuThuocId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KPIDetailChild");

            migrationBuilder.DropTable(
                name: "KPIDetail");
        }
    }
}
