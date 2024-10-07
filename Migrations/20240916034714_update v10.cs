using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class updatev10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "ChucDanh_NhomChucDanh");

            //migrationBuilder.DropTable(
            //    name: "NhomChucDanh");

            migrationBuilder.AddColumn<int>(
                name: "NamDanhGia",
                table: "NSKhongDanhGias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NhomChucDanh",
                table: "DanhMucTyTrongs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenDanhMucTrongYeu",
                table: "DanhMucTrongYeus",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "MaDanhMucTrongYeu",
                table: "DanhMucTrongYeus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "TenDanhMucNhomPI",
                table: "DanhMucNhomPIs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "MaDanhMucNhomPI",
                table: "DanhMucNhomPIs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChucDanhs_DanhMucTyTrongs_DanhMucTyTrongId",
                table: "ChucDanhs");

            migrationBuilder.DropIndex(
                name: "IX_ChucDanhs_DanhMucTyTrongId",
                table: "ChucDanhs");

            migrationBuilder.DropColumn(
                name: "NamDanhGia",
                table: "NSKhongDanhGias");

            migrationBuilder.DropColumn(
                name: "NhomChucDanh",
                table: "DanhMucTyTrongs");

            migrationBuilder.DropColumn(
                name: "DanhMucTyTrongId",
                table: "ChucDanhs");

            migrationBuilder.AlterColumn<string>(
                name: "TenDanhMucTrongYeu",
                table: "DanhMucTrongYeus",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaDanhMucTrongYeu",
                table: "DanhMucTrongYeus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenDanhMucNhomPI",
                table: "DanhMucNhomPIs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaDanhMucNhomPI",
                table: "DanhMucNhomPIs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "NhomChucDanh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenNhomChucDanh = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomChucDanh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChucDanh_NhomChucDanh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChucDanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucTyTrongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhomChucDanhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucDanh_NhomChucDanh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_ChucDanhs_ChucDanhId",
                        column: x => x.ChucDanhId,
                        principalTable: "ChucDanhs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_DanhMucTyTrongs_DanhMucTyTrongId",
                        column: x => x.DanhMucTyTrongId,
                        principalTable: "DanhMucTyTrongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChucDanh_NhomChucDanh_NhomChucDanh_NhomChucDanhId",
                        column: x => x.NhomChucDanhId,
                        principalTable: "NhomChucDanh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_ChucDanhId",
                table: "ChucDanh_NhomChucDanh",
                column: "ChucDanhId");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_CreatedBy",
                table: "ChucDanh_NhomChucDanh",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_DanhMucTyTrongId",
                table: "ChucDanh_NhomChucDanh",
                column: "DanhMucTyTrongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChucDanh_NhomChucDanh_NhomChucDanhId",
                table: "ChucDanh_NhomChucDanh",
                column: "NhomChucDanhId");
        }
    }
}
