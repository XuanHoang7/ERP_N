using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ERP.Infrastructure;
using System.Linq;
using System;
namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CapDoNhanSuController : ControllerBase
    {
        private readonly IUnitofWork uow;
        public static IWebHostEnvironment environment;
        public CapDoNhanSuController(IUnitofWork _uow, IWebHostEnvironment _environment)
        {
            uow = _uow;
            environment = _environment;
        }
        [HttpGet]
        public ActionResult Get(string keyword, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;
            else keyword = keyword.Trim().ToUpper();
            var pageSizeData = uow.Configs.GetAll(x => !x.IsDeleted).ToList();
            var data = uow.CapDoNhanSus.GetAll(t => (keyword == null || t.MaCapDoNhanSu.ToUpper().Contains(keyword) || t.TenCapDoNhanSu.ToUpper().Contains(keyword))
                )
                .OrderByDescending(x => x.MaCapDoNhanSu)
                .Select(x => new
                {
                    x.Id,
                    x.MaCapDoNhanSu,
                    x.TenCapDoNhanSu,
                    x.MoTa,
                });
            if (data == null)
            {
                return NotFound();
            }
            if (page == -1)
            {
                return Ok(data);
            }
            else
            {
                int totalRow = data.Count();
                int pageSize = pageSizeData[0].PageSize;
                int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
                // Kiểm tra và điều chỉnh giá trị của page
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