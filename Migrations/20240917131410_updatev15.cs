using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class updatev15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChucDanhs_DanhMucTyTrongs_DanhMucTyTrongId",
                table: "ChucDanhs");

            migrationBuilder.DropIndex(
                name: "IX_ChucDanhs_DanhMucTyTrongId",
                table: "ChucDanhs");

            migrationBuilder.DropColumn(
                name: "DanhMucTyTrongId",
                table: "ChucDanhs");

            migrationBuilder.CreateTable(
                name: "ChucDanhTyTrong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChucDanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTyTrongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ChucDanhTyTrong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChucDanhTyTrong_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanhTyTrong_ChucDanhs_ChucDanhId",
                        column: x => x.ChucDanhId,
                        principalTable: "ChucDanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanhTyTrong_DanhMucTyTrongs_DanhMucTyTrongId",
                        column: x => x.DanhMucTyTrongId,
                        principalTable: "DanhMucTyTrongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanhTyTrong_ChucDanhId",
                table: "ChucDanhTyTrong",
                column: "ChucDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanhTyTrong_CreatedBy",
                table: "ChucDanhTyTrong",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanhTyTrong_DanhMucTyTrongId",
                table: "ChucDanhTyTrong",
                column: "DanhMucTyTrongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChucDanhTyTrong");

            migrationBuilder.AddColumn<Guid>(
                name: "DanhMucTyTrongId",
                table: "ChucDanhs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanhs_DanhMucTyTrongId",
                table: "ChucDanhs",
                column: "DanhMucTyTrongId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChucDanhs_DanhMucTyTrongs_DanhMucTyTrongId",
                table: "ChucDanhs",
                column: "DanhMucTyTrongId",
                principalTable: "DanhMucTyTrongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
