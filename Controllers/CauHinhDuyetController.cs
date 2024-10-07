using ERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using ThacoLibs;
using static ERP.Data.MyDbContext;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CauHinhDuyetController : ControllerBase
    {
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public CauHinhDuyetController(IConfiguration _configuration, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        [HttpPut]
        public IActionResult Update(ClassCauHinhDuyet dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_UpdateCauHinhDuyet");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = dto.Id;
                dbAdapter.sqlCommand.Parameters.Add("@NhanVienId", SqlDbType.UniqueIdentifier).Value = dto.NhanVienId;
                dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@CapDuyet", dto.CapDuyet);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result > 0)
                    return Ok("Update success");
                else
                    return NotFound("Id không tồn tại hoặc trùng lặp dữ liệu");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý là " + ex.Message });
            }
        }

        [HttpPut("change-order")]
        public IActionResult ChangeThuTuDuyet(Guid id, int newThuTuDuyet)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_ChangeThuTuDuyet");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                dbAdapter.sqlCommand.Parameters.Add("@NewThuTuDuyet", SqlDbType.Int).Value = newThuTuDuyet;
                dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok("Thay đổi thứ tự thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("{id}")]
        public IActionResult GetCauHinhDuyetById(Guid id)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_GetCauHinhDuyetById");
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
        public IActionResult GetAllCauHinhDuyet(int page = 1, int pageSize = 10)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_GetAllCauHinhDuyet");
                var result = dbAdapter.runStored2ObjectList();
                dbAdapter.deConnect();
                int totalRow = result.Count;
                int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
                if (page < 1)
                    page = 1;
                else if (page > totalPage)
                    page = totalPage;
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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCauHinhDuyet(Guid id)
        {
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_DeleteCauHinhDuyetById");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
                dbAdapter.sqlCommand.Parameters.Add("@DeleteBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok("Delete success");
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
            }
        }

        [HttpPost]
        public IActionResult AddCauHinhDuyet(ClassCauHinhDuyet dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_AddCauHinhDuyet");
                dbAdapter.sqlCommand.Parameters.Add("@NhanVienId", SqlDbType.UniqueIdentifier).Value = dto.NhanVienId;
                dbAdapter.sqlCommand.Parameters.AddWithValue("@CapDuyet", dto.CapDuyet);
                dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);

                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();

                if (result > 0)
                    return Ok("Add success");
                else
                    return NotFound("IdUser không tồn tại hoặc dữ liệu đã tồn tại");
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình xử lý." });
            }
        }

    }
}
