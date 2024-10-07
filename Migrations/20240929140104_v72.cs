using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v72 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamReviewOrApproveKPI_ApproveKPIs_ApproveKPIId",
                table: "StreamReviewKPI");

            migrationBuilder.DropTable(
                name: "ApproveKPIs");

            migrationBuilder.DropIndex(
                name: "IX_StreamReviewOrApproveKPI_ApproveKPIId",
                table: "StreamReviewKPI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_StreamReviewOrApproveKPI_ApproveKPIId",
                table: "StreamReviewKPI",
                column: "ApproveKPIId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveKPIs_KPIId",
                table: "ApproveKPIs",
                column: "KPIId");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamReviewOrApproveKPI_ApproveKPIs_ApproveKPIId",
                table: "StreamReviewKPI",
                column: "ApproveKPIId",
                principalTable: "ApproveKPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
