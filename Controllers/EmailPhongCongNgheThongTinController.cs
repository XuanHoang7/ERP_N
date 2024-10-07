using ERP.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using static ERP.SendEmailLibs;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EmailPhongCongNgheThongTinController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DbAdapter dbAdapter;
        private readonly SendEmailLibs sendEmail;
        public EmailPhongCongNgheThongTinController(IConfiguration _configuration, IUnitofWork _uow, UserManager<ApplicationUser> _userManager, SendEmailLibs _sendEmail)
        {
            uow = _uow;
            userManager = _userManager;
            sendEmail = _sendEmail;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        [HttpGet]
        public ActionResult Get()
        {
            var appUser = userManager.FindByIdAsync(User.Identity.Name).Result;
            var IsAdministrator = userManager.IsInRoleAsync(appUser, "Administrator").Result;
            if (!IsAdministrator) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            var email = uow.EmailPhongCongNgheThongTins.FirstOrDefault(x => true);
            return Ok(email?.Email);
        }
        [HttpGet("is-administrator")]
        public ActionResult GetIsAdminVPTQ()
        {
            var appUser = userManager.FindByIdAsync(User.Identity.Name).Result;
            var IsAdministrator = userManager.IsInRoleAsync(appUser, "Administrator").Result;
            return Ok(IsAdministrator);
        }
        public class Class_EmailPhongCongNgheThongTinPut
        {
            [Required]
            [StringLength(50)]
            public string Email { get; set; }
            [Required]
            [StringLength(250)]
            public string Password { get; set; }
        }
        [HttpPut("email")]
        public ActionResult PutEmail(Class_EmailPhongCongNgheThongTinPut data)
        {
            var appUser = userManager.FindByIdAsync(User.Identity.Name).Result;
            var IsAdministrator = userManager.IsInRoleAsync(appUser, "Administrator").Result;
            if (!IsAdministrator) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string testEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(data.Email, testEmail))
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"Địa chỉ email không đúng định dạng!");
                }
                if (!data.Email.Contains("@thaco.com.vn"))
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"Địa chỉ email phải là mail Thaco!");
                }
                var email = uow.EmailPhongCongNgheThongTins.FirstOrDefault(x => true);
                if (email == null)
                {
                    try
                    {
                        sendEmail.SendEmailCongNgheThongTin(data.Email, data.Password, "Hoàn tất Đăng ký email phòng Công nghệ thông tin", $"Bạn đã đăng ký địa chỉ email {data.Email} làm email để gửi thông báo mặc định của các phần mềm thuộc phòng công nghệ thông tin!", new List<string> { data.Email });
                        System.Threading.Thread.Sleep(600);
                        ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                        service.Credentials = new WebCredentials(data.Email, data.Password);
                        service.Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx");
                        // Tìm kiếm một số mục trong hộp thư
                        FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, new ItemView(1)).Result;
                        if (findResults.Any())
                        {
                            //Thay đổi email đăng ký
                            dbAdapter.connect();
                            dbAdapter.createStoredProceder("sp_Put_EmailPhongCongNgheThongTin");
                            dbAdapter.sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = data.Email;
                            //hash password
                            string PasswordHash = EncryptPassword(EncryptPassword(data.Password));
                            dbAdapter.sqlCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = PasswordHash;
                            dbAdapter.runStoredNoneQuery();
                            dbAdapter.deConnect();
                            return Ok();
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status409Conflict, $"Địa chỉ email hoặc mật khẩu không đúng!");
                        }
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status409Conflict, $"Địa chỉ email hoặc mật khẩu không đúng!");
                    }
                }
                else
                {
                    if (email.Email == data.Email)
                    {
                        return StatusCode(StatusCodes.Status409Conflict, $"Địa chỉ email không thay đổi!");
                    }
                    try
                    {
                        sendEmail.SendEmailCongNgheThongTin(data.Email, data.Password, "Hoàn tất Đăng ký email phòng Công nghệ thông tin", $"Bạn đã đăng ký địa chỉ email {data.Email} làm email để gửi thông báo mặc định của các phần mềm thuộc phòng công nghệ thông tin!", new List<string> { data.Email });
                        System.Threading.Thread.Sleep(600);
                        ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                        service.Credentials = new WebCredentials(data.Email, data.Password);
                        service.Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx");
                        // Tìm kiếm một số mục trong hộp thư
                        FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, new ItemView(1)).Result;
                        if (findResults.Any())
                        {
                            //Thay đổi email đăng ký
                            dbAdapter.connect();
                            dbAdapter.createStoredProceder("sp_Put_EmailPhongCongNgheThongTin");
                            dbAdapter.sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = data.Email;
                            //hash password
                            string PasswordHash = EncryptPassword(EncryptPassword(data.Password));
                            dbAdapter.sqlCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = PasswordHash;
                            dbAdapter.runStoredNoneQuery();
                            dbAdapter.deConnect();
                            sendEmail.SendEmailCongNgheThongTin(email.Email, email.PasswordHash, "Hoàn tất thay đổi email Phòng Công nghệ thông tin", $"Bạn đã thay đổi email gửi thông báo phòng Công nghệ thông tin sang địa chỉ email {data.Email} thành công!", new List<string> { email.Email }, true);
                            return Ok();
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status409Conflict, $"Địa chỉ email hoặc mật khẩu không đúng!");
                        }
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status409Conflict, $"Địa chỉ email hoặc mật khẩu không đúng!");
                    }
                }
            }
        }
        public class Class_EmailPhongCongNgheThongTinPutPassword
        {
            [Required]
            [StringLength(250)]
            public string PasswordOld { get; set; }
            [Required]
            [StringLength(250)]
            public string PasswordNew { get; set; }
        }
        [HttpPut("password")]
        public ActionResult PutPassword(Class_EmailPhongCongNgheThongTinPutPassword data)
        {
            var appUser = userManager.FindByIdAsync(User.Identity.Name).Result;
            var IsAdministrator = userManager.IsInRoleAsync(appUser, "Administrator").Result;
            if (!IsAdministrator) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var email = uow.EmailPhongCongNgheThongTins.FirstOrDefault(x => true);
                var password = DecryptPassword(DecryptPassword(email.PasswordHash));
                if (password != data.PasswordOld)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Mật khẩu cũ không đúng!");
                }
                if (data.PasswordNew == data.PasswordOld)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Mật khẩu không thay đổi!");
                }
                try
                {
                    sendEmail.SendEmailCongNgheThongTin(email.Email, data.PasswordNew, "Thay đổi mật khẩu email gửi thông báo thành công", $"Bạn đã thay đổi mật khẩu email phòng Công nghệ thông tin thành công!", new List<string> { email.Email });
                    ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                    service.Credentials = new WebCredentials(email.Email, data.PasswordNew);
                    service.Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx");
                    // Tìm kiếm một số mục trong hộp thư
                    FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, new ItemView(1)).Result;
                    if (findResults.Any())
                    {
                        //Thay đổi email đăng ký
                        dbAdapter.connect();
                        dbAdapter.createStoredProceder("sp_Put_EmailPhongCongNgheThongTin");
                        dbAdapter.sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email.Email;
                        //hash password
                        string PasswordHash = EncryptPassword(EncryptPassword(data.PasswordNew));
                        dbAdapter.sqlCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = PasswordHash;
                        dbAdapter.runStoredNoneQuery();
                        dbAdapter.deConnect();
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status409Conflict, $"Mật khẩu mới không đúng!");
                    }
                }
                catch
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"Mật khẩu mới không đúng!");
                }
            }
        }
    }
}