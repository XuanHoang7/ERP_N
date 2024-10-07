using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhMucPICauHinhDuyet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucPICauHinhDuyet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CauHinhDuyetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucPIId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucPICauHinhDuyet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhMucPICauHinhDuyet_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPICauHinhDuyet_CauHinhDuyets_CauHinhDuyetId",
                        column: x => x.CauHinhDuyetId,
                        principalTable: "CauHinhDuyets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhMucPICauHinhDuyet_DanhMucPIs_DanhMucPIId",
                        column: x => x.DanhMucPIId,
                        principalTable: "DanhMucPIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPICauHinhDuyet_CauHinhDuyetId",
                table: "DanhMucPICauHinhDuyet",
                column: "CauHinhDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPICauHinhDuyet_CreatedBy",
                table: "DanhMucPICauHinhDuyet",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucPICauHinhDuyet_DanhMucPIId",
                table: "DanhMucPICauHinhDuyet",
                column: "DanhMucPIId");
        }
    }
}
