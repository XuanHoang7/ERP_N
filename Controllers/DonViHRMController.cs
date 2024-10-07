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
    public class DonViHRMController : ControllerBase
    {
        private readonly IUnitofWork uow;
        public static IWebHostEnvironment environment;
        public DonViHRMController(IUnitofWork _uow, IWebHostEnvironment _environment)
        {
            uow = _uow;
            environment = _environment;
        }
        [HttpGet]
        public ActionResult Get(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;
            else keyword = keyword.Trim().ToUpper();
            var data = uow.DonViHRMs.GetAll(x => keyword == null || x.TenDonViHRM.ToUpper().Contains(keyword)).OrderBy(x => x.TenDonViHRM);
            return Ok(data);
        }
    }
}