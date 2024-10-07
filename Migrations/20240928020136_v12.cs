using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
