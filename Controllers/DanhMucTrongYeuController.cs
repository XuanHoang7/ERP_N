using DocumentFormat.OpenXml.Wordprocessing;
using ERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using ThacoLibs;
using static ERP.Data.MyDbContext;
using System.Collections.Generic;
using ERP.Infrastructure;
using Quartz.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using ERP.Models.DanhMuc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsApi")]
    [Authorize]

    public class DanhMucTrongYeuController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter _dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DanhMucTrongYeuController(IConfiguration _configuration, 
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitof)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            _dbAdapter = new DbAdapter(connectionString);
            _uow = unitof;
        }

        [HttpGet]
        public ActionResult GetAll(string keyword = "", int page = 1, int pageSize = 10)
        {
            _dbAdapter.connect();
            _dbAdapter.createStoredProceder("sp_GetAllDanhMucTrongYeuByKeyword");
            _dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar, 500).Value = keyword;
            var data = _dbAdapter.runStored2ObjectList();

            int totalRow = data.Count();
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);

            // Kiểm tra và điều chỉnh giá trị của page
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPage)
            {
                page = totalPage;
            }
            var datalist = data.Skip((page - 1) * pageSize).Take(pageSize);
            _dbAdapter.deConnect();
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                datalist
            });
        }
        [HttpGet("{id}")]
        public ActionResult GetById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("Id null or empty");
            }
            _dbAdapter.connect();
            _dbAdapter.createStoredProceder("sp_GetDanhMucTrongYeuById");
            _dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            var data = _dbAdapter.runStored2Object();
            _dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("Id is null or empty");
            }
            _dbAdapter.connect();
            _dbAdapter.createStoredProceder("sp_DeleteDanhMucTrongYeuById");
            _dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            _dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = _dbAdapter.runStoredNoneQuery();
            _dbAdapter.deConnect();

            if (result > 0)
            {
                return Ok("thành công.");
            }
            else
            {
                return BadRequest("Id không tồn tại");
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
                worksheet.Cells[1, 1].Value = "Mã mục tiêu trọng yếu";
                worksheet.Cells[1, 2].Value = "Tên mục tiêu trọng yếu";
                worksheet.Cells[1, 3].Value = "Diễn giải";
                worksheet.Cells[1, 4].Value = "Trạng thái sử dụng";

                // Add some formatting for header
                using (var range = worksheet.Cells[1, 1, 1, 4])
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

                // Set column width
                worksheet.Column(1).Width = 20; // "Mã mục tiêu trọng yếu"
                worksheet.Column(2).Width = 50; // "Tên mục tiêu trọng yếu"
                worksheet.Column(3).Width = 50; // "Diễn giải"
                worksheet.Column(4).Width = 20; // "Trạng thái sử dụng"

                // Convert Excel file to a byte array
                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Return the Excel file as a downloadable response
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhMucTrongYeuTemplate.xlsx");
            }
        }

        [HttpPost("import-excel")]
        public IActionResult ImportDanhMucTrongYeu([FromBody] List<ClassDanhMucTrongYeuImport> danhMucTrongYeus)
        {
            if (danhMucTrongYeus == null || danhMucTrongYeus.Count == 0)
                return BadRequest("Không có dữ liệu");
            var danhMucListError = new List<dynamic>();
            var importMaDonViTrongYeus = new HashSet<string>();
            var existingMaDanhMucTrongYeu = _uow.DanhMucTrongYeus
                .GetAll(dm => !dm.IsDeleted)
                .Select(dm => dm.MaDanhMucTrongYeu)
                .ToHashSet();
            foreach (var d in danhMucTrongYeus)
            {
                var ghichu = "";
                if (string.IsNullOrWhiteSpace(d.MaDanhMucTrongYeu))
                    ghichu += "Mã danh mục trọng yếu không được rỗng;";
                else
                {
                    if (importMaDonViTrongYeus.Contains(d.MaDanhMucTrongYeu))
                        ghichu += "Mã danh mục trọng yếu đã bị trùng lặp trong danh sách import;";
                    else
                        importMaDonViTrongYeus.Add(d.MaDanhMucTrongYeu);
                    if (existingMaDanhMucTrongYeu.Contains(d.MaDanhMucTrongYeu))
                        ghichu += "Mã danh mục trọng yếu đã tồn tại trong database;";
                }

                if (string.IsNullOrWhiteSpace(d.TenDanhMucTrongYeu))
                    ghichu += "Tên danh mục trọng yếu không được rỗng;";
                if (!string.IsNullOrEmpty(ghichu))
                    danhMucListError.Add(new
                    {
                        maDanhMucTrongYeu = d.MaDanhMucTrongYeu,
                        tenDanhMucTrongYeu = d.TenDanhMucTrongYeu,
                        dienGiai = d.DienGiai,
                        trangThai = d.TrangThai,
                        ghiChu = d.GhiChu,
                        ghiChuLoi = ghichu
                    });
            }
            if (danhMucListError.Count > 0)
                return BadRequest(danhMucListError);
            var newDanhMucs = danhMucTrongYeus.Select(d => new DanhMucTrongYeu
            {
                CreatedBy = Guid.Parse(User.Identity.Name),
                CreatedDate = DateTime.Now,
                MaDanhMucTrongYeu = d.MaDanhMucTrongYeu,
                TenDanhMucTrongYeu = d.TenDanhMucTrongYeu,
                DienGiai = d.DienGiai,
                GhiChu = d.GhiChu,
                TrangThai = d.TrangThai
            }).ToList();

            _uow.DanhMucTrongYeus.AddRange(newDanhMucs);
            _uow.Complete();

            return Ok(new { Message = "Import successful", Count = danhMucTrongYeus.Count });
        }





        [HttpPut]
        public ActionResult Update([FromBody] ClassDanhMucTrongYeu updatedData)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _dbAdapter.connect();
                _dbAdapter.createStoredProceder("sp_UpdateDanhMucTrongYeuById");

                _dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = updatedData.Id;
                _dbAdapter.sqlCommand.Parameters.Add("@MaDanhMucTrongYeu", SqlDbType.NVarChar, 50).Value = updatedData.MaDanhMucTrongYeu;
                _dbAdapter.sqlCommand.Parameters.Add("@TenDanhMucTrongYeu", SqlDbType.NVarChar, 250).Value = updatedData.TenDanhMucTrongYeu;
                _dbAdapter.sqlCommand.Parameters.Add("@DienGiai", SqlDbType.NVarChar, 500).Value = updatedData.DienGiai;
                _dbAdapter.sqlCommand.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = updatedData.TrangThai;
                _dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar, 500).Value = updatedData.GhiChu;
                _dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);

                var result = _dbAdapter.runStoredNoneQuery();
                _dbAdapter.deConnect();

                if (result > 0)
                {
                    return Ok("Cập nhật thành công.");
                }
                else
                {
                    return BadRequest("Id không tồn tại hoặc mã đang tồn tại.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000)  // Số lỗi 50000 là số mặc định khi dùng RAISERROR
                {
                    return BadRequest(new { ex.Message });
                }
                else
                {
                    return StatusCode(500, new { Message = "Có lỗi xảy ra khi update danh mục." });
                }
            }

        }

        [HttpPost]
        public ActionResult AddDanhMucTrongYeu([FromBody] ClassDanhMucTrongYeu model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _dbAdapter.connect();
                _dbAdapter.createStoredProceder("sp_AddDanhMucTrongYeu");
                _dbAdapter.sqlCommand.Parameters.Add("@MaDanhMucTrongYeu", SqlDbType.NVarChar).Value = model.MaDanhMucTrongYeu;
                _dbAdapter.sqlCommand.Parameters.Add("@TenDanhMucTrongYeu", SqlDbType.NVarChar).Value = model.TenDanhMucTrongYeu;
                _dbAdapter.sqlCommand.Parameters.Add("@DienGiai", SqlDbType.NVarChar).Value = model.DienGiai ?? string.Empty;
                _dbAdapter.sqlCommand.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = model.TrangThai;
                _dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = model.GhiChu ?? string.Empty;
                _dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = _dbAdapter.runStoredNoneQuery();
                _dbAdapter.deConnect();
                if (result > 0)
                {
                    return Ok("Thêm thành công.");
                }
                else
                {
                    return BadRequest("Thêm thất bại do mã đã tồn tại.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000)  // Số lỗi 50000 là số mặc định khi dùng RAISERROR
                {
                    return BadRequest(new { ex.Message });
                }
                else
                {
                    return StatusCode(500, new { Message = "Có lỗi xảy ra khi update danh mục." });
                }
            }

        }


    }
}
