using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhanQuyenDonViKPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonViKPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanQuyenDonViKPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhanQuyenDonViKPIs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhanQuyenDonViKPIs_vptq_kpi_DonViKPIs_DonViKPIId",
                        column: x => x.DonViKPIId,
                        principalTable: "vptq_kpi_DonViKPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyenDonViKPIs_DonViKPIId",
                table: "PhanQuyenDonViKPIs",
                column: "DonViKPIId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyenDonViKPIs_UserId",
                table: "PhanQuyenDonViKPIs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhanQuyenDonViKPIs");
        }
    }
}
