using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThacoLibs;
namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class __TestController : ControllerBase
    {
        private readonly DbAdapter dbAdapter;
        public __TestController(IConfiguration _configuration)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
    }
}