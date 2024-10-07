using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ERP.Helpers;
using ERP.Models;
using static ERP.Data.MyDbContext;
using System.Linq;
using System.Data;
using System.Text.Json;
using System.IO;
using System.Security.Cryptography;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("token")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppSettings appSettings;
        private const string key = "THACOINDUSTRIES2";
        private const string iv = "@industries@2024";
        public AuthController(UserManager<ApplicationUser> _userManager, IOptions<AppSettings> _appSettings)
        {
            userManager = _userManager;
            appSettings = _appSettings.Value;
        }
        private static string EncryptPassword(string plainText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using StreamWriter swEncrypt = new(csEncrypt);
                swEncrypt.Write(plainText);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        private static string DecryptPassword(string cipherText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        [HttpGet("lay")]
        public ActionResult Getb(string lay)
        {
            string laypass = "";
            laypass = EncryptPassword(lay);
            return Ok(laypass);
        }
        [HttpGet("dich")]
        public ActionResult Geta(string dich)
        {
            string laypass = "";
            laypass = DecryptPassword(dich);
            return Ok(laypass);
        }
        [HttpPost]
        public async Task<IActionResult> Authencation(LoginModel model, bool isMobile)
        {
            model.Username = model.Username.Trim();
            try
            {
                model.Password = DecryptPassword(model.Password);
            }
            catch
            {
                return BadRequest("Lỗi frontend");
            }
            if (!string.IsNullOrEmpty(model.Domain))
            {
                string checkemail = model.Username + "@" + model.Domain;
                var appUser = userManager.Users.Where(x => x.Email == checkemail && !x.IsDeleted).FirstOrDefault();
                if (appUser == null)
                {
                    return BadRequest("Tài khoản không tồn tại");
                }
                if (!appUser.IsActive)
                {
                    return BadRequest("Tài khoản đã bị khóa hoặc chưa được kích hoạt");
                }
                var result = CheckLoginAsync(checkemail, model.Password);
                if (result)
                {
                    var role = await userManager.GetRolesAsync(appUser);
                    var token = GenToken(appUser.Id.ToString(), isMobile);
                    return Ok(new
                    {
                        token.Token,
                        token.Expires,
                        Id = appUser.Id.ToString(),
                        appUser.Email,
                        appUser.FullName,
                        AccessRole = JsonSerializer.Serialize(role),
                        appUser.HinhAnhUrl
                    });
                }
                else
                {
                    return BadRequest("Thông tin đăng nhập không đúng");
                }
            }
            else
            {
                var appUser = userManager.Users.FirstOrDefault(x => x.UserName == model.Username && !x.IsDeleted);
                if (appUser == null)
                {
                    return BadRequest("Tài khoản không tồn tại");
                }
                if (!appUser.IsActive)
                {
                    return BadRequest("Tài khoản đã bị khóa hoặc chưa được kích hoạt");
                }
                var checklogin = Commons.VerifyPassword(model.Password, appUser.PasswordHash);
                if (checklogin == false)
                {
                    return BadRequest("Thông tin đăng nhập không đúng");
                }
                else
                {
                    var role = await userManager.GetRolesAsync(appUser);
                    var token = GenToken(appUser.Id.ToString(),isMobile);
                    return Ok(new
                    {
                        token.Token,
                        token.Expires,
                        Id = appUser.Id.ToString(),
                        appUser.Email,
                        appUser.FullName,
                        AccessRole = JsonSerializer.Serialize(role),
                        appUser.HinhAnhUrl
                    });
                }
            }
        }
        public class GetToken
        {
            public string Token { get; set; }
            public DateTime? Expires { get; set; }
        }
        private GetToken GenToken(string Id, bool isMobile)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            double time = 1;
            if (isMobile)
            {
                time = 7;
            }
            var Expires = DateTime.Now.AddDays(time);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Id) }),
                Expires = DateTime.Now.AddDays(time),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return new GetToken()
            {
                Token = tokenHandler.WriteToken(token),
                Expires = Expires,
            };
        }
        [Authorize]
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await userManager.FindByIdAsync(User.Identity.Name);
            var role = await userManager.GetRolesAsync(user);
            var token = GenToken(User.Identity.Name, false);
            return Ok(new
            {
                token.Token,
                token.Expires,
                user.Id,
                user.Email,
                user.FullName,
                AccessRole = JsonSerializer.Serialize(role),
                user.HinhAnhUrl
            });
        }
        private static bool CheckLoginAsync(string email, string password)
        {
            try
            {
                ExchangeService service = new(ExchangeVersion.Exchange2013)
                {
                    Credentials = new WebCredentials(email, password),
                    Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx")
                };
                // Kiểm tra kết nối bằng cách thử lấy danh sách thư mục
                Folder rootFolder = Folder.Bind(service, WellKnownFolderName.MsgFolderRoot).Result;
                if (rootFolder != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}