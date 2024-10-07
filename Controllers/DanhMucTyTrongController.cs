using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using System.Data;
using ERP.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ERP.Infrastructure;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Drawing;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DanhMucTyTrongController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DanhMucTyTrongController(IConfiguration _configuration, 
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitofWork)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitofWork;
        }


        [HttpPost("addDanhMucTyTrong")]
        public IActionResult AddDanhMucTyTrong([FromBody] ClassDanhMucTyTrongDTO model)
        {
            if (model == null)
                return BadRequest("Invalid data.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if(!(model.NhomChucDanh.Equals("Công Ty") 
                || model.NhomChucDanh.Equals("Ban Phòng Nghiệp Vụ")
                || model.NhomChucDanh.Equals("Công Ty và Ban Phòng Nghiệp Vụ")))
                return BadRequest("Nhóm chức danh phải là 1 trong 3: 1.Công Ty, 2.Ban Phòng Nghiệp Vụ, 3.Công Ty và Ban Phòng Nghiệp Vụ");
            if(!(model.ChuKyDanhGia.Equals("Tháng") || model.ChuKyDanhGia.Equals("Năm")))
                return BadRequest("Chu kỳ đánh giá phải là Tháng hoặc Năm");
            if (model.ChiTieuTyTrongList.Sum(r => r.ChiTieu) != 100)
                return BadRequest("Tổng tất cả các chỉ tiêu phải là 100");
            if (!model.ChiTieuTyTrongList.Any(r => r.ToanTu.Equals("=")
                || r.ToanTu.Equals(">=") || r.ToanTu.Equals("<=")))
                return BadRequest("Các toàn tử phải là =, <=, >=");

            var duplicateChiTieu = model.ChiTieuTyTrongList
            .GroupBy(dto => new { dto.DanhMucNhomPiId })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
            if (duplicateChiTieu.Any())
            {
                Console.WriteLine("Có các phần tử trùng lặp trong danh sách ChiTieuTyTrongList.");
            }

            var duplicateChucDanh = model.ChucDanhList
            .GroupBy(dto => dto.ChucDanhId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

            if (duplicateChucDanh.Any())
            {
                Console.WriteLine("Có các phần tử trùng lặp trong danh sách ChucDanhList.");
            }
            var validationResult = ValidateChucDanhList(model.ChucDanhList, model.ChuKyDanhGia, model.NhomChucDanh, null);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return BadRequest(validationResult); // Return error if validation fails
            }
            var chiTieuTyTrongTable = new DataTable();
            chiTieuTyTrongTable.Columns.Add("ChiTieu", typeof(float));
            chiTieuTyTrongTable.Columns.Add("ToanTu", typeof(string));
            chiTieuTyTrongTable.Columns.Add("DanhMucNhomPiId", typeof(Guid));

            foreach (var item in model.ChiTieuTyTrongList)
            {
                chiTieuTyTrongTable.Rows.Add(item.ChiTieu, item.ToanTu, item.DanhMucNhomPiId);
            }

            SqlParameter chiTieuTyTrongParam = new SqlParameter
            {
                ParameterName = "@ChiTieuTyTrongList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.ChiTieuTyTrongType", // Đặt đúng tên của Table-Valued Parameter (TVP) trong database
                Value = chiTieuTyTrongTable
            };

            // Prepare table-valued parameters for ChucDanh
            var chucDanhTable = new DataTable();
            chucDanhTable.Columns.Add("ChucDanhId", typeof(Guid));

            foreach (var item in model.ChucDanhList)
            {
                chucDanhTable.Rows.Add(item.ChucDanhId);
            }
            SqlParameter chucDanhTyTrongParam = new()
            {
                ParameterName = "@ChucDanhList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.ChucDanhType", // Đặt đúng tên của Table-Valued Parameter (TVP) trong database
                Value = chucDanhTable
            };

            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_AddDanhMucTyTrong");
            dbAdapter.sqlCommand.Parameters.Add("@NhomChucDanh", SqlDbType.NVarChar).Value = model.NhomChucDanh;
            dbAdapter.sqlCommand.Parameters.Add("@ChuKyDanhGia", SqlDbType.NVarChar).Value = model.ChuKyDanhGia;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@BatBuocDung", model.BatBuocDung);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsKhong", model.IsKhong);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
            dbAdapter.sqlCommand.Parameters.Add(chiTieuTyTrongParam);
            dbAdapter.sqlCommand.Parameters.Add(chucDanhTyTrongParam);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Thêm thành công.");
            else
                return BadRequest("Thêm thất bại do mã đã tồn tại.");
            
        }

        private string ValidateChucDanhList(List<ChucDanhDTO> chucDanhList, string ChuKyDanhGia, string nhomChucDanh, Guid? Id)
        {
            Id = Id ?? Guid.Empty;
            var chucDanhIds = chucDanhList.Select(d => d.ChucDanhId).ToList();
            var chucDanhTyTrongs = _uow.ChucDanhTyTrongs
                .GetAll(ctt => chucDanhIds.Contains(ctt.ChucDanhId) && !ctt.IsDeleted && ctt.DanhMucTyTrongId != Id);
            var danhMucTyTrongIds = chucDanhTyTrongs.Select(ctt => ctt.DanhMucTyTrongId).Distinct().ToList();
            var danhMucTyTrongs = _uow.DanhMucTyTrongs
                .GetAll(dmt => danhMucTyTrongIds.Contains(dmt.Id) && !dmt.IsDeleted && dmt.Id != Id && dmt.ChuKyDanhGia.Equals(ChuKyDanhGia));
            foreach (var chucDanh in chucDanhTyTrongs)
            {
                var danhMucTyTrong = danhMucTyTrongs.FirstOrDefault(dmt => dmt.Id == chucDanh.DanhMucTyTrongId);
                if (danhMucTyTrong != null)
                {
                    // Nếu nhóm hiện tại là "Công Ty và ban phòng nghiệp vụ"
                    if (nhomChucDanh == "Công Ty và Ban Phòng Nghiệp Vụ")
                    {
                        // Không được thêm vào nếu đã tồn tại trong "Công Ty" hoặc "Ban phòng nghiệp vụ"
                        if (danhMucTyTrong.NhomChucDanh == "Công Ty" || danhMucTyTrong.NhomChucDanh == "Ban Phòng Nghiệp Vụ")
                        {
                            return $"Chức danh  đã tồn tại trong nhóm 'Công Ty' hoặc 'Ban phòng nghiệp vụ' và không thể thêm vào 'Công Ty và ban phòng nghiệp vụ'.";
                        }
                    }
                    // Nếu nhóm hiện tại là "Công Ty" hoặc "Ban phòng nghiệp vụ"
                    else if (nhomChucDanh == "Công Ty" || nhomChucDanh == "Ban Phòng Nghiệp Vụ")
                    {
                        // Không được thêm vào "Công Ty và ban phòng nghiệp vụ" nếu đã tồn tại trong "Công Ty và ban phòng nghiệp vụ"
                        if (danhMucTyTrongs.Any(dmt => dmt.NhomChucDanh == "Công Ty và Ban Phòng Nghiệp Vụ"))
                        {
                            return $"Chức danh {chucDanh.ChucDanhId} đã tồn tại trong nhóm Công Ty và ban phòng nghiệp vụ.";
                        }
                    }
                }
            }
            // Lấy tất cả các chức danh trong danh mục tỷ trọng khác
            var existingTyTrongs = _uow.DanhMucTyTrongs
                .GetAll(t => t.NhomChucDanh == nhomChucDanh && !t.IsDeleted && t.Id != Id && t.ChuKyDanhGia.Equals(ChuKyDanhGia)) // Lọc theo nhóm chức danh
                .Where(t => _uow.ChucDanhTyTrongs.Any(c => chucDanhIds.Contains(c.ChucDanhId) && c.DanhMucTyTrongId == t.Id && !c.IsDeleted)); // Kiểm tra nếu tồn tại chức danh

            // Nếu có bất kỳ danh mục tỷ trọng nào trùng nhóm và chứa chức danh, trả về lỗi
            if (existingTyTrongs.Any())
            {
                return "Chức danh đã tồn tại trong một danh mục tỷ trọng khác với cùng nhóm chức danh.";
            }

            return null;

        }


        [HttpPut]
        public IActionResult UpdateDanhMucTyTrong([FromBody] ClassDanhMucTyTrongDTO model)
        {
            if (model == null)
                return BadRequest("Invalid data.");
            if (model.Id == null || model.Id == Guid.Empty)
                return BadRequest("Id không hợp lệ");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!(model.NhomChucDanh.Equals("Công Ty")
                || model.NhomChucDanh.Equals("Ban Phòng Nghiệp Vụ")
                || model.NhomChucDanh.Equals("Công Ty và Ban Phòng Nghiệp Vụ")))
                return BadRequest("Nhóm chức danh phải là 1 trong 3: 1.Công Ty, 2.Ban Phòng Nghiệp Vụ, 3.Công Ty và Ban Phòng Nghiệp Vụ");
            if (!(model.ChuKyDanhGia.Equals("Tháng") || model.ChuKyDanhGia.Equals("Năm")))
                return BadRequest("Chu kỳ đánh giá phải là Tháng hoặc Năm");
            if (model.ChiTieuTyTrongList.Sum(r => r.ChiTieu) != 100)
                return BadRequest("Tổng tất cả các chỉ tiêu phải là 100");
            if (!model.ChiTieuTyTrongList.Any(r => r.ToanTu.Equals("=")
                || r.ToanTu.Equals(">=") || r.ToanTu.Equals("<=")))
                return BadRequest("Các toàn tử phải là =, <=, >=");

            var duplicateChiTieu = model.ChiTieuTyTrongList
            .GroupBy(dto => new { dto.DanhMucNhomPiId })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
            if (duplicateChiTieu.Any())
            {
                Console.WriteLine("Có các phần tử trùng lặp trong danh sách ChiTieuTyTrongList.");
            }

            var duplicateChucDanh = model.ChucDanhList
            .GroupBy(dto => dto.ChucDanhId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

            if (duplicateChucDanh.Any())
            {
                Console.WriteLine("Có các phần tử trùng lặp trong danh sách ChucDanhList.");
            }
            var validationResult = ValidateChucDanhList(model.ChucDanhList,model.ChuKyDanhGia, model.NhomChucDanh, model.Id);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return BadRequest(validationResult); // Return error if validation fails
            }
            var chiTieuTyTrongTable = new DataTable();
            chiTieuTyTrongTable.Columns.Add("ChiTieu", typeof(float));
            chiTieuTyTrongTable.Columns.Add("ToanTu", typeof(string));
            chiTieuTyTrongTable.Columns.Add("DanhMucNhomPiId", typeof(Guid));

            foreach (var item in model.ChiTieuTyTrongList)
            {
                chiTieuTyTrongTable.Rows.Add(item.ChiTieu, item.ToanTu, item.DanhMucNhomPiId);
            }

            SqlParameter chiTieuTyTrongParam = new SqlParameter
            {
                ParameterName = "@ChiTieuTyTrongList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.ChiTieuTyTrongType", // Đặt đúng tên của Table-Valued Parameter (TVP) trong database
                Value = chiTieuTyTrongTable
            };

            // Prepare table-valued parameters for ChucDanh
            var chucDanhTable = new DataTable();
            chucDanhTable.Columns.Add("ChucDanhId", typeof(Guid));

            foreach (var item in model.ChucDanhList)
            {
                chucDanhTable.Rows.Add(item.ChucDanhId);
            }
            SqlParameter chucDanhTyTrongParam = new()
            {
                ParameterName = "@ChucDanhList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.ChucDanhType", // Đặt đúng tên của Table-Valued Parameter (TVP) trong database
                Value = chucDanhTable
            };

            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_UpdateDanhMucTyTrong");
            dbAdapter.sqlCommand.Parameters.Add("@NhomChucDanh", SqlDbType.NVarChar).Value = model.NhomChucDanh;
            dbAdapter.sqlCommand.Parameters.Add("@ChuKyDanhGia", SqlDbType.NVarChar).Value = model.ChuKyDanhGia;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@BatBuocDung", model.BatBuocDung);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsKhong", model.IsKhong);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
            dbAdapter.sqlCommand.Parameters.Add(chiTieuTyTrongParam);
            dbAdapter.sqlCommand.Parameters.Add(chucDanhTyTrongParam);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Thêm thành công.");
            else
                return BadRequest("Thêm thất bại do mã đã tồn tại.");
        }

        [HttpGet("GetAllByChuKyAndKeyword")]
        public IActionResult GetAllByChuKyAndKeyword(string chuKyDanhGia = "", string keyword = "", int page = 1, int pageSize = 10)
        {
            // Kết nối tới cơ sở dữ liệu
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllByChuKyAndKeyword");
            dbAdapter.sqlCommand.Parameters.Add("@ChuKyDanhGia", SqlDbType.NVarChar).Value = chuKyDanhGia;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword ?? string.Empty;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            if (!string.IsNullOrEmpty(result.ToString()))
            {
                var jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());
                var danhMucTyTrongList = jsonObject.Select(item => new
                {
                    danhMucTyTrongId = Guid.TryParse(item.id?.ToString(), out Guid id) ? id : Guid.Empty,
                    chuKyDanhGia = item.chuKyDanhGia?.ToString(),
                    nhomChucDanh = item.nhomChucDanh?.ToString(),
                    chiTieuTyTrongList = ((IEnumerable<dynamic>)item.chiTieuTyTrongList ?? Enumerable.Empty<dynamic>())
                    .Select(subItem => new
                    {
                        subItem.toanTu,
                        subItem.chiTieu,
                        tenDanhMucNhomPI = subItem.tenDanhMucNhomPI?.ToString()
                    }).ToList(),
                    chucDanhTyTrongList = ((IEnumerable<dynamic>)item.chucDanhTyTrongList ?? Enumerable.Empty<dynamic>())
                    .Select(subItem => new
                    {
                        tenChucDanh = subItem.tenChucDanh?.ToString()
                    }).ToList()
                }).ToList();

                var totalRow = danhMucTyTrongList.Count;
                var totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
                if (page < 1) page = 1;
                if (page > totalPage) page = totalPage;

                var dataList = danhMucTyTrongList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
                return Ok(new
                {
                    totalRow = 0,
                    totalPage = 0,
                    pageSize
                });
            }
        }




        [HttpGet("GetDanhMucTyTrongById")]
        public IActionResult GetDanhMucTyTrongById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("ID không tồn tại");
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_GetDanhMucTyTrongById");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                var result = dbAdapter.runStored2JSON();
                dbAdapter.deConnect();
                var jsonObject = JsonConvert.DeserializeObject(result.ToString());
                if (jsonObject == null)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu." });
                }
                return Ok(jsonObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("Id null or empty");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteDanhMucTyTrong");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Cập nhật thành công.");
            else
                return BadRequest("Cập nhật thất bại do Id không tồn tại");
        }
    }
}
