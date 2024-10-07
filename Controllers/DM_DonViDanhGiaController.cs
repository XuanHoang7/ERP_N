using ERP.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using ERP.Models;
using System.Data;
using System.Linq;
using System;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using ERP.Models.DanhMuc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsApi")]
    [Authorize]
    public class DM_DonViDanhGiaController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter _dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DM_DonViDanhGiaController(IConfiguration _configuration,
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
            _dbAdapter.createStoredProceder("sp_GetAllDM_DonViDanhGiaByKeyword");
            _dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar, 250).Value = keyword;
            var data = _dbAdapter.runStored2ObjectList();
            _dbAdapter.deConnect();
            int totalRow = data.Count;
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1)
                page = 1;
            else if (page > totalPage)
                page = totalPage;
            var dataList = data.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                dataList
            });
        }

        [HttpGet("{id}")]
        public ActionResult GetById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return BadRequest("Id is null or empty");
            _dbAdapter.connect();
            _dbAdapter.createStoredProceder("sp_GetDM_DonViDanhGiaById");
            _dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            var data = _dbAdapter.runStored2Object();
            _dbAdapter.deConnect();

            if (data == null)
                return NotFound("Data not found");
            return Ok(data);
        }

        [HttpPost]
        public ActionResult Add([FromBody] ClassDonViDanhGia model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (model.IdDonViKPI == null && _uow.DonViDanhGias.Any(dm => dm.IdDonViKPI == null && !dm.IsDeleted))
                return BadRequest("Đơn vị chung là danh mục Pi tổng đã tồn tại");
            if (_uow.DonViDanhGias.Any(dm => dm.IdDonViKPI == model.IdDonViKPI && !dm.IsDeleted))
                return BadRequest("Đơn vị KPI này đã tồn tại tiền tố");
            _dbAdapter.connect();
            _dbAdapter.createStoredProceder("sp_AddDM_DonViDanhGia");
            if (model.IdDonViKPI != null)
                _dbAdapter.sqlCommand.Parameters.Add("@IdDonViKPI", SqlDbType.UniqueIdentifier).Value = model.IdDonViKPI;
            _dbAdapter.sqlCommand.Parameters.Add("@TienTo", SqlDbType.NVarChar, 50).Value = model.TienTo;
            _dbAdapter.sqlCommand.Parameters.Add("@DanhGia", SqlDbType.Bit).Value = model.DanhGia;
            _dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = _dbAdapter.runStoredNoneQuery();
            _dbAdapter.deConnect();
            if (result > 0)
                return Ok("Successfully added.");
            return BadRequest("Failed to add.");
        }

        [HttpPut]
        public ActionResult Update([FromBody] ClassDonViDanhGia model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if(model.Id == null || model.Id == Guid.Empty)
                return BadRequest("Id không được null or empty");
            if (!_uow.DonViDanhGias.Any(dm => dm.Id == model.Id))
                return BadRequest("Id này không tồn tại trong database");
            if (model.IdDonViKPI == null && _uow.DonViDanhGias.Any(dm => dm.IdDonViKPI == null 
                && !dm.IsDeleted && dm.Id != model.Id))
                return BadRequest("Đơn vị chung là danh mục Pi tổng đã tồn tại");
            if (_uow.DonViDanhGias.Any(dm => dm.IdDonViKPI == model.IdDonViKPI 
                && !dm.IsDeleted && dm.Id != model.Id))
                return BadRequest("Đơn vị KPI này đã tồn tại tiền tố");
            _dbAdapter.connect();
            _dbAdapter.createStoredProceder("sp_UpdateDM_DonViDanhGia");
            _dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = model.Id;
            _dbAdapter.sqlCommand.Parameters.Add("@IdDonViKPI", SqlDbType.UniqueIdentifier).Value = model.IdDonViKPI;
            _dbAdapter.sqlCommand.Parameters.Add("@TienTo", SqlDbType.NVarChar, 50).Value = model.TienTo ?? string.Empty;
            _dbAdapter.sqlCommand.Parameters.Add("@DanhGia", SqlDbType.Bit).Value = model.DanhGia;
            _dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = _dbAdapter.runStoredNoneQuery();
            _dbAdapter.deConnect();
            if (result > 0)
                return Ok("Update successful.");
            return BadRequest("Failed to update. Id may not exist.");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return BadRequest("Id is null or empty");
            _dbAdapter.connect();
            _dbAdapter.createStoredProceder("sp_DeleteDM_DonViDanhGiaById");
            _dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            _dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = _dbAdapter.runStoredNoneQuery();
            _dbAdapter.deConnect();
            if (result > 0)
                return Ok("Delete successful.");
            return BadRequest("Id not found or already deleted.");
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("DM_DonViDanhGiaTemplate");
            worksheet.Cells[1, 1].Value = "Mã Đơn Vị KPI";
            worksheet.Cells[1, 2].Value = "Tiền Tố";
            worksheet.Cells[1, 3].Value = "Đánh Giá";
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

            worksheet.Column(1).Width = 40; // IdDonViKPI
            worksheet.Column(2).Width = 30; // TienTo
            worksheet.Column(3).Width = 10; // DanhGia (Boolean)

            var stream = new MemoryStream();
            package.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DM_DonViDanhGiaTemplate.xlsx");
        }
        [HttpPost("import-excel")]
        public IActionResult ImportDM_DonViDanhGia([FromBody] List<DM_DonViDanhGiaImport> donViDanhGias)
        {
            if (donViDanhGias == null || donViDanhGias.Count == 0)
                return BadRequest("Không có dữ liệu để import");

            var danhMucListError = new List<dynamic>();
            var importIds = new HashSet<Guid?>();
            var existingIds = _uow.DonViDanhGias
                .GetAll(dm => !dm.IsDeleted)
                .Select(dm => dm.IdDonViKPI)
                .ToHashSet();
            foreach (var d in donViDanhGias)
            {
                var ghichu = "";
                if (d.IdDonViKPI == null && existingIds.Contains(null))
                    ghichu += "Đã tồn tại danh mục PI tổng;";
                else
                {
                    if (importIds.Contains(d.IdDonViKPI))
                        ghichu += "IdDonViKPI đã bị trùng lặp trong danh sách import;";
                    else
                        importIds.Add(d.IdDonViKPI);
                    if (existingIds.Contains(d.IdDonViKPI))
                        ghichu += "IdDonViKPI đã tồn tại trong database;";
                    if (string.IsNullOrWhiteSpace(d.TienTo))
                        ghichu += "Tiền tố không được rỗng;";
                }
                if (!string.IsNullOrEmpty(ghichu))
                {
                    danhMucListError.Add(new
                    {
                        idDonViKPI = d.IdDonViKPI,
                        tienTo = d.TienTo,
                        danhGia = d.DanhGia,
                        ghiChuLoi = ghichu
                    });
                }
            }

            if (danhMucListError.Count > 0)
                return BadRequest(danhMucListError);

            // Thêm các dòng mới vào database nếu không có lỗi
            var newDonVis = donViDanhGias.Select(d => new DM_DonViDanhGia
            {
                CreatedBy = Guid.Parse(User.Identity.Name),
                CreatedDate = DateTime.Now,
                IdDonViKPI = d.IdDonViKPI,
                TienTo = d.TienTo,
                DanhGia = d.DanhGia ?? true
            }).ToList();

            _uow.DonViDanhGias.AddRange(newDonVis);
            _uow.Complete();

            return Ok(new { Message = "Import thành công", donViDanhGias.Count });
        }
        [HttpPost("get-tien-to")]
        public IActionResult getTienTo(Guid? Id)
        {
            DM_DonViDanhGia donViDanhGia = null;
            if (Id == null)
                donViDanhGia = _uow.DonViDanhGias.GetSingle(d => d.IdDonViKPI == null);
            else
                donViDanhGia = _uow.DonViDanhGias.GetSingle(d => d.Id == Id);
            return Ok(new { donViDanhGia.TienTo});
        }

    }
}
