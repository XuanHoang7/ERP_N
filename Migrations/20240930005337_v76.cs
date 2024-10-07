using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v76 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIDetails_DanhMucPIs_DanhMucPIId",
                table: "KPIDetails");

            migrationBuilder.RenameColumn(
                name: "DanhMucPIId",
                table: "KPIDetails",
                newName: "DanhMucPIChiTietId");

            migrationBuilder.RenameIndex(
                name: "IX_KPIDetails_DanhMucPIId",
                table: "KPIDetails",
                newName: "IX_KPIDetails_DanhMucPIChiTietId");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIDetails_DanhMucPIChiTiet_DanhMucPIChiTietId",
                table: "KPIDetails",
                column: "DanhMucPIChiTietId",
                principalTable: "DanhMucPIChiTiet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIDetails_DanhMucPIChiTiet_DanhMucPIChiTietId",
                table: "KPIDetails");

            migrationBuilder.RenameColumn(
                name: "DanhMucPIChiTietId",
                table: "KPIDetails",
                newName: "DanhMucPIId");

            migrationBuilder.RenameIndex(
                name: "IX_KPIDetails_DanhMucPIChiTietId",
                table: "KPIDetails",
                newName: "IX_KPIDetails_DanhMucPIId");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIDetails_DanhMucPIs_DanhMucPIId",
                table: "KPIDetails",
                column: "DanhMucPIId",
                principalTable: "DanhMucPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
