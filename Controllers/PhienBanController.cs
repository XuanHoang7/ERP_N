using ERP.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThacoLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PhienBanController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly DbAdapter dbAdapter;
        public PhienBanController(IConfiguration _configuration, IUnitofWork _uow)
        {
            uow = _uow;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        [HttpGet]
        public ActionResult Get(string keyword, Guid phanMem_Id, Guid? donVi_Id)
        {
            var phienBans = uow.phienBans.GetAll(x => !x.IsDeleted && x.PhanMem_Id == phanMem_Id && (donVi_Id == null || x.DonVi_Id == donVi_Id) && (string.IsNullOrEmpty(keyword)
            || x.MaPhienBan.ToLower().Contains(keyword.Trim().ToLower())
            || x.MoTa.ToLower().Contains(keyword.Trim().ToLower())
            )).OrderByDescending(x => x.CreatedDate);
            return Ok(new
            {
                Success = true,
                Data = phienBans
            });
        }
        [HttpGet("kiem-tra-cap-nhat")]
        public ActionResult KiemTra(string MaPhienBan, Guid phanMem_Id, Guid? donVi_Id)
        {
            var phienban = uow.phienBans.GetAll(x => !x.IsDeleted && x.PhanMem_Id == phanMem_Id && (donVi_Id == null || x.DonVi_Id == donVi_Id) && x.IsSuDung).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            return Ok(new
            {
                Success = true,
                Data = new
                {
                    IsCapNhat = (phienban.MaPhienBan != MaPhienBan.Trim().ToUpper()),
                    info = phienban
                }
            });
        }
        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var phienban = uow.phienBans.GetById(id);
            return Ok(new
            {
                Success = true,
                Data = phienban
            });
        }
        public class Class_PhienBanPost
        {
            [StringLength(50)]
            [Required]
            public string MaPhienBan { get; set; }
            [StringLength(250)]
            [Required]
            public string MoTa { get; set; }
            [StringLength(250)]
            public string FileName { get; set; }
            [StringLength(500)]
            public string FileUrl { get; set; }
            public bool IsSuDung { get; set; }
            public Guid PhanMem_Id { get; set; }
            public Guid DonVi_Id { get; set; }
        }
        [HttpPost]
        public ActionResult Post(Class_PhienBanPost data)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (uow.phienBans.Exists(x => x.MaPhienBan.ToUpper() == data.MaPhienBan.Trim().ToUpper() && x.DonVi_Id == data.DonVi_Id && x.PhanMem_Id == data.PhanMem_Id && !x.IsDeleted))
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"Mã {data.MaPhienBan} đã tồn tại");
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_PostPhienBan");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                dbAdapter.sqlCommand.Parameters.Add("@MaPhienBan", SqlDbType.NVarChar).Value = data.MaPhienBan.Trim().ToUpper();
                dbAdapter.sqlCommand.Parameters.Add("@MoTa", SqlDbType.NVarChar).Value = data.MoTa;
                dbAdapter.sqlCommand.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = data.FileName;
                dbAdapter.sqlCommand.Parameters.Add("@FileUrl", SqlDbType.NVarChar).Value = data.FileUrl;
                dbAdapter.sqlCommand.Parameters.Add("@IsSuDung", SqlDbType.Bit).Value = data.IsSuDung;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = data.PhanMem_Id;
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = data.DonVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok();
            }
        }
        public class Class_PhienBanPut
        {
            public Guid Id { get; set; }
            [StringLength(50)]
            [Required]
            public string MaPhienBan { get; set; }
            [StringLength(250)]
            [Required]
            public string MoTa { get; set; }
            [StringLength(250)]
            public string FileName { get; set; }
            [StringLength(500)]
            public string FileUrl { get; set; }
        }
        [HttpPut("{id}")]
        public ActionResult Put(Guid id, Class_PhienBanPut data)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (id != data.Id)
                {
                    return BadRequest(ModelState);
                }
                var phienban = uow.phienBans.GetById(id);
                if (phienban.IsSuDung)
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"Phiên bản đang sử dụng không thể xóa");
                }
                if (uow.phienBans.Exists(x => x.Id != id && x.MaPhienBan.ToUpper() == data.MaPhienBan.Trim().ToUpper() && x.DonVi_Id == phienban.DonVi_Id && x.PhanMem_Id == phienban.PhanMem_Id && !x.IsDeleted))
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"Mã {data.MaPhienBan} đã tồn tại");
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_PutPhienBan");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                dbAdapter.sqlCommand.Parameters.Add("@MaPhienBan", SqlDbType.NVarChar).Value = data.MaPhienBan.Trim().ToUpper();
                dbAdapter.sqlCommand.Parameters.Add("@MoTa", SqlDbType.NVarChar).Value = data.MoTa;
                dbAdapter.sqlCommand.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = data.FileName;
                dbAdapter.sqlCommand.Parameters.Add("@FileUrl", SqlDbType.NVarChar).Value = data.FileUrl;
                dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok();
            }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            lock (Commons.LockObjectState)
            {
                dbAdapter.connect(); //Mở kết nối
                var phienban = uow.phienBans.GetById(id);
                if (phienban.IsSuDung)
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"Phiên bản đang sử dụng không thể xóa");
                }
                dbAdapter.createStoredProceder("sp_DeletePhienBan");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                dbAdapter.sqlCommand.Parameters.Add("@DeletedBy", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok();
            }
        }
    }
}
