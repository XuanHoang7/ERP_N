using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ERP.Models;
using System;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DanhMucNhomPIController : ControllerBase
    {
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DanhMucNhomPIController(IConfiguration _configuration, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }

        [HttpPost]
        public ActionResult AddDanhMucNhomPI([FromBody] ClassDanhMucNhomPI model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_AddDanhMucNhomPI");
                dbAdapter.sqlCommand.Parameters.Add("@MaDanhMucNhomPI", SqlDbType.NVarChar).Value = model.MaDanhMucNhomPI;
                dbAdapter.sqlCommand.Parameters.Add("@TenDanhMucNhomPI", SqlDbType.NVarChar).Value = model.TenDanhMucNhomPI;
                dbAdapter.sqlCommand.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = model.TrangThai;
                dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = model.GhiChu ?? string.Empty;
                dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();

                if (result > 0)
                {
                    return Ok("thành công.");
                }
                else
                {
                    return BadRequest("thất bại.");
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
                    return StatusCode(500, new { Message = "Có lỗi xảy ra khi thêm danh mục." });
                }
            }
        }
        [HttpDelete("{id}")]
        public ActionResult DeleteDanhMucNhomPI(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("Id is null or empty");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteDanhMucNhomPIById");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);

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

        [HttpPut]
        public ActionResult UpdateDanhMucNhomPI([FromBody] ClassDanhMucNhomPI model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_UpdateDanhMucNhomPI");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                dbAdapter.sqlCommand.Parameters.Add("@MaDanhMucNhomPI", SqlDbType.NVarChar).Value = model.MaDanhMucNhomPI;
                dbAdapter.sqlCommand.Parameters.Add("@TenDanhMucNhomPI", SqlDbType.NVarChar).Value = model.TenDanhMucNhomPI;
                dbAdapter.sqlCommand.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = model.TrangThai;
                dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = model.GhiChu ?? string.Empty;
                dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);

                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                {
                    return Ok("Cập nhật thành công.");
                }
                else
                {
                    return BadRequest("Id không tồn tại");
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
        [HttpGet("search")]
        public ActionResult GetAllDanhMucNhomPIByKeyword(string keyword = "", int page = 1, int pageSize = 10)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllDanhMucNhomPIByKeyword");
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var data = dbAdapter.runStored2ObjectList();
            int totalRow = data.Count();
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPage)
            {
                page = totalPage;
            }
            var datalist = data.Skip((page - 1) * pageSize).Take(pageSize);
            dbAdapter.deConnect();
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                datalist
            });
        }
        [HttpGet("{id}")]
        public ActionResult GetDanhMucNhomPIById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("Id is null or empty");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucNhomPIById");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            var danhMucNhomPI = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            if (danhMucNhomPI == null)
            {
                return NotFound();
            }
            return Ok(danhMucNhomPI);
        }
    }
}
