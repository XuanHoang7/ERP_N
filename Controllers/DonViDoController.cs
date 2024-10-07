using DocumentFormat.OpenXml.Office2010.Excel;
using ERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ThacoLibs;
using static ERP.Data.MyDbContext;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DonViDoController : ControllerBase
    {

        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonViDoController(IConfiguration _configuration, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDonViDoById");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
            var result = dbAdapter.runStored2Object();
            dbAdapter.deConnect();

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet]
        public IActionResult GetAllByKeyword(string keyword, int page = 1, int pageSize = 10)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllDonViDoByKeyword");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
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

        [HttpPost]
        public IActionResult Add([FromBody] ClassDonViDo dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_AddDonViDo");
                dbAdapter.sqlCommand.Parameters.Add("@MaDonViDo", SqlDbType.NVarChar).Value = dto.MaDonViDo;
                dbAdapter.sqlCommand.Parameters.Add("@TenDonViDo", SqlDbType.NVarChar).Value = dto.TenDonViDo;
                dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
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

        [HttpPut]
        public IActionResult Update(ClassDonViDo dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_UpdateDonViDo");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = dto.Id;
                dbAdapter.sqlCommand.Parameters.Add("@MaDonViDo", SqlDbType.NVarChar).Value = dto.MaDonViDo;
                dbAdapter.sqlCommand.Parameters.Add("@TenDonViDo", SqlDbType.NVarChar).Value = dto.TenDonViDo;
                dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                {
                    return Ok("thành công.");
                }
                else
                {
                    return BadRequest("thất bại do Id không tồn tại");
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

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if(id == null || id == Guid.Empty)
            {
                return BadRequest("id not null or empty");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteDonViDo");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            dbAdapter.runStoredNoneQuery();
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
            {
                return Ok("Xóa thành công.");
            }
            else
            {
                return BadRequest("Xóa thất bại do Id không tồn tại. ");
            }
        }

    }
}
