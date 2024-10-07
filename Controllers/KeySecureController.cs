using ERP.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
  [ApiKey]
  [EnableCors("CorsApi")]
  [AllowAnonymous]
  [Route("api/[controller]")]
  [ApiController]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class KeySecureController : ControllerBase
  {
    public KeySecureController()
    {
    }
    [HttpGet]
    public ActionResult Get()
    {
      return Ok("abc");
    }

  }
}