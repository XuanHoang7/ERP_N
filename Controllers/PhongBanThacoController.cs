using ERP.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Data;
using ThacoLibs;
using Microsoft.Extensions.Configuration;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PhongBanThacoController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly DbAdapter dbAdapter;
        public PhongBanThacoController(IConfiguration _configuration, IUnitofWork _uow)
        {
            uow = _uow;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        [HttpGet]
        public ActionResult Get(Guid? donVi_Id, string keyword, int page = 1)
        {
            keyword = keyword?.Trim().ToUpper();
            if (string.IsNullOrEmpty(keyword)) keyword = null;
            var data = uow.PhongBanThacos.GetAll(x =>
                    (donVi_Id == null || x.DonVi_Id == donVi_Id)
                    && (keyword == null || x.MaPhongBan.Contains(keyword) || x.TenPhongBan.Contains(keyword))
                    , null, new string[] { "DonVi" }
                )
                .Select(x => new
                {
                    x.Id,
                    x.CapDo,
                    x.ThuTuCap1,
                    x.ThuTuCap2,
                    x.ThuTuCap3,
                    x.ThuTuCap4,
                    x.ThuTuCap5,
                    x.ThuTuCap6,
                    x.ThuTuCap7,
                    x.ThuTuCap8,
                    x.MaPhongBan,
                    x.TenPhongBan,
                    x.Parent_Id,
                    x.TenCap1,
                    x.TenCap2,
                    x.TenCap3,
                    x.TenCap4,
                    x.TenCap5,
                    x.TenCap6,
                    x.TenCap7,
                    x.TenCap8,
                    x.DonVi_Id,
                    x.DonVi?.MaDonVi,
                    x.DonVi?.TenDonVi,
                })
                .OrderBy(x => x.ThuTuCap1)
                .ThenBy(x => x.ThuTuCap2)
                .ThenBy(x => x.ThuTuCap3)
                .ThenBy(x => x.ThuTuCap4)
                .ThenBy(x => x.ThuTuCap5)
                .ThenBy(x => x.ThuTuCap6)
                .ThenBy(x => x.ThuTuCap7)
                .ThenBy(x => x.ThuTuCap8);
            if (page == -1)
            {
                return Ok(data);
            }
            else
            {
                int totalRow = data.Count();
                int pageSize = uow.Configs.FirstOrDefault(x => !x.IsDeleted)?.PageSize ?? 20;
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
                return Ok(new
                {
                    totalRow,
                    totalPage,
                    pageSize,
                    datalist
                });
            }
        }
        [HttpGet("phong-ban-co-nguoi")]
        public ActionResult GetPhongBanCoNguoi(Guid? donVi_Id, string keyword, int page = 1)
        {
            keyword = keyword?.Trim().ToUpper();
            if (string.IsNullOrEmpty(keyword)) keyword = null;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetList_PhongBanThaco");
            dbAdapter.sqlCommand.Parameters.Add("@keyword", SqlDbType.NVarChar).Value = keyword;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            if (page == -1)
            {
                return Ok(data);
            }
            else
            {
                int totalRow = data.Count;
                int pageSize = uow.Configs.FirstOrDefault(x => !x.IsDeleted)?.PageSize ?? 20;
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
                return Ok(new
                {
                    totalRow,
                    totalPage,
                    pageSize,
                    datalist
                });
            }
        }
    }
}
