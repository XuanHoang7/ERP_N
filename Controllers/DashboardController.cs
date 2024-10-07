using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DashboardController : ControllerBase
    {
        private MyDbContext db;
        private readonly IUnitofWork uow;
        public DashboardController(IUnitofWork _uow, MyDbContext _db)
        {
            uow = _uow;
            db = _db;
        }
        [HttpGet]
        public ActionResult Get(string user)
        {
            Expression<Func<Log, bool>> whereFunc = item => item.AccessdBy.ToString() == user;
            var result = uow.Logs.GetAll(whereFunc, null, null).Select(x => new { x.Id }).ToList();
            return Ok(result.Count);
        }
    }
}