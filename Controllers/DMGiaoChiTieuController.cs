using ERP.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using ERP.Models;
using System.Data;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using ERP.Models.DanhMuc;
using System.Text;
using ERP.Models.Default;
using NPOI.HSSF.Record.Chart;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DMGiaoChiTieuController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;
        public DMGiaoChiTieuController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitofWork)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitofWork;
        }
        [HttpGet]
        public IActionResult GetAllByKeyword(Guid? DonViKPIId, Guid? PhongBanId, string keyword = "", int page = 1, int pageSize = 10)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucGiaoChiTieus");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViKPIId", DonViKPIId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@PhongBanId", PhongBanId);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());
            if (jsonObject == null)
            {
                return NotFound();
            }
            var danhMucGiaoChiTieuList = jsonObject.Select(item => new
            {
                idDMGiaoChiTieu = (Guid)item.id,
                tenDonViKPI = item.tenDonViKPI.ToString(),
                nhanSuGiaoChiTieu = string.Join("; ",
                    ((IEnumerable<dynamic>)item.nhanSuDuocGiaoChiTieus)
                    .Select(subItem => $"{subItem.maNhanVien.ToString()} - {subItem.hoVaTen.ToString()}"))
            }).ToList();

            var totalRow = danhMucGiaoChiTieuList.Count;
            var totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPage) page = totalPage;

            var dataList = danhMucGiaoChiTieuList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                data = dataList
            });
        }

        [HttpPost]
        public IActionResult Add([FromBody] DMGiaoChiTieuDTO model)
        {
            var duplicateIds = model.DuocGiaoChiTieuDTOs
                .GroupBy(d => d.UserId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
                return BadRequest($"Có ID nhân sự được giao chỉ tiêu bị trùng: {string.Join(", ", duplicateIds)}");
            dbAdapter.connect();
            int rowChange = 0;
            if(!_uow.vptq_kpi_DonViKPIs.Exists(dv => dv.Id == model.DonViKPIId))
                return BadRequest("Id đơn vị KPI này không tồn tại");
            if (_uow.DMGiaoChiTieus.Exists(dm => dm.DonViKPIId == model.DonViKPIId))
                return BadRequest("Đơn vị này đã có danh mục giao chỉ tiêu");
            foreach(var item in model.DuocGiaoChiTieuDTOs)
            {
                if(!_uow.Users.Exists(u => u.Id == item.UserId && !u.IsDeleted))
                    return BadRequest("User Id này không tồn tại trong database");
            }
            foreach (var item in model.DuocGiaoChiTieuDTOs)
            {
                dbAdapter.createStoredProceder("sp_AddDanhMucGiaoChiTieu");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViKPIId", model.DonViKPIId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", item.UserId);
                var result = dbAdapter.runStoredNoneQuery();
                if (result > 0)
                    rowChange++;
            }
            dbAdapter.deConnect();
            return Ok("Success" + rowChange);
        }

        [HttpPut]
        public IActionResult Update([FromBody] DMGiaoChiTieuDTO model)
        {
            if (!_uow.DMGiaoChiTieus.Exists(dv => dv.Id == model.Id))
                return BadRequest("Id danh mục giao chỉ tiêu không tồn tại");
            var duplicateIds = model.DuocGiaoChiTieuDTOs
                .GroupBy(d => d.UserId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateIds.Any())
                return BadRequest($"Có ID nhân sự được giao chỉ tiêu bị trùng: {string.Join(", ", duplicateIds)}");
            dbAdapter.connect();
            int rowChange = 0;
            if (!_uow.vptq_kpi_DonViKPIs.Exists(dv => dv.Id == model.DonViKPIId))
                return BadRequest("Id đơn vị KPI này không tồn tại");
            if (_uow.DMGiaoChiTieus.Exists(dm => dm.DonViKPIId == model.DonViKPIId && dm.Id != model.Id))
                return BadRequest("Đơn vị này đã có danh mục giao chỉ tiêu");
            foreach (var item in model.DuocGiaoChiTieuDTOs)
            {
                if (!_uow.Users.Exists(u => u.Id == item.UserId && !u.IsDeleted))
                    return BadRequest("User Id này không tồn tại trong database");
            }
            foreach (var item in model.DuocGiaoChiTieuDTOs)
            {
                dbAdapter.createStoredProceder("sp_AddDanhMucGiaoChiTieu");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViKPIId", model.DonViKPIId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", item.UserId);
                var result = dbAdapter.runStoredNoneQuery();
                if (result > 0)
                    rowChange++;
            }
            if(rowChange == model.DuocGiaoChiTieuDTOs.Count)
            {
                DeleleDanhMuc(model.Id ?? Guid.Empty);
            }
            dbAdapter.deConnect();
            return Ok("Success" + rowChange);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty || !_uow.DMGiaoChiTieus.Exists(u => u.Id == id))
                return BadRequest("Id danh mục giao chỉ tiêu này không tồn tại");
            if (DeleleDanhMuc(id) > 0)
                return Ok("Delete thành công.");
            return BadRequest("Delete thất bại");
        }
        private int DeleleDanhMuc(Guid id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteDanhMucGiaoChiTieu");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return result;
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("DMGiaoChiTieuTemplate");
            worksheet.Cells[1, 1].Value = "Mã đơn vị KPI";
            worksheet.Cells[1, 2].Value = "Mã nhân sự được trao quyền";
            using (var range = worksheet.Cells[1, 1, 1, 2])
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
            var stream = new MemoryStream();
            package.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DMGiaoChiTieu.xlsx");
        }

        [HttpPost("import-excel")]
        public IActionResult Import([FromBody] List<ImportDMGiaoChiTieuDTO> dMGiaoChiTieuDTOs)
        {
            if (dMGiaoChiTieuDTOs == null || dMGiaoChiTieuDTOs.Count == 0)
                return BadRequest("Không có dữ liệu");
            var danhMucListError = new List<dynamic>();
            var importGiaoChiTieuPairs = new HashSet<(Guid DonViKPIId, Guid NhanVienId)>();
            var danhMucUyQuyenMap = new Dictionary<Guid, DanhMucUyQuyen>();
            var existingDuocUyQuyenPairs = new HashSet<(Guid LanhDaoUyQuyenId, Guid LanhDaoDuocUyQuyenId)>();
            foreach (var item in dMGiaoChiTieuDTOs)
            {
                vptq_kpi_DonViKPI donViKPI = null;
                ApplicationUser nhanVien = null;
                StringBuilder ghichu = new StringBuilder("");
                if (string.IsNullOrEmpty(item.MaDonViKPI))
                    ghichu.Append("Mã đơn vị KPI không được null;");
                else
                {
                    donViKPI = _uow.vptq_kpi_DonViKPIs.GetSingle(dv => dv.MaDonViKPI == item.MaDonViKPI);
                    if (donViKPI == null)
                        ghichu.Append("Mã đơn vị KPI không tồn tại trong database;");
                }
                if (string.IsNullOrEmpty(item.MaNhanVien))
                    ghichu.Append("Mã nhân viên không được null;");
                else
                {
                    nhanVien = _uow.Users.GetSingle(dv => dv.MaNhanVien == item.MaNhanVien);
                    if (nhanVien == null)
                        ghichu.Append("Mã nhân viên không tồn tại trong database;");
                }
                if(donViKPI != null && nhanVien != null)
                {
                    if (importGiaoChiTieuPairs.Contains((donViKPI.Id, nhanVien.Id)))
                        ghichu.Append("Dữ liệu đã bị trùng lặp trong danh sách import;");
                    else
                    {
                        importGiaoChiTieuPairs.Add((donViKPI.Id, nhanVien.Id));
                        var DMGiaoChiTieu = _uow.DMGiaoChiTieus.GetSingle(dv => dv.DonViKPIId == donViKPI.Id);
                        if(DMGiaoChiTieu != null)
                        {
                            if (_uow.DuocGiaoChiTieus.Exists(d => d.DMGiaoChiTieuId == DMGiaoChiTieu.Id && d.UserId == nhanVien.Id))
                            {
                                ghichu.Append("Dữ liệu này đã tồn tại trong database;");
                            }
                        }
                    }
                }
                if (ghichu.Length > 0)
                {
                    danhMucListError.Add(new
                    {
                        maDonViKPI = item.MaDonViKPI,
                        maNhanVienId = item.MaNhanVien,
                        ghiChu = ghichu
                    });
                }
            }
            if (danhMucListError.Count == 0)
            {
                int rowChange = 0;
                foreach (var (DonViKPIId, NhanVienId) in importGiaoChiTieuPairs)
                {
                    dbAdapter.createStoredProceder("sp_AddDanhMucGiaoChiTieu");
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViKPIId", DonViKPIId);
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", NhanVienId);
                    var result = dbAdapter.runStoredNoneQuery();
                    if (result > 0)
                        rowChange++;
                    dbAdapter.deConnect();
                }
                return Ok("Success" + rowChange);
            }
            return BadRequest(danhMucListError);
        }
    }
}
