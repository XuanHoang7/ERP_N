using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using System.Threading.Tasks;
using System.Data;
using System;
using System.Linq;
using ERP.Models;
using System.Data.SqlClient;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using ERP.Infrastructure;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.IdentityModel.Tokens;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NS_KhongDanhGiaController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public NS_KhongDanhGiaController(IConfiguration _configuration, 
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitofWork)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitofWork;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNSKhongDanhGiaById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("id not null or empty");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteNSKhongDanhGiaById");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            dbAdapter.runStoredNoneQuery();
            var result =  dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            //var result = await _context.Database.ExecuteSqlRawAsync("sp_DeleteNSKhongDanhGiaById @p0", id);

            return Ok(new { message = "Xóa  thành công" });
        }

        [HttpPost("Search")]
        public IActionResult GetAllByKeyword([FromBody] NSKhongDanhGiaDTO dto, int page = 1, int pageSize = 10)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_SearchNSKhongDanhGia");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", dto.Keyword);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViId", dto.IdDonVi);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@PhongBanId", dto.IdPhongBan);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKyDanhGia", dto.ChuKyDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiemDanhGia", dto.ThoiDiemDanhGia);

            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            int totalRow = result.Count;
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPage)
            {
                page = totalPage;
            }
            var datalist = result.Skip((page - 1) * pageSize).Take(pageSize);
            dbAdapter.deConnect();
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                datalist
            });
        }
        [HttpPost("xuatExcel")]
        public IActionResult XuatExcel([FromBody] NSKhongDanhGiaDTO dto, int page = 1, int pageSize = 10)
        {
            // Call the GetAllByKeyword function to get the list of data
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_SearchNSKhongDanhGia");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", dto.Keyword);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViId", dto.IdDonVi);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@PhongBanId", dto.IdPhongBan);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKyDanhGia", dto.ChuKyDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiemDanhGia", dto.ThoiDiemDanhGia);

            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachNhanVien");

                // Thiết lập tiêu đề bảng
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã nhân viên";
                worksheet.Cells[1, 3].Value = "Họ và tên";
                worksheet.Cells[1, 4].Value = "Chu kỳ đánh giá";
                worksheet.Cells[1, 5].Value = "Thời điểm đánh giá";
                worksheet.Cells[1, 6].Value = "Đơn vị";

                // Tạo viền cho header
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 102, 204)); // Màu xanh header
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White); // Màu chữ trắng
                }

                worksheet.Column(1).Width = 10; // Cột STT
                worksheet.Column(2).Width = 25; // Cột Mã nhân viên
                worksheet.Column(3).Width = 35; // Cột Họ và tên
                worksheet.Column(4).Width = 20; // Cột Chu kỳ đánh giá
                worksheet.Column(5).Width = 20; // Cột Thời điểm đánh giá
                worksheet.Column(6).Width = 25;
                // Điền dữ liệu vào các hàng bắt đầu từ hàng 2
                int rowIndex = 2;
                for (int i = 0; i < result.Count; i++)
                {
                    // Use dynamic access since the list is of type object
                    var record = result[i] as IDictionary<string, object>;

                    worksheet.Cells[rowIndex, 1].Value = (i + 1).ToString(); // STT
                    worksheet.Cells[rowIndex, 2].Value = record["maNhanVien"]?.ToString(); // Mã nhân viên
                    worksheet.Cells[rowIndex, 3].Value = record["hoVaTen"]?.ToString(); // Họ và tên
                    worksheet.Cells[rowIndex, 4].Value = record["chuKyDanhGia"]?.ToString(); // Chu kỳ đánh giá
                    worksheet.Cells[rowIndex, 5].Value = record["thoiDiemDanhGia"]?.ToString(); // Thời điểm đánh giá
                    worksheet.Cells[rowIndex, 6].Value = record["donVi"]?.ToString(); // Đơn vị

                    using (var range = worksheet.Cells[rowIndex, 1, rowIndex, 6])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    rowIndex++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Trả về file Excel
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhMucNSKhongDanhGia.xlsx");
            }
        }
        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            // Set the license context for EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhMucTrongYeuTemplate");

                // Set header row
                worksheet.Cells[1, 1].Value = "Id nhân viên";
                worksheet.Cells[1, 2].Value = "Chu kỳ đánh giá";
                worksheet.Cells[1, 3].Value = "Thời điểm đánh giá";

                // Add some formatting for header
                using (var range = worksheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 102, 204)); // Màu xanh header
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White); // Màu chữ trắng
                }

                worksheet.Column(1).Width = 50; 
                worksheet.Column(2).Width = 50; 
                worksheet.Column(3).Width = 30; 

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Return the Excel file as a downloadable response
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhMucNSKhongDanhGiaTemplate.xlsx");
            }
        }

        [HttpPost("Import")]
        public IActionResult ImportNSKhongDanhGia([FromBody] List<ClassNSKhongDanhGia> nSKhongDanhGias)
        {
            if (nSKhongDanhGias == null || nSKhongDanhGias.Count == 0)
                return BadRequest("Không có dữ liệu");
            var danhMucList = new List<dynamic>();
            var danhMucNotError = new List<dynamic>();
            foreach (var d in nSKhongDanhGias)
            {
               
                var ghichu = "";
                if (string.IsNullOrWhiteSpace(d.MaNhanVien))
                    ghichu += "Id nhân viên không được null hay empty;";
                else
                {
                    if (danhMucNotError.Exists(dm =>
                         dm.MaNhanVien == d.MaNhanVien
                         && dm.ChuKyDanhGia == d.ChuKyDanhGia
                         && dm.ThoiDiemDanhGia == d.ThoiDiemDanhGia))
                        ghichu += "Dữ liệu đã trùng trong danh sách import;";
                    else
                    {
                        danhMucNotError.Add(new
                        {
                            d.MaNhanVien,
                            d.ChuKyDanhGia,
                            d.ThoiDiemDanhGia
                        });
                    }
                    var nhanVien = _uow.Users.FirstOrDefault(x => x.MaNhanVien == d.MaNhanVien && !x.IsDeleted);
                    if(nhanVien == null)
                        ghichu += "Không tồn tại mã nhân viên trong database;";
                    if (_uow.DanhMucNSKhongDanhGias.Exists(dm =>
                         dm.ApplicationUserId == nhanVien.Id
                         && dm.ChuKyDanhGia == d.ChuKyDanhGia
                         && dm.ThoiDiemDanhGia == d.ThoiDiemDanhGia
                         && !dm.IsDeleted))
                        ghichu += "Dữ liệu đã tồn tại trong database;";
                    
                    if (nhanVien.vptq_kpi_DonViKPI_Id == null)
                        ghichu += "Id nhân viên này không tồn tại donvi KPI";
                }
                switch (d.ChuKyDanhGia)
                {
                    case "Tháng":
                        string[] parts = d.ThoiDiemDanhGia.Split('/');
                        int month;
                        int year;
                        if (d.ThoiDiemDanhGia == null || !int.TryParse(parts[0], out month) || !int.TryParse(parts[1], out year))
                            ghichu += "Thời điểmm đánh giá không hợp lệ (phải theo định dạng tháng/năm);";
                        else if (d.ThoiDiemDanhGia == null || month < 1 || month > 12 || year < 1900)
                            ghichu += "Thời điểm đánh giá tháng không hợp lệ (phải từ 1 đến 12) và year > 1900;";
                        break;
                    case "Năm":
                        if (d.ThoiDiemDanhGia == null || !int.TryParse(d.ThoiDiemDanhGia, out year) || year < 1900)
                            ghichu += "Thời điểm đánh giá có format không hợp lệ (phải có định dạng năm và > 1900";
                        break;
                    default:
                        ghichu += "Chu Kỳ đánh giá có định dạng không hợp lệ phải là Tháng hoặc Năm;";
                        break;
                }
                if (ghichu != "")
                    danhMucList.Add(new
                    {
                        d.MaNhanVien,
                        d.ChuKyDanhGia,
                        d.ThoiDiemDanhGia,
                        ghiChuLoi = ghichu
                    });
            }
            if (danhMucList.Count > 0)
                return BadRequest(danhMucList);
            dbAdapter.connect();
            foreach (var d in nSKhongDanhGias)
            {
                var nhanVien = _uow.Users.FirstOrDefault(x => x.MaNhanVien == d.MaNhanVien && !x.IsDeleted);
                dbAdapter.createStoredProceder("sp_AddNSKhongDanhGia");
                dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = nhanVien.Id;
                dbAdapter.sqlCommand.Parameters.Add("@ChuKyDanhGia", SqlDbType.NVarChar).Value = d.ChuKyDanhGia;
                dbAdapter.sqlCommand.Parameters.Add("@ThoiDiemDanhGia", SqlDbType.NVarChar).Value = d.ThoiDiemDanhGia;
                dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
            }
            dbAdapter.deConnect();
            return Ok(new { Message = "Import successful", nSKhongDanhGias.Count });
        }
    }
}
