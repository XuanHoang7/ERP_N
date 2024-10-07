using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuyetPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsRefuse = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForRefuse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuyetPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuyetPIs_DanhMucPIs_DanhMucPIId",
                        column: x => x.DanhMucPIId,
                        principalTable: "DanhMucPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DuyetPIStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DuyetPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsBrowse = table.Column<bool>(type: "bit", nullable: false),
                    Serial = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuyetPIStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuyetPIStatus_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuyetPIStatus_DuyetPIs_DuyetPIId",
                        column: x => x.DuyetPIId,
                        principalTable: "DuyetPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuyetPIs_DanhMucPIId",
                table: "DuyetPIs",
                column: "DanhMucPIId");

            migrationBuilder.CreateIndex(
                name: "IX_DuyetPIStatus_DuyetPIId",
                table: "DuyetPIStatus",
                column: "DuyetPIId");

            migrationBuilder.CreateIndex(
                name: "IX_DuyetPIStatus_UserId",
                table: "DuyetPIStatus",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuyetPIStatus");

            migrationBuilder.DropTable(
                name: "DuyetPIs");
        }
    }
}
