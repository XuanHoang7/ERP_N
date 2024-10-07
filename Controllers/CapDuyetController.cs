using ERP.Infrastructure;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using ERP.Models;
using System.Data;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
using ERP.Models.DanhMuc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using ERP.Models.ChiTieuKPI;
using Google.Apis.Logging;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CapDuyetController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;
        public CapDuyetController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitofWork)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitofWork;
        }

        [HttpPost]
        public IActionResult Add([FromBody] DanhMucDuyetAddAndUpdateDTO model)
        {
            string validate = ValidateModel(model);
            if (validate != null)
                return BadRequest(validate);
            if (AddDanhMuc(model) > 0)
                return Ok("Add Success");
            return BadRequest("Add Failed");
        }

        private string ValidateModel(DanhMucDuyetAddAndUpdateDTO model, Guid? id = null)
        {
            if (model.IsCaNhan)
            {
                model.DanhMucDonViId = null;
                if(id != null)
                {
                    if (_uow.DanhMucDuyets.Exists(p => p.NhanVienId == model.NhanVienId && p.Id != id))
                        return "Nhân viên này đã có danh sách cấp duyệt";
                }
                if (_uow.DanhMucDuyets.Exists(p => p.NhanVienId == model.NhanVienId))
                    return "Nhân viên này đã có danh sách cấp duyệt";
                if (!_uow.Users.Exists(p => p.Id == model.NhanVienId))
                    return "Id nhân viên này không tồn tại";
            }
            else
            {
                model.NhanVienId = null;
                if(id != null)
                {
                    if (_uow.DanhMucDuyets.Exists(p => p.DM_DonViDanhGiaId == model.DanhMucDonViId && p.Id != id))
                        return "Đơn vị này đã có danh sách cấp duyệt";
                }
                if (_uow.DanhMucDuyets.Exists(p => p.DM_DonViDanhGiaId == model.DanhMucDonViId))
                    return "Đơn vị này đã có danh sách cấp duyệt";
                if (!_uow.DonViDanhGias.Exists(p => p.Id == model.DanhMucDonViId))
                    return "Id đơn vị này không tồn tại";
            }
            if (model.CapDuyetDTOs == null || model.CapDuyetDTOs.Count == 0)
                return "Dữ liệu cấp duyệt là bắt buộc";
            var duplicateCapDuyet = model.CapDuyetDTOs
                .GroupBy(d => d.CapDuyet)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateCapDuyet.Any())
                return "Có cấp duyệt bị trùng";
            foreach (var item in model.CapDuyetDTOs)
            {
                if (!_uow.Users.Exists(p => p.Id == item.LanhDaoDuyetId))
                    return "Id lãnh đạo cấp duyệt này không tồn tại";
            }
            return null;
        }

        private int AddDanhMuc(DanhMucDuyetAddAndUpdateDTO model)
        {
            dbAdapter.connect();
            int result = 0;
            foreach (var item in model.CapDuyetDTOs)
            {
                dbAdapter.createStoredProceder("sp_AddDanhMucDuyet");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@IsIndividual", model.IsCaNhan);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@NhanVienId", model.NhanVienId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DMDonViDanhGiaId", model.DanhMucDonViId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@CapDuyet", (int)item.CapDuyet);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@LanhDaoDuyetId", item.LanhDaoDuyetId);
                var r = dbAdapter.runStoredNoneQuery();
                if (r > 0)
                    result++;
            }
            dbAdapter.deConnect();
            return result;
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int pageSize = 10, string keyword = "", bool IsCaNhan = true)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllDanhMucDuyets");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Isdividual", IsCaNhan);
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            try
            {
                List<dynamic> jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());
                if (IsCaNhan)
                {
                    var danhMucDuyetList = jsonObject.Select(item => new
                    {
                        id = (Guid)item.id,
                        maNhanVien = item.maNhanVien.ToString(),
                        hoVaTen = item.hoVaTen.ToString(),
                        capDuyets = ((IEnumerable<dynamic>)item.capDuyets)
                    .Select(subItem => new
                    {
                        cacCapDuyet = subItem.cacCapDuyet.ToString(),
                        tenLanhDaoDuyet = subItem.tenLanhDaoDuyet.ToString()
                    }).ToList()
                    }).ToList();
                    var totalRow = danhMucDuyetList.Count;
                    var totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
                    if (page < 1) page = 1;
                    if (page > totalPage) page = totalPage;

                    var dataList = danhMucDuyetList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    return Ok(new
                    {
                        totalRow,
                        totalPage,
                        pageSize,
                        data = dataList
                    });
                }
                else
                {
                    var danhMucDuyetList = jsonObject.Select(item => new
                    {
                        id = (Guid)item.id,
                        maDonViKPI = item.maDonViKPI.ToString(),
                        tenDonVi = item.tenDonVi.ToString(),
                        capDuyets = ((IEnumerable<dynamic>)item.capDuyets)
                    .Select(subItem => new
                    {
                        cacCapDuyet = subItem.cacCapDuyet.ToString(),
                        tenLanhDaoDuyet = subItem.tenLanhDaoDuyet.ToString()
                    }).ToList()
                    }).ToList();
                    var totalRow = danhMucDuyetList.Count;
                    var totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
                    if (page < 1) page = 1;
                    if (page > totalPage) page = totalPage;

                    var dataList = danhMucDuyetList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                    return Ok(new
                    {
                        totalRow,
                        totalPage,
                        pageSize,
                        data = dataList
                    });
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty || !_uow.DanhMucDuyets.Exists(u => u.Id == id))
                return BadRequest("Id cấp duyệt này không tồn tại");
            if (DeleleDanhMuc(id) > 0)
                return Ok("Delete thành công.");
            return BadRequest("Delete thất bại");
        }
        private int DeleleDanhMuc(Guid id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteDanhMucDuyet");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
            var result =  dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return result;
        }
        [HttpPut]
        public IActionResult Update([FromBody] DanhMucDuyetAddAndUpdateDTO model)
        {
            if (model.Id == Guid.Empty || !_uow.DanhMucDuyets.Exists(u => u.Id == model.Id))
                return BadRequest("Id cấp duyệt này không tồn tại");
            string validate = ValidateModel(model, model.Id);
            if (validate != null)
                return BadRequest(validate);
            if (AddDanhMuc(model) > 0)
            {
                DeleleDanhMuc(model.Id ?? Guid.Empty);
                return Ok("Update Success");
            }
            return BadRequest("Update Failed");
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate(bool IsCaNhan = true)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("CapDuyetTemplate");
            if(IsCaNhan)
                worksheet.Cells[1, 1].Value = "Mã nhân viên";
            else
                worksheet.Cells[1, 1].Value = "Mã Đơn Vị KPI";
            worksheet.Cells[1, 2].Value = "Mã lãnh dạo đánh giá(*)";
            worksheet.Cells[1, 3].Value = "Mã lãnh dạo phòng KPI";
            worksheet.Cells[1, 4].Value = "Mã lãnh đạo phê duyệt(*)";
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
            worksheet.Column(1).Width = 50;
            worksheet.Column(2).Width = 50;
            worksheet.Column(3).Width = 50;
            worksheet.Column(4).Width = 50;
            var stream = new MemoryStream();
            package.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CapDuyetTemplate.xlsx");
        }
        [HttpPost("import-excel")]
        public IActionResult Import([FromBody] List<DanhMUcDuyetImportDTO> model, bool IsCaNhan = true)
        {
            if (model == null || model.Count == 0)
                return BadRequest("Không có dữ liệu");
            var capDuyetErrors = new List<dynamic>();
            var importCapDuyets = new HashSet<string>();
            Dictionary<string, Guid> dataDictionary = new();
            dynamic existingCapDuyet = null;
            if (IsCaNhan)
            {
                var userList = _uow.Users.GetAll(u => !u.IsDeleted);// Giả sử bạn có một lớp người dùng User với các thuộc tính Id và MaNhanVien
                foreach (var user in userList)
                {
                    if (!string.IsNullOrEmpty(user.MaNhanVien))
                        dataDictionary[user.MaNhanVien] = user.Id; // Thêm vào dictionary
                }

                existingCapDuyet = _uow.DanhMucDuyets
                    .GetAll(dm => true)
                    .Select(dm => dm.NhanVienId)
                    .ToHashSet();
            }
            else
            {
                var donViList = _uow.vptq_kpi_DonViKPIs.GetAll(k => k.IsDanhGia);// Giả sử bạn có một lớp người dùng User với các thuộc tính Id và MaNhanVien
                foreach (var item in donViList)
                {
                    var donViDanhGia = _uow.DonViDanhGias.GetSingle(d => d.IdDonViKPI == item.Id);
                    if (donViDanhGia != null && !string.IsNullOrEmpty(item.MaDonViKPI))
                        dataDictionary[item.MaDonViKPI] = donViDanhGia.Id; // Thêm vào dictionary
                }

                existingCapDuyet = _uow.DanhMucDuyets
                    .GetAll(dm => true)
                    .Select(dm => dm.DM_DonViDanhGiaId)
                    .ToHashSet();
            }
            foreach (var d in model)
            {
                var ghichu = "";
                if (IsCaNhan)
                {
                    if (string.IsNullOrWhiteSpace(d.MaNhanVien))
                        ghichu += "Mã nhân viên không được rỗng;";
                    else
                    {
                        if (!dataDictionary.TryGetValue(d.MaNhanVien, out Guid NhanVienId))
                            ghichu += "Mã nhân viên không tồn tại trong database;";
                        if (NhanVienId != Guid.Empty || existingCapDuyet.Contains(NhanVienId))
                            ghichu += "Danh mục duyệt của nhân viên này đã tồn tại trong database;";
                        else
                        {
                            if (d.CapDuyetImportDTOs == null)
                                ghichu += "Các cấp duyệt không được rỗng;";
                            var duplicateCapDuyet = d.CapDuyetImportDTOs
                            .GroupBy(d => d.CapDuyet)
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();
                            if (duplicateCapDuyet.Any())
                                ghichu += "Có cấp duyệt bị trùng;";
                            foreach (var item in d.CapDuyetImportDTOs)
                            {
                                if (item.CapDuyet != CacCapDuyet.LDDVKPI && !dataDictionary.TryGetValue(item.MaLanhDaoDuyet, out Guid LanhDaoDuyetId))
                                    ghichu += "Mã nhân viên của cấp duyệt này không tồn tại trong database;";
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(d.MaDonViKPI))
                        ghichu += "Mã đơn vị KPI không được rỗng;";
                    else
                    {
                        if (!dataDictionary.TryGetValue(d.MaDonViKPI, out Guid DMDonViDanhGiaId))
                            ghichu += "Mã đơn vị KPI không tồn tại trong database hoặc nó không thuộc đơn vị đánh gái;";
                        if (DMDonViDanhGiaId != Guid.Empty || existingCapDuyet.Contains(DMDonViDanhGiaId))
                            ghichu += "Danh mục duyệt của đơn vị KPI này đã tồn tại trong database;";
                        else
                        {
                            if (d.CapDuyetImportDTOs == null)
                                ghichu += "Các cấp duyệt không được rỗng;";
                            var duplicateCapDuyet = d.CapDuyetImportDTOs
                            .GroupBy(d => d.CapDuyet)
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();
                            if (duplicateCapDuyet.Any())
                                ghichu += "Có cấp duyệt bị trùng;";
                            foreach (var item in d.CapDuyetImportDTOs)
                            {
                                if (item.CapDuyet != CacCapDuyet.LDDVKPI && !dataDictionary.TryGetValue(item.MaLanhDaoDuyet, out Guid LanhDaoDuyetId))
                                    ghichu += "Mã nhân viên của cấp duyệt này không tồn tại trong database;";
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ghichu))
                    capDuyetErrors.Add(new
                    {
                        maNhanVien = d.MaNhanVien,
                        maDonViKPI = d.MaDonViKPI,
                        capDuyet = d.CapDuyetImportDTOs,
                        ghichu
                    });
            }
            if (capDuyetErrors.Count > 0)
                return BadRequest(capDuyetErrors);
            int result = 0;
            dbAdapter.connect();
            foreach (var item in model)
            {
                foreach (var itemChild in item.CapDuyetImportDTOs)
                {
                    dbAdapter.createStoredProceder("sp_AddDanhMucDuyet");
                    dbAdapter.sqlCommand.Parameters.Add("@IsIndividual", SqlDbType.Bit).Value = IsCaNhan;
                    dbAdapter.sqlCommand.Parameters.Add("@NhanVienId", SqlDbType.UniqueIdentifier).Value = dataDictionary.GetValueOrDefault(item.MaNhanVien);
                    dbAdapter.sqlCommand.Parameters.Add("@DMDonViDanhGiaId", SqlDbType.UniqueIdentifier).Value = dataDictionary.GetValueOrDefault(item.MaDonViKPI);
                    dbAdapter.sqlCommand.Parameters.Add("@CapDuyet", SqlDbType.Int).Value = itemChild.CapDuyet;
                    dbAdapter.sqlCommand.Parameters.Add("@LanhDaoDuyetId", SqlDbType.UniqueIdentifier).Value = dataDictionary.GetValueOrDefault(itemChild.MaLanhDaoDuyet);
                    var r = dbAdapter.runStoredNoneQuery();
                    if (r > 0)
                        result++;
                }
            }
            dbAdapter.deConnect();
            return Ok(new { Message = "Import successful", result });
        }
    }
}
