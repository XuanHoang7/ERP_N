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
using System.Data;
using System.IO;
using System.Linq;
using ThacoLibs;
using static ERP.Data.MyDbContext;
using System.Collections.Generic;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsApi")]
    [Authorize]

    public class DanhMucLanhDaoDonViController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DanhMucLanhDaoDonViController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitofWork)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitofWork;
        }
        [HttpPost]
        public IActionResult AddDM_LanhDaoDonVi(DM_LanhDaoDonViDTO lanhDaoDonVi)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_AddDM_LanhDaoDonVi");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@LanhDaoId", lanhDaoDonVi.LanhDaoId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViID", lanhDaoDonVi.DonViID);
                dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);

                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                {
                    return Ok("thành công.");
                }
                else
                {
                    return BadRequest("Id không tồn tại, đơn vị đã có lãnh đạo hoặc dữ liệu đã tồn tại");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý là " + ex.Message });
            }
        }

        [HttpPut]
        public IActionResult UpdateDM_LanhDaoDonVi(DM_LanhDaoDonViDTO dto)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_UpdateDM_LanhDaoDonVi");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = dto.Id;
                dbAdapter.sqlCommand.Parameters.Add("@LanhDaoId", SqlDbType.UniqueIdentifier).Value = dto.LanhDaoId;
                dbAdapter.sqlCommand.Parameters.Add("@DonViID", SqlDbType.UniqueIdentifier).Value = dto.DonViID;
                dbAdapter.sqlCommand.Parameters.Add("@UpdateBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                {
                    return Ok("thành công.");
                }
                else
                {
                    return BadRequest("Id không tồn tại, đơn vị đã có lãnh đạo hoặc dữ liệu đã tồn tại");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý là " + ex.Message});
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetDM_LanhDaoDonViById(Guid? id)
        {
            if (id == null && id == Guid.Empty)
                return BadRequest("Id is null or empty");
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_GetDM_LanhDaoDonViById");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
                var result = dbAdapter.runStored2Object();
                dbAdapter.deConnect();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý là " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetDM_LanhDaoDonViByKeyword(string keyword = "", int page = 1, int pageSize = 10)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_GetDM_LanhDaoDonViByKeyword");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
                var result = dbAdapter.runStored2ObjectList();
                dbAdapter.deConnect();
                // Tạo kết quả phân trang
                var totalRow = result.Count;
                var totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
                if (page < 1)
                {
                    page = 1;
                }
                else if (page > totalPage)
                {
                    page = totalPage;
                }
                var dataList = result.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                // Trả về kết quả
                return Ok(new
                {
                    totalRow,
                    totalPage,
                    pageSize,
                    data = dataList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDM_LanhDaoDonVi(Guid? id)
        {
            if (id == null && id == Guid.Empty)
                return BadRequest("Id is null or empty");
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_DeleteDM_LanhDaoDonVi");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                dbAdapter.sqlCommand.Parameters.Add("@DeleteBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                {
                    return Ok("Xóa thành công.");
                }
                else
                {
                    return BadRequest("Id không tồn tại");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý là " + ex.Message });
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
                worksheet.Cells[1, 1].Value = "Mã Đơn vị KPI";
                worksheet.Cells[1, 2].Value = "Mã lãnh đạo";

                // Add some formatting for header
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

                // Set column width
                worksheet.Column(1).Width = 40; // "Mã mục tiêu trọng yếu"
                worksheet.Column(2).Width = 40; // "Tên mục tiêu trọng yếu"

                // Convert Excel file to a byte array
                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Return the Excel file as a downloadable response
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhMucLanhDaoDonVi.xlsx");
            }
        }

        [HttpPost("import-excel")]
        public IActionResult ImportDanhMucLanhDaoDonVi([FromBody] List<ClassDanhMucLanhDaoDonViImport> danhMucLanhDaoDonVis)
        {
            if (danhMucLanhDaoDonVis == null || danhMucLanhDaoDonVis.Count == 0)
                return BadRequest("Không có dữ liệu");

            var danhMucListError = new List<dynamic>();
            var existingDonViIds = _uow.DanhMucLanhDaoDonVis
                .GetAll(dm => !dm.IsDeleted)
                .GroupBy(dm => new { dm.DonViID, dm.LanhDaoId })
                .ToDictionary(group => group.Key, group => group.First());
            var importDonViIds = new HashSet<Guid?>();
            foreach (var d in danhMucLanhDaoDonVis)
            {
                var ghichu = "";

                // Kiểm tra MaDonVi và MaLanhDao từ cơ sở dữ liệu
                var donVi = _uow.vptq_kpi_DonViKPIs.FirstOrDefault(x => x.MaDonViKPI == d.MaDonVi);
                var lanhDao = _uow.Users.FirstOrDefault(x => x.MaNhanVien == d.MaLanhDao && !x.IsDeleted);

                if (donVi == null)
                    ghichu += "Mã đơn vị không tồn tại;";
                else if (importDonViIds.Contains(donVi.Id))
                    ghichu += "Đơn vị này đã bị trùng lặp trong danh sách import;";
                else
                    importDonViIds.Add(donVi.Id);

                if (lanhDao == null)
                    ghichu += "Mã lãnh đạo không tồn tại;";

                // Kiểm tra trùng lặp trong cơ sở dữ liệu
                if (donVi != null && lanhDao != null &&
                    existingDonViIds.ContainsKey(new { DonViID = donVi.Id, LanhDaoId = lanhDao.Id }))
                {
                    ghichu += "Dữ liệu này đã tồn tại trong database;";
                }

                if (!string.IsNullOrEmpty(ghichu))
                {
                    danhMucListError.Add(new
                    {
                        maDonVi = d.MaDonVi,
                        maLanhDao = d.MaLanhDao,
                        ghiChuLoi = ghichu
                    });
                }
            }
            if (danhMucListError.Count > 0) return BadRequest(danhMucListError);
            var newDonVis = danhMucLanhDaoDonVis
                .Where(d => _uow.vptq_kpi_DonViKPIs.Any(x => x.MaDonViKPI == d.MaDonVi) &&
                            _uow.Users.Any(x => x.MaNhanVien == d.MaLanhDao))
                .Select(d => new DM_LanhDaoDonVi
                {
                    CreatedBy = Guid.Parse(User.Identity.Name),
                    CreatedDate = DateTime.Now,
                    LanhDaoId = _uow.Users.FirstOrDefault(x => x.MaNhanVien == d.MaLanhDao).Id,
                    DonViID = _uow.vptq_kpi_DonViKPIs.FirstOrDefault(x => x.MaDonViKPI == d.MaDonVi).Id
                }).ToList();

            _uow.DanhMucLanhDaoDonVis.AddRange(newDonVis);
            _uow.Complete();

            return Ok(new { Message = "Import successful", danhMucLanhDaoDonVis.Count });
        }
    }
}
