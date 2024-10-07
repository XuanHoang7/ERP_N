using ERP.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using ERP.Models;
using System.Data;
using System;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using ERP.Models.DanhMuc;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhanQuyenDonViKPIController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;
        public PhanQuyenDonViKPIController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitofWork)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitofWork;
        }

        [HttpPost]
        public IActionResult Add([FromBody] AddAndUpdatePhanQuyenDVKPIDTO model)
        {
            var duplicateIds = model.DonViKPIDTOs
                .GroupBy(d => d.DonViKPIId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if(_uow.PhanQuyenDonViKPIs.Exists(p => p.UserId == model.UserId))
                return BadRequest($"User này đã được phân quyền đơn vị");
            if (duplicateIds.Any())
                return BadRequest($"Có ID Đơn vị KPI bị trùng: {string.Join(", ", duplicateIds)}");
            dbAdapter.connect();
            int rowChange = 0;
            foreach (var item in model.DonViKPIDTOs)
            {
                dbAdapter.createStoredProceder("sp_AddPhanQuyenDonViKPI");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@NhanVienId", model.UserId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViKPIId", item.DonViKPIId);
                dbAdapter.sqlCommand.Parameters.Add("@UserNow", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                if (result > 0)
                    rowChange++;
            }
            dbAdapter.deConnect();
            return Ok("Success" + rowChange);
        }
        [HttpGet]
        public IActionResult Get(int page = 1, int pageSize = 10, string keyword = null)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetPhanQuyenKPIByKeyWord");
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            //var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            // Deserialize kết quả JSON
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(result.ToString());
            if (jsonObject == null || jsonObject.dataList == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            var donViKPIsList = new List<object>();
            foreach (var item in jsonObject.dataList)
            {
                // Deserialize DonViKPIs từ chuỗi JSON
                var donViKPIs = JsonConvert.DeserializeObject<List<dynamic>>(item.donViKPIs.ToString());
                donViKPIsList.Add(new
                {
                    maNhanVien = item.maNhanVien,
                    tenNhanVien = item.tenNhanVien,
                    donViKPIs = donViKPIs // Gán danh sách DonViKPIs đã deserialized
                });
            }

            // Trả về kết quả
            return Ok(new
            {
                totalRow = jsonObject.totalRow,
                totalPage = jsonObject.totalPage,
                pageSize = jsonObject.pageSize,
                dataList = donViKPIsList
            });
        }
        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            if(id == Guid.Empty || !_uow.Users.Exists(u => u.Id == id))
                return BadRequest("Id user này không tồn tại");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeletePhanQuyenDonViKPI");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = id;
            var result = dbAdapter.runStoredNoneQuery();
            if (result > 0)
                return Ok("Delete thành công.");
            return BadRequest("Delete thất bại");
        }

        [HttpPut]
        public IActionResult Update([FromBody] AddAndUpdatePhanQuyenDVKPIDTO model)
        {
            if (!_uow.PhanQuyenDonViKPIs.Exists(p => p.UserId == model.UserId))
                return BadRequest($"User này không tồn tại dữ liệu phân quyền");
            var duplicateIds = model.DonViKPIDTOs
                .GroupBy(d => d.DonViKPIId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
                return BadRequest($"Có ID Đơn vị KPI bị trùng: {string.Join(", ", duplicateIds)}");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeletePhanQuyenDonViKPI");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = model.UserId;
            int rowChange = 0;
            foreach (var item in model.DonViKPIDTOs)
            {
                dbAdapter.createStoredProceder("sp_AddPhanQuyenDonViKPI");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@NhanVienId", model.UserId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViKPIId", item.DonViKPIId);
                dbAdapter.sqlCommand.Parameters.Add("@UserNow", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                if (result > 0)
                    rowChange++;
            }
            dbAdapter.deConnect();
            return Ok("Success" + rowChange);
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("DanhMucUyQuyenTemplate");
            worksheet.Cells[1, 1].Value = "Mã nhân viên";
            worksheet.Cells[1, 2].Value = "Mã đơn vị KPI";
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
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhMucUyQuyenTemplate.xlsx");
        }
        [HttpPost("import-excel")]
        public IActionResult Import([FromBody] List<ImportPhanQuyenDonViKPIDTO> PhanQuyenDonViKPIs)
        {
            if (PhanQuyenDonViKPIs == null || PhanQuyenDonViKPIs.Count == 0)
                return BadRequest("Không có dữ liệu");
            var importPhanQuyenPairs = new HashSet<(string MaNhanVien, string MaDonViKPI)>();
            var userList = _uow.Users.GetAll(u=> !u.IsDeleted);// Giả sử bạn có một lớp người dùng User với các thuộc tính Id và MaNhanVien
            Dictionary<string, Guid> userDictionary = new();
            foreach (var user in userList)
            {
                if (!string.IsNullOrEmpty(user.MaNhanVien))
                    userDictionary[user.MaNhanVien] = user.Id; // Thêm vào dictionary
            }
            var donViKPIList = _uow.vptq_kpi_DonViKPIs.GetAll(u=>true);// Giả sử bạn có một lớp người dùng User với các thuộc tính Id và MaNhanVien
            Dictionary<string, Guid> donViKPIDictionary = new Dictionary<string, Guid>();
            foreach (var donvi in donViKPIList)
            {
                if (!string.IsNullOrEmpty(donvi.MaDonViKPI))
                    donViKPIDictionary[donvi.MaDonViKPI] = donvi.Id; // Thêm vào dictionary
            }
            var dataExists = _uow.PhanQuyenDonViKPIs.GetAll(u => true);// Giả sử bạn có một lớp người dùng User với các thuộc tính Id và MaNhanVien
            HashSet<(Guid IdNhanVien, Guid IdDonVi)> phanQuyenKPIDictionary = new();
            foreach (var phanquyen in dataExists)
            {
                phanQuyenKPIDictionary.Add((phanquyen.UserId, phanquyen.DonViKPIId)); // Thêm vào dictionary
            }
            var phanQuyenListError = new List<dynamic>();
            foreach (var d in PhanQuyenDonViKPIs)
            {
                var ghichu = "";
                if (string.IsNullOrWhiteSpace(d.MaNhanVIen))
                    ghichu += "Mã danh mục trọng yếu không được rỗng;";
                if (string.IsNullOrWhiteSpace(d.MaNhanVIen))
                    ghichu += "Mã danh mục trọng yếu không được rỗng;";
                else
                {
                    if (importPhanQuyenPairs.Contains((d.MaNhanVIen, d.MaDonViKPI)))
                        ghichu += "Mã nhân viên và mã đơn vị KPI này đã trùng lặp đã bị trùng lặp trong danh sách import;";
                    else
                        importPhanQuyenPairs.Add((d.MaNhanVIen, d.MaDonViKPI));
                    if (!userDictionary.TryGetValue(d.MaNhanVIen, out Guid NhanVienId))
                        ghichu += "Mã nhân viên không tồn tại trong database;";
                    if (!donViKPIDictionary.TryGetValue(d.MaDonViKPI, out Guid DonViId))
                        ghichu += "Mã đơn vị KPI không tồn tại trong database;";
                    if (phanQuyenKPIDictionary.Contains((NhanVienId, DonViId)))
                        ghichu += "Dữ liệu đã tồn tại trong database;";
                }

                if (!string.IsNullOrEmpty(ghichu))
                    phanQuyenListError.Add(new
                    {
                        d.MaNhanVIen,
                        d.MaDonViKPI,
                        ghiChuLoi = ghichu
                    });
            }
            if (phanQuyenListError.Count > 0)
                return BadRequest(phanQuyenListError);
            var newDanhMucs = PhanQuyenDonViKPIs.Select(d => new PhanQuyenDonViKPI
            {
                CreatedBy = Guid.Parse(User.Identity.Name),
                CreatedDate = DateTime.Now,
                UserId = userDictionary.GetValueOrDefault(d.MaNhanVIen),
                DonViKPIId = donViKPIDictionary.GetValueOrDefault(d.MaDonViKPI)
            }).ToList();

            _uow.PhanQuyenDonViKPIs.AddRange(newDanhMucs);
            _uow.Complete();

            return Ok(new { Message = "Import successful", PhanQuyenDonViKPIs.Count });
        }
    }
}
