using ERP.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using ERP.Models;
using System.Data;
using System;
using System.Linq;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsApi")]
    [Authorize]
    public class DM_KetQuaDanhGiaController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DM_KetQuaDanhGiaController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitof)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitof;
        }

        [HttpPut]
        public IActionResult Update(ClassDM_KetQuaDanhGia dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_uow.KetQuaDanhGias.Any(kq => kq.Id == dto.Id && !kq.IsDeleted))
                return Conflict("Id Danh mục kết quả đánh giá không tồn tại.");
            if (_uow.KetQuaDanhGias.Any(kq => kq.KetQuaDanhGia == dto.KetQuaDanhGia && !kq.IsDeleted && kq.Id != dto.Id))
                return Conflict("Kết quả đánh giá đã tồn tại trong danh sách.");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_UpdateDM_KetQuaDanhGia");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = dto.Id;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@KetQuaDanhGia", dto.KetQuaDanhGia);
            dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Update thành công.");
            else
                return NotFound("Id không tồn tại trong database.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteKQDanhGia(Guid id)
        {
            if (!_uow.KetQuaDanhGias.Any(kq => kq.Id == id && !kq.IsDeleted))
                return Conflict("Id Danh mục kết quả đánh giá không tồn tại.");
            var maxThuTuItem = _uow.KetQuaDanhGias.GetAll(kq => kq.Id == kq.Id && !kq.IsDeleted)
            .OrderByDescending(kq => kq.ThuTu)
            .FirstOrDefault();
            if (maxThuTuItem.Id != id)
                return BadRequest("Phải xóa phần tử có thứ tự lớn nhất trước!!!");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteDM_KetQuaDanhGiaById");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", id);
            dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return result > 0 ? Ok("Xóa thành công.") : BadRequest("Xóa không thành công.");
        }

        [HttpPost]
        public IActionResult AddKQDanhGia(ClassDM_KetQuaDanhGia dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (_uow.KetQuaDanhGias.Any(kq => kq.KetQuaDanhGia == dto.KetQuaDanhGia && !kq.IsDeleted))
                return Conflict("Kết quả đánh giá đã tồn tại trong danh sách.");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_AddDM_KetQuaDanhGia");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@KetQuaDanhGia", dto.KetQuaDanhGia);
            dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Thêm thành công.");
            else
                return BadRequest("Lỗi: Dữ liệu đã tồn tại hoặc không hợp lệ.");
        }

        [HttpGet]
        public IActionResult GetAllKQDanhGia(int page = 1, int pageSize = 10)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllDM_KetQuaDanhGia");
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            if (result == null || !result.Any())
                return NotFound("Không có dữ liệu.");
            int totalRow = result.Count;
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1) page = 1;
            else if (page > totalPage) page = totalPage;
            var pagedData = result.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                currentPage = page,
                data = pagedData
            });
        }

    }
}
