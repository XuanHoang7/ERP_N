using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ERP.Data.MyDbContext;
using ERP.Infrastructure;
using Microsoft.Extensions.Configuration;
using ThacoLibs;
using System.Data;
using System.Dynamic;
using static ERP.Controllers.PhanMemController;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RoleController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly DbAdapter dbAdapter;
        public RoleController(IConfiguration _configuration,  IUnitofWork _uow, RoleManager<ApplicationRole> _roleManager)
        {
            uow = _uow;
            roleManager = _roleManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        [HttpPost]
        public async Task<IActionResult> Post(ApplicationRole duLieu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var PhanMemDonViURL = uow.PhanMemDonViURLs.FirstOrDefault(x => x.DonVi_Id == duLieu.DonVi_Id && x.PhanMem_Id == duLieu.PhanMem_Id);
            if (PhanMemDonViURL == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Chưa khai báo thông tin đường dẫn của phần mềm theo đơn vị");
            }
            var exit = await roleManager.FindByNameAsync(duLieu.Name);
            if (exit == null)
            {
                duLieu.CreatedDate = DateTime.Now;
                IdentityResult result = await roleManager.CreateAsync(duLieu);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                    return BadRequest(string.Join(",", result.Errors));
            }
            else
            {
                if (exit.IsDeleted)
                {
                    exit.IsDeleted = false;
                    exit.DeletedDate = null;
                    exit.Description = duLieu.Description;
                    exit.Name = duLieu.Name;
                    exit.PhanMem_Id = duLieu.PhanMem_Id;
                    exit.DonVi_Id = duLieu.DonVi_Id;
                    exit.TapDoan_Id = duLieu.TapDoan_Id;
                    exit.PhongBan_Id = duLieu.PhongBan_Id;
                    var result = await roleManager.UpdateAsync(exit);
                    if (result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status204NoContent);
                    }
                    else
                        return BadRequest(string.Join(",", result.Errors));
                }
                return StatusCode(StatusCodes.Status409Conflict, "Thông tin vai trò đã tồn tại");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, ApplicationRole duLieu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != duLieu.Id.ToString())
            {
                return BadRequest();
            }
            if (await roleManager.RoleExistsAsync(duLieu.Name) && duLieu.Id.ToString() != id)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Thông tin vai trò đã tồn tại");
            }
            else
            {
                var role = await roleManager.FindByIdAsync(id);
                if (
                    role.PhanMem_Id == PhanMemDaoTao_Id
                    && (
                        role.Name.Contains("ADMINISTRATOR_DAOTAO_")
                        || role.Name.Contains("QLKIEMTRA_DAOTAO_")
                        || role.Name.Contains("QLDANGKYVATHEODOI_DAOTAO_")
                        || role.Name.Contains("QLDAOTAO_DAOTAO_")
                        || role.Name.Contains("QLBAOCAO_DAOTAO_")
                        || role.Name.Contains("QLTAILIEUTHAMKHAO_DAOTAO_")
                        || role.Name == "QUANTRI_CBNV_ERP")
                )
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Quyền đã được sử dụng! Không thể sửa! Liên hệ IT để được hỗ trợ!");
                }
                role.Description = duLieu.Description;
                role.UpdatedDate = DateTime.Now;
                role.Name = duLieu.Name;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                return BadRequest(string.Join(",", result.Errors));
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var duLieu = await roleManager.FindByIdAsync(id);
            if (duLieu == null)
            {
                return NotFound();
            }
            return Ok(duLieu);
        }
        [HttpGet]
        public ActionResult Get(int page = 1, int pageSize = 20, string keyword = null, Guid? PhanMem_Id = null, Guid? DonVi_Id = null, Guid? TapDoan_Id = null, Guid? PhongBan_Id = null)
        {
            var query = roleManager.Roles.Include(x => x.UserRoles).Include(x => x.Menu_Roles)
                .Where(x => 
                    (string.IsNullOrEmpty(keyword) || x.Name.ToLower().Contains(keyword.ToLower()) || x.Description.ToLower().Contains(keyword.ToLower()))
                    && !x.IsDeleted 
                    && x.PhanMem_Id == PhanMem_Id 
                    && x.DonVi_Id == DonVi_Id
                    && (TapDoan_Id == null || x.TapDoan_Id == TapDoan_Id) 
                    && (PhongBan_Id == null || x.PhongBan_Id == PhongBan_Id));
            var data = query.OrderByDescending(a => a.Description)
                .Select(x => new { 
                    x.Id, 
                    x.Name, 
                    x.Description, 
                    IsUsed = (x.UserRoles.Count > 0 || x.Menu_Roles.Where(a => !a.IsDeleted).Count() > 0), 
                    x.PhanMem_Id, 
                    x.DonVi_Id, 
                    x.TapDoan_Id, 
                    x.PhongBan_Id 
                });
            return Ok(data);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (
                    role.PhanMem_Id == PhanMemDaoTao_Id
                    && (
                        role.Name.Contains("ADMINISTRATOR_DAOTAO_")
                        || role.Name.Contains("QLKIEMTRA_DAOTAO_")
                        || role.Name.Contains("QLDANGKYVATHEODOI_DAOTAO_")
                        || role.Name.Contains("QLDAOTAO_DAOTAO_")
                        || role.Name.Contains("QLBAOCAO_DAOTAO_")
                        || role.Name.Contains("QLTAILIEUTHAMKHAO_DAOTAO_")
                        || role.Name == "QUANTRI_CBNV_ERP"
                    )
                )
            {
                return StatusCode(StatusCodes.Status409Conflict, "Quyền đã được sử dụng! Không thể xóa! Liên hệ IT để được hỗ trợ!");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetCheckRole_IsUseByUser");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            var re = dbAdapter.runStored2Object();
            if (re is ExpandoObject expandoObj && expandoObj.Any())
            {
                dbAdapter.deConnect();
                return StatusCode(StatusCodes.Status409Conflict, "Role đang có user sử dụng KHÔNG thể xóa!");
            }

            dbAdapter.createStoredProceder("sp_GetCheckRole_IsUseByMenu");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            var result2 = dbAdapter.runStored2Object();
            if (result2 is ExpandoObject expandoObj2 && expandoObj2.Any())
            {
                dbAdapter.deConnect();
                return StatusCode(StatusCodes.Status409Conflict, "Quyền đã có menu KHÔNG thể xóa!");
            }
            role.IsDeleted = true;
            role.DeletedDate = DateTime.Now;
            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK, "Xóa vai trò thành công");
            }
            return BadRequest(string.Join(",", result.Errors));
        }
        [HttpGet("Form")]
        public ActionResult Form()
        {
            var query = roleManager.Roles.Where(x => !x.IsDeleted);
            return Ok(query);
        }
    }
}