using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ERP.Infrastructure;
using System;
using System.Linq;
using System.Data;
using ThacoLibs;
using Microsoft.Extensions.Configuration;
using ERP.Models.Default;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ThongBaoHeThongController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly DbAdapter dbAdapter;
        public ThongBaoHeThongController(IConfiguration _configuration, IUnitofWork _uow)
        {
            uow = _uow;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        [HttpGet]
        public ActionResult Get(Guid phanMem_Id, Guid? donVi_Id)
        {
            DateTime now = DateTime.Now;
            var list_ChiTiets = uow.ThongBaoHeThongs.GetAll(x => x.PhanMem_Id == phanMem_Id && (donVi_Id == null || x.DonVi_Id == donVi_Id) && x.User_Id == Guid.Parse(User.Identity.Name))
                .OrderByDescending(x => x.ThoiGian)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Body,
                    x.DuongDan,
                    ThoiGian = GetElapsedTime(x.ThoiGian, now),
                    x.Icon,
                    x.IsDaXem,
                });
            return Ok(new
            {
                SoLuongChuaXem = list_ChiTiets.Count(x => !x.IsDaXem),
                list_ChiTiets,
            });
        }
        private static string GetElapsedTime(DateTime createdDate, DateTime now)
        {
            TimeSpan timeDifference = now - createdDate;
            if ((int)timeDifference.TotalMinutes < 1)
            {
                // Dưới 1 phút
                return $"Vài giây trước";
            }
            else if (timeDifference.TotalMinutes < 60)
            {
                // Dưới 1 giờ
                return $"{(int)timeDifference.TotalMinutes} phút trước";
            }
            else if (timeDifference.TotalHours < 24)
            {
                // Dưới 1 ngày
                return $"{(int)timeDifference.TotalHours} giờ trước";
            }
            else if (timeDifference.TotalDays < 7)
            {
                // Dưới 1 tuần
                return $"{(int)timeDifference.TotalDays} ngày trước";
            }
            else
            {
                // Hơn 1 tuần
                return createdDate.ToString("HH:mm dd/MM/yyyy");
            }
        }
        [HttpPut("danh-dau-da-xem/{id}")]
        public ActionResult PutSetDaXem(Guid id)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                ThongBaoHeThong thongBaoHeThong = uow.ThongBaoHeThongs.GetById(id);
                if (thongBaoHeThong == null) return NotFound();
                thongBaoHeThong.IsDaXem = true;
                uow.ThongBaoHeThongs.Update(thongBaoHeThong);
                uow.Complete();
                return Ok();
            }
        }
        [HttpPut("danh-dau-da-xem-tat-ca")]
        public ActionResult PutSetDaXemAll(Guid phanMem_Id, Guid donVi_Id)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Put_ThongBaoHeThong_IsDaXemAll");
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_Id;
                dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok();
            }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                ThongBaoHeThong thongBaoHeThong = uow.ThongBaoHeThongs.GetById(id);
                if (thongBaoHeThong == null) return NotFound();
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Delete_ThongBaoHeThong");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok();
            }
        }
        [HttpDelete("xoa-tat-ca")]
        public ActionResult DeleteAll(Guid phanMem_Id, Guid donVi_Id)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Delete_ThongBaoHeThong_All");
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_Id;
                dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
                dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                return Ok();
            }
        }
    }
}
