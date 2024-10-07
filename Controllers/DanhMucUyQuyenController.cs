using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using ThacoLibs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using ERP.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DanhMucUyQuyenController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;
        public DanhMucUyQuyenController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitofWork)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitofWork;
        }
        [HttpGet]
        public IActionResult GetAllByKeyword(string keyword = null, int page = 1, int pageSize = 10)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucUyQuyenByKeyWord");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());
            var danhMucUyQuyenList = jsonObject.Select(item => new
            {
                danhMucUyQuyenId = (Guid)item.danhMucUyQuyenId,
                maLanhDaoUyQuyen = item.maLanhDaoUyQuyen.ToString(),
                tenLanhDaoUyQuyen = item.tenLanhDaoUyQuyen.ToString(),
                lanhDaoDuocUyQuyens = ((IEnumerable<dynamic>)item.lanhDaoDuocUyQuyens)
                .Select(subItem => new
                {
                    duocUyQuyenId = (Guid)subItem.duocUyQuyenId,
                    maLanhDaoDuocUyQuyen = subItem.maLanhDaoDuocUyQuyen.ToString(),
                    tenLanhDaoDuocUyQuyen = subItem.tenLanhDaoDuocUyQuyen.ToString()
                }).ToList()
            }).ToList();

            var totalRow = danhMucUyQuyenList.Count;
            var totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPage) page = totalPage;

            var dataList = danhMucUyQuyenList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                data = dataList
            });
        }
        [HttpPost]
        public IActionResult AddDanhMucUyQuyen([FromBody] AddAndUpdateDanhMucUyQuyenDTO model)
        {
            if (model.LanhDaoUyQuyenId == model.DuocUyQuyenDTOs.FirstOrDefault()?.LanhDaoDuocUyQuyenId)
                return BadRequest("Lãnh đạo được ủy quyền trùng với lãnh đạo ủy quyền.");
            var duplicateIds = model.DuocUyQuyenDTOs
                .GroupBy(d => d.LanhDaoDuocUyQuyenId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
                return BadRequest($"Có ID lãnh đạo được ủy quyền bị trùng: {string.Join(", ", duplicateIds)}");
            try
            {
                dbAdapter.connect();
                int rowChange = 0;
                foreach (var duocUyQuyenDTO in model.DuocUyQuyenDTOs)
                {
                    var existingDuocUyQuyen = _uow.DuocUyQuyens.GetAll(du =>
                        du.DanhMucUyQuyenId == model.LanhDaoUyQuyenId &&
                        du.LanhDaoDuocUyQuyenId == duocUyQuyenDTO.LanhDaoDuocUyQuyenId &&
                        !du.IsDeleted).FirstOrDefault();
                    if (existingDuocUyQuyen != null)
                        return BadRequest($"Lãnh đạo được ủy quyền ID {duocUyQuyenDTO.LanhDaoDuocUyQuyenId} đã tồn tại.");
                }
                foreach (var duocUyQuyenDTO in model.DuocUyQuyenDTOs)
                {
                    dbAdapter.createStoredProceder("sp_AddDanhMucUyQuyen");
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@LanhDaoUyQuyenId", model.LanhDaoUyQuyenId);
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@LanhDaoDuocUyQuyenId", duocUyQuyenDTO.LanhDaoDuocUyQuyenId);
                    dbAdapter.sqlCommand.Parameters.Add("@UserNow", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                    var result = dbAdapter.runStoredNoneQuery();
                    if(result > 0)
                        rowChange++;
                }
                dbAdapter.deConnect();
                return Ok("Success" + rowChange);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
            }
        }
        [HttpPut]
        public IActionResult UpdateDanhMucUyQuyen([FromBody] AddAndUpdateDanhMucUyQuyenDTO model)
        {
            var duplicateIds = model.DuocUyQuyenDTOs
                .GroupBy(d => d.LanhDaoDuocUyQuyenId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateIds.Any())
                return BadRequest($"Có ID lãnh đạo được ủy quyền bị trùng: {string.Join(", ", duplicateIds)}");
            if (model.LanhDaoUyQuyenId == model.DuocUyQuyenDTOs.FirstOrDefault()?.LanhDaoDuocUyQuyenId)
                return BadRequest("Lãnh đạo được ủy quyền trùng với lãnh đạo ủy quyền.");
            var danhMucExists = _uow.DanhMucUyQuyens.GetById(model.Id ?? Guid.Empty);
            if(danhMucExists == null)
                return BadRequest("Id này không tồn tại.");
            else
            {
                if(danhMucExists.LanhDaoUpQuyenId != model.LanhDaoUyQuyenId 
                    && _uow.DanhMucUyQuyens.GetSingle(dm => dm.LanhDaoUpQuyenId == model.LanhDaoUyQuyenId && !dm.IsDeleted) != null)
                    return BadRequest("Lãnh đạo này đã tồn tại danh mục ủy quyền rồi");
                try
                {
                    // Cập nhật đối tượng DanhMucUyQuyen
                    danhMucExists.LanhDaoUpQuyenId = model.LanhDaoUyQuyenId;
                    danhMucExists.UpdatedBy = Guid.Parse(User.Identity.Name);
                    danhMucExists.UpdatedDate = DateTime.Now;
                    _uow.DanhMucUyQuyens.Update(danhMucExists);
                    List<DuocUyQuyen> duocUyQuyens = _uow.DuocUyQuyens.GetAll(d => d.DanhMucUyQuyenId == model.Id).ToList();
                    foreach(var duocUyQuyen in duocUyQuyens)
                    {
                        duocUyQuyen.IsDeleted = true;
                        duocUyQuyen.DeletedBy = Guid.Parse(User.Identity.Name);
                        duocUyQuyen.DeletedDate = DateTime.Now;
                        _uow.DuocUyQuyens.Update(duocUyQuyen);
                    }
                    // Xử lý danh sách DuocUyQuyenDTOs
                    foreach (var duocUyQuyenDTO in model.DuocUyQuyenDTOs)
                    {
                        var existingDuocUyQuyen = _uow.DuocUyQuyens.GetAll(du =>
                            du.DanhMucUyQuyenId == model.LanhDaoUyQuyenId &&
                            du.LanhDaoDuocUyQuyenId == duocUyQuyenDTO.LanhDaoDuocUyQuyenId &&
                            !du.IsDeleted).FirstOrDefault();

                        if (existingDuocUyQuyen == null) 
                        { 
                            // Thêm đối tượng DuocUyQuyen mới
                            var newDuocUyQuyen = new DuocUyQuyen
                            {
                                DanhMucUyQuyenId = model.Id ?? Guid.Empty,
                                LanhDaoDuocUyQuyenId = duocUyQuyenDTO.LanhDaoDuocUyQuyenId,
                                CreatedBy = Guid.Parse(User.Identity.Name),
                                CreatedDate = DateTime.Now
                            };
                            _uow.DuocUyQuyens.Add(newDuocUyQuyen);
                        }
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    var result = _uow.Complete();
                    if (result > 0)
                        return Ok("Cập nhật thành công.");
                    return BadRequest("Cập nhật thất bại.");
                }
                catch (Exception ex)
                {
                    // Bắt các lỗi khác
                    return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
                }
            }
            
        }

        [HttpPut("duoc-uy-quyen")]
        public IActionResult UpdateDuocUyQuyen([FromBody] UpdateDanhMucUyQuyenDTO model)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_UpdateDanhMucUyQuyen");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DuocUyQuyenId", model.DuocUyQuyenId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@LanhDaoDuocUyQuyenId", model.LanhDaoDuocUyQuyenId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@DanhMucUyQuyenId", model.DanhMucUyQuyenId);
                dbAdapter.sqlCommand.Parameters.Add("@UserNow", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                    return Ok("Update thành công.");
                return BadRequest("Update thất bại do bị trùng hoặc id không tồn tại.");
            }
            catch (Exception ex)
            {
                // Bắt các lỗi khác
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetDanhMucUyQuyenById(Guid? id)
        {
            if (id == null && id == Guid.Empty)
                return BadRequest("Id is null or empty");
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_GetDanhMucUyQuyenById");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
                var result = dbAdapter.runStored2JSON();
                dbAdapter.deConnect();
                var jsonObject = JsonConvert.DeserializeObject(result.ToString());
                if (jsonObject == null)
                    return NotFound(new { message = "Không tìm thấy dữ liệu." });
                return Ok(jsonObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult SoftDeleteDanhMucUyQuyen(Guid? id)
        {
            if (id == null && id == Guid.Empty)
                return BadRequest("Id is null or empty");
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_DeleteDanhMucUyQuyen");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
                dbAdapter.sqlCommand.Parameters.Add("@UserNow", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                    return Ok("xóa thành công.");
                return BadRequest("xóa thất bại do Id không tồn tại");
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
            }
        }

        [HttpDelete("DuocUyQuyen/{id}")]
        public IActionResult SoftDeleteLanhDaoDuocUyQuyen(Guid? id)
        {
            if (id == null && id == Guid.Empty)
                return BadRequest("Id is null or empty");
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_DeleteLanhDaoDuocUyQuyen");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
                dbAdapter.sqlCommand.Parameters.Add("@UserNow", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                    return Ok("xóa thành công.");
                return BadRequest("xóa thất bại do Id không tồn tại");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." + ex.Message });
            }
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("DanhMucUyQuyenTemplate");
            worksheet.Cells[1, 1].Value = "Mã lãnh đạo ủy quyền";
            worksheet.Cells[1, 2].Value = "Mã lãnh đạo được ủy quyền";
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
        private Dictionary<string, Guid> GetAllUserMap()
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllUsers");
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return result
                .Where(item => item is IDictionary<string, object> dict && dict.ContainsKey("MaNhanVien") && dict.ContainsKey("Id"))
                .ToDictionary(item => ((IDictionary<string, object>)item)["MaNhanVien"].ToString(),
                              item => Guid.Parse(((IDictionary<string, object>)item)["Id"].ToString()));
        }

        [HttpPost("import-excel")]
        public IActionResult ImportDanhMucTrongYeu([FromBody] List<ImportDanhMucUyQuyenDTO> danhMucUyQuyens)
        {
            if (danhMucUyQuyens == null || danhMucUyQuyens.Count == 0)
                return BadRequest("Không có dữ liệu");
            var danhMucListError = new List<dynamic>();
            var importUyQuyenPairs = new HashSet<(Guid LanhDaoUyQuyenId, Guid LanhDaoDuocUyQuyenId)>();
            var danhMucUyQuyenMap = new Dictionary<Guid, DanhMucUyQuyen>();
            var existingDuocUyQuyenPairs = new HashSet<(Guid LanhDaoUyQuyenId, Guid LanhDaoDuocUyQuyenId)>();
            var userMap = GetAllUserMap();
            var existingDanhMucUyQuyens = _uow.DanhMucUyQuyens.GetAll(dm => !dm.IsDeleted)?.ToList() ?? new List<DanhMucUyQuyen>();
            foreach (var d in existingDanhMucUyQuyens)
            {
                danhMucUyQuyenMap[d.LanhDaoUpQuyenId] = d;
            }
            var existingDuocUyQuyens = _uow.DuocUyQuyens.GetAll(du => !du.IsDeleted)?.ToList() ?? new List<DuocUyQuyen>();
            var duocUyQuyenMap = new Dictionary<Guid, List<DuocUyQuyen>>();
            foreach (var du in existingDuocUyQuyens)
            {
                if (!duocUyQuyenMap.ContainsKey(du.DanhMucUyQuyenId))
                    duocUyQuyenMap[du.DanhMucUyQuyenId] = new List<DuocUyQuyen>();
                duocUyQuyenMap[du.DanhMucUyQuyenId].Add(du);
            }
            foreach (var d in danhMucUyQuyens)
            {
                Guid lanhDaoUyQuyenId = Guid.Empty;
                Guid lanhDaoDuocUyQuyenId = Guid.Empty;
                var ghichu = "";
                if (string.IsNullOrEmpty(d.MaLanhDaoUyQuyen) || string.IsNullOrEmpty(d.MaLanhDaoDuocUyQuyen))
                    ghichu += "Lãnh đạo ủy quyền và lãnh đạo được ủy quyền không được null;";
                else if (d.MaLanhDaoUyQuyen.Equals(d.MaLanhDaoDuocUyQuyen))
                    ghichu += "Lãnh đạo ủy quyền và lãnh đạo được ủy quyền không được giống nhau;";
                else if (!userMap.ContainsKey(d.MaLanhDaoUyQuyen) || !userMap.ContainsKey(d.MaLanhDaoDuocUyQuyen))
                    ghichu += "ID của lãnh đạo ủy quyền hoặc lãnh đạo được ủy quyền không tồn tại;";
                else
                {
                    lanhDaoUyQuyenId = userMap[d.MaLanhDaoUyQuyen];
                    lanhDaoDuocUyQuyenId = userMap[d.MaLanhDaoDuocUyQuyen];

                    if (importUyQuyenPairs.Contains((lanhDaoUyQuyenId, lanhDaoDuocUyQuyenId)))
                        ghichu += "Dữ liệu đã bị trùng lặp trong danh sách import;";
                    else
                    {
                        var danhMucUyQuyenExists = existingDanhMucUyQuyens.Find(dm => dm.LanhDaoUpQuyenId == lanhDaoUyQuyenId);
                        if (danhMucUyQuyenExists != null && (duocUyQuyenMap.TryGetValue(danhMucUyQuyenExists.Id, out var duocUyQuyens) &&
                             (duocUyQuyens != null || duocUyQuyens.Any(u => u.LanhDaoDuocUyQuyenId == lanhDaoDuocUyQuyenId))))
                            ghichu += "Dữ liệu đã tồn tại trong database;";
                    }
                }
                if (ghichu != "")
                {
                    danhMucListError.Add(new
                    {
                        maLanhDaoUyQuyen = d.MaLanhDaoUyQuyen,
                        maLanhDaoDuocUyQuyen = d.MaLanhDaoDuocUyQuyen,
                        ghiChu = ghichu
                    });
                }
                else
                    importUyQuyenPairs.Add((lanhDaoUyQuyenId, lanhDaoDuocUyQuyenId)); // Thêm vào tập hợp import
            }

            if (danhMucListError.Count == 0)
            {
                var validDanhMucUyQuyens = new List<DanhMucUyQuyen>();
                foreach (var d in danhMucUyQuyens)
                {
                    var lanhDaoUyQuyenId = userMap[d.MaLanhDaoUyQuyen];
                    var lanhDaoDuocUyQuyenId = userMap[d.MaLanhDaoDuocUyQuyen];
                    if (!danhMucUyQuyenMap.TryGetValue(lanhDaoUyQuyenId, out var existingDanhMuc))
                    {
                        var newDanhMuc = new DanhMucUyQuyen
                        {
                            CreatedBy = Guid.Parse(User.Identity.Name),
                            CreatedDate = DateTime.Now,
                            LanhDaoUpQuyenId = lanhDaoUyQuyenId,
                            DuocUyQuyens = new List<DuocUyQuyen>() // Khởi tạo danh sách
                        };
                        _uow.DanhMucUyQuyens.Add(newDanhMuc);
                        try
                        {
                            _uow.Complete(); // Gọi Complete để lưu vào cơ sở dữ liệu
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi khi lưu dữ liệu", Error = ex.Message });
                        }

                        // Sau khi lưu, tạo đối tượng DuocUyQuyen với DanhMucUyQuyenId mới
                        newDanhMuc.DuocUyQuyens.Add(new DuocUyQuyen
                        {
                            LanhDaoDuocUyQuyenId = lanhDaoDuocUyQuyenId,
                            DanhMucUyQuyenId = newDanhMuc.Id, // Id đã được gán sau khi lưu
                            CreatedBy = Guid.Parse(User.Identity.Name),
                            CreatedDate = DateTime.Now
                        });

                        validDanhMucUyQuyens.Add(newDanhMuc);
                    }
                    else
                    {
                        if (existingDanhMuc.DuocUyQuyens == null)
                            existingDanhMuc.DuocUyQuyens = new List<DuocUyQuyen>();
                        if (!existingDanhMuc.DuocUyQuyens.Any(u => u.LanhDaoDuocUyQuyenId == lanhDaoDuocUyQuyenId))
                        {
                            existingDanhMuc.DuocUyQuyens.Add(new DuocUyQuyen
                            {
                                LanhDaoDuocUyQuyenId = lanhDaoDuocUyQuyenId,
                                DanhMucUyQuyenId = existingDanhMuc.Id,
                                CreatedBy = Guid.Parse(User.Identity.Name),
                                CreatedDate = DateTime.Now
                            });
                        }
                    }
                }

                foreach (var danhMuc in validDanhMucUyQuyens)
                {
                    var existingDanhMuc = _uow.DanhMucUyQuyens.GetById(danhMuc.Id);
                    if (existingDanhMuc != null)
                    {
                        _uow.DanhMucUyQuyens.Update(danhMuc);
                        foreach (var du in danhMuc.DuocUyQuyens)
                        {
                            var existingDuocUyQuyen = _uow.DuocUyQuyens.GetById(du.Id);
                            if (existingDuocUyQuyen == null)
                            {
                                du.DanhMucUyQuyenId = danhMuc.Id; // Đảm bảo liên kết với DanhMucUyQuyen
                                _uow.DuocUyQuyens.Add(du);
                            }
                        }
                    }
                    else
                    {
                        _uow.DanhMucUyQuyens.Add(danhMuc);
                        foreach (var du in danhMuc.DuocUyQuyens)
                        {
                            var existingDuocUyQuyen = _uow.DuocUyQuyens.GetById(du.Id);
                            if (existingDuocUyQuyen == null)
                            {
                                du.DanhMucUyQuyenId = danhMuc.Id; // Đảm bảo liên kết với DanhMucUyQuyen
                                _uow.DuocUyQuyens.Add(du);
                            }
                        }
                    }
                }
                _uow.Complete();
                return Ok(new { Message = "Import successful", danhMucUyQuyens.Count });
            }
            return BadRequest(danhMucListError);
        }
    }
}
