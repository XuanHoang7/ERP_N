using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class update_v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NamDanhGia",
                table: "NSKhongDanhGias");

            migrationBuilder.AlterColumn<string>(
                name: "ThoiDiemDanhGia",
                table: "NSKhongDanhGias",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ThoiDiemDanhGia",
                table: "NSKhongDanhGias",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NamDanhGia",
                table: "NSKhongDanhGias",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
