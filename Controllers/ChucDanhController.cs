using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ERP.Infrastructure;
using System.Linq;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ChucDanhController : ControllerBase
    {
        private readonly IUnitofWork uow;
        public static IWebHostEnvironment environment;
        public ChucDanhController(IUnitofWork _uow, IWebHostEnvironment _environment)
        {
            uow = _uow;
            environment = _environment;
        }
        [HttpGet]
        public ActionResult Get(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;
            else keyword = keyword.Trim().ToUpper();
            var data = uow.ChucDanhs.GetAll(x => keyword == null || x.TenChucDanh.ToUpper().Contains(keyword)).OrderBy(x => x.TenChucDanh);
            return Ok(data);
        }
    }
}