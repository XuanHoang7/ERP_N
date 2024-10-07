using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v73 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "StreamReviewKPI");

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
                    table.PrimaryKey("PK_StreamReviewOrApproveKPI", x => x.Id);
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
            //migrationBuilder.DropTable(
            //    name: "StreamReviewKPI");

            migrationBuilder.CreateTable(
                name: "StreamReviewKPI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproveKPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
