using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ERP.Helpers;
using ERP.Infrastructure;
using ERP.Models;
using OfficeOpenXml;
using ThacoLibs;
using static ERP.Commons;
using static ERP.Data.MyDbContext;
using System.Dynamic;
using static ERP.SendEmailLibs;
using System.Globalization;
using ERP.Models.Default;

namespace ERP.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DbAdapter dbAdapter;
        private readonly MyTypedClient client;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SendEmailLibs sendEmail;
        public AccountController(IConfiguration _configuration, RoleManager<ApplicationRole> _roleManager, IUnitofWork _uow, UserManager<ApplicationUser> _userManager, MyTypedClient _client, SendEmailLibs _sendEmail)
        {
            uow = _uow;
            userManager = _userManager;
            client = _client;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            sendEmail = _sendEmail;
            roleManager = _roleManager;
        }
        private static string SecurityStampRandom()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }
        [HttpPut("hinh-anh/{id}")]
        public async Task<ActionResult> PutHinhAnh(string id)
        {
            try
            {
                var appUser = await userManager.FindByIdAsync(id);
                appUser.HinhAnhUrl = await client.AnhNhanVien(appUser.MaNhanVien);
                var result = await userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, $"{appUser.FullName} đã cập nhật hình ảnh thành công");
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Không có hình ảnh hoặc lỗi đã xảy ra!");
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status409Conflict, "Lỗi đã xảy ra!");
            }
        }
        [HttpPut("chu-ky-so")]
        public ActionResult PutChuKySo(string HinhAnhChuKySo, bool Isdelete = false)
        {
            var check = uow.ChuKySos.FirstOrDefault(x => x.User_Id == Guid.Parse(User.Identity.Name));
            if (Isdelete == true)
            {
                uow.ChuKySos.Delete(check.Id);
                uow.Complete();
                return Ok("Xóa chữ ký số thành công");
            }
            else
            {
                if (check == null)
                {
                    string fileBase64 = FileToBase64(HinhAnhChuKySo);
                    if (string.IsNullOrEmpty(fileBase64))
                    {
                        return StatusCode(StatusCodes.Status409Conflict, "Chưa gửi hình ảnh chữ ký số!");
                    }
                    Guid nid = Guid.NewGuid();
                    ChuKySo cks = new()
                    {
                        Id = nid,
                        User_Id = Guid.Parse(User.Identity.Name),
                        HinhAnhChuKySo = fileBase64,
                        CreatedBy = Guid.Parse(User.Identity.Name),
                        CreatedDate = DateTime.Now
                    };
                    uow.ChuKySos.Add(cks);
                }
                else
                {
                    string fileBase64 = FileToBase64(HinhAnhChuKySo);
                    if (string.IsNullOrEmpty(fileBase64))
                    {
                        return StatusCode(StatusCodes.Status409Conflict, "Chưa gửi hình ảnh chữ ký số!");
                    }
                    check.HinhAnhChuKySo = fileBase64;
                    check.UpdatedDate = DateTime.Now;
                    check.UpdatedBy = Guid.Parse(User.Identity.Name);
                    uow.ChuKySos.Update(check);
                }
                uow.Complete();
                return Ok("Cập nhật chữ ký số thành công");
            }
        }
        private static string XuLyDuongDanFile(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            return url.TrimStart('/', '\\');
        }

        private static string FileToBase64(string filePath)
        {
            try
            {
                filePath = XuLyDuongDanFile(filePath);
                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string base64String = EncryptPassword(Convert.ToBase64String(fileBytes));
                System.IO.File.Delete(filePath);
                return base64String;
            }
            catch
            {
                return null;
            }
        }
        [HttpGet("chu-ky-so")]
        public ActionResult GetChuKySo()
        {
            var check = uow.ChuKySos.FirstOrDefault(x => x.User_Id == Guid.Parse(User.Identity.Name));
            if (check == null)
            {
                return Ok();
            }
            return Ok(new
            {
                HinhAnhChuKySo = DecryptPassword(check.HinhAnhChuKySo),
                check.User_Id
            });
        }
        [HttpGet("list-chuc-danh")]
        public ActionResult GetListChucDanh()
        {
            var data = uow.ChucDanhs.GetAll(x => true).OrderBy(x => x.TenChucDanh);
            return Ok(data);
        }
        [HttpGet("list-chuc-vu")]
        public ActionResult GetListChucVu()
        {
            var data = uow.chucVus.GetAll(x => true).OrderBy(x => x.TenChucVu);
            return Ok(data);
        }
        [HttpGet("list-thanh-phan")]
        public ActionResult GetListThanhPhan()
        {
            var data = uow.ThanhPhans.GetAll(x => true).OrderBy(x => x.TenThanhPhan);
            return Ok(data);
        }
        [HttpGet("list-don-vi-tra-luong")]
        public ActionResult GetListDonViTraLuong()
        {
            var data = uow.DonViTraLuongs.GetAll(x => true).OrderBy(x => x.TenDonViTraLuong);
            return Ok(data);
        }
        [HttpGet("list-cap-do-nhan-su")]
        public ActionResult GetListCapDoNhanSu()
        {
            var data = uow.CapDoNhanSus.GetAll(x => true).OrderBy(x => x.TenCapDoNhanSu);
            return Ok(data);
        }
        [HttpGet("list-ma-phong-ban")]
        public ActionResult GetListMaPhongBan(Guid? donVi_Id)
        {
            var data = uow.PhongBanHRMs.GetAll(x => donVi_Id == null || x.DonVi_Id == donVi_Id, null, new string[] { "DonVi" }).Select(x => new
            {
                x.MaPhongBanHRM,
                x.Id,
                x.DonVi_Id,
                x.DonVi.TenDonVi,
                x.TenPhongBan
            }).OrderBy(x => x.MaPhongBanHRM);
            return Ok(data);
        }
        public class RegisterModel
        {
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            [StringLength(10)]
            [Required]
            [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Mã nhân viên chỉ được chứa số và có độ dài không quá 10 chữ số.")]
            public string MaNhanVien { get; set; }
            [StringLength(100)]
            public string FullName { get; set; }
            public Guid? ChucDanh_Id { get; set; }
            public Guid? ChucVu_Id { get; set; }
            public Guid? ThanhPhan_Id { get; set; }
            public Guid DonVi_Id { get; set; }
            public Guid? DonViTraLuong_Id { get; set; }
            public Guid? CapDoNhanSu_Id { get; set; }
            [StringLength(30)]
            [Required]
            public string MaPhongBanHRM { get; set; }
            [StringLength(40)]
            public string TrinhDoChuyenMon { get; set; }
            [StringLength(150)]
            public string Truong { get; set; }
            [StringLength(100)]
            public string ChuyenNganh { get; set; }
            [StringLength(100)]
            public string EmailThongBao { get; set; }
            [Required]
            public string NgaySinh { get; set; }
            [Required]
            public string NgayVaoLam { get; set; }
            [StringLength(250)]
            public string GhiChu { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult> Post(RegisterModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            data.MaNhanVien = data.MaNhanVien.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "");
            var maPhongBan = uow.PhongBanHRMs.FirstOrDefault(x => x.MaPhongBanHRM == data.MaPhongBanHRM);
            if (maPhongBan == null) return StatusCode(StatusCodes.Status409Conflict, "Mã phòng ban không tồn tại.");
            data.DonVi_Id = maPhongBan.DonVi_Id;
            if (!DateTime.TryParseExact(data.NgaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgaySinh))
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Ngày sinh không đúng định dạng!");
            }
            if (!DateTime.TryParseExact(data.NgayVaoLam, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgayVaoLam))
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Ngày vào làm không đúng định dạng!");
            }
            if (Regex.IsMatch(data.MaNhanVien, @"^\d{1,10}$"))
            {
                if (data.MaNhanVien.Length > 10)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên quá dài!");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên chỉ chứa kí tự số!");
            }
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Post_AspNetUser");
                dbAdapter.sqlCommand.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = data.FullName;
                dbAdapter.sqlCommand.Parameters.Add("@MaNhanVien", SqlDbType.NVarChar).Value = data.MaNhanVien;
                dbAdapter.sqlCommand.Parameters.Add("@HinhAnhUrl", SqlDbType.NVarChar).Value = await client.AnhNhanVien(data.MaNhanVien);
                dbAdapter.sqlCommand.Parameters.Add("@DonViTraLuong_Id", SqlDbType.UniqueIdentifier).Value = data.DonViTraLuong_Id;
                dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = data.GhiChu;
                dbAdapter.sqlCommand.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = data.MaNhanVien;
                dbAdapter.sqlCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = Commons.HashPassword(data.MaNhanVien);
                dbAdapter.sqlCommand.Parameters.Add("@SecurityStamp", SqlDbType.NVarChar).Value = SecurityStampRandom();
                dbAdapter.sqlCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = data.PhoneNumber;
                dbAdapter.sqlCommand.Parameters.Add("@ChucDanh_Id", SqlDbType.UniqueIdentifier).Value = data.ChucDanh_Id;
                dbAdapter.sqlCommand.Parameters.Add("@ChucVu_Id", SqlDbType.UniqueIdentifier).Value = data.ChucVu_Id;
                dbAdapter.sqlCommand.Parameters.Add("@ChuyenNganh", SqlDbType.NVarChar).Value = data.ChuyenNganh;
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = data.DonVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@EmailThongBao", SqlDbType.NVarChar).Value = data.EmailThongBao;
                dbAdapter.sqlCommand.Parameters.Add("@MaPhongBanHRM", SqlDbType.NVarChar).Value = data.MaPhongBanHRM;
                dbAdapter.sqlCommand.Parameters.Add("@NgaySinh", SqlDbType.DateTime2).Value = NgaySinh;
                dbAdapter.sqlCommand.Parameters.Add("@NgayVaoLam", SqlDbType.DateTime2).Value = NgayVaoLam;
                dbAdapter.sqlCommand.Parameters.Add("@ThanhPhan_Id", SqlDbType.UniqueIdentifier).Value = data.ThanhPhan_Id;
                dbAdapter.sqlCommand.Parameters.Add("@TrinhDoChuyenMon", SqlDbType.NVarChar).Value = data.TrinhDoChuyenMon;
                dbAdapter.sqlCommand.Parameters.Add("@Truong", SqlDbType.NVarChar).Value = data.Truong;
                dbAdapter.sqlCommand.Parameters.Add("@CapDoNhanSu_Id", SqlDbType.UniqueIdentifier).Value = data.CapDoNhanSu_Id;
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result == 0) return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên đã tồn tại.");
                return Ok("Thêm CBNV thành công!");
            }
            else
            {
                var exit = userManager.Users.FirstOrDefault(x => !x.IsDeleted && x.Email == data.Email);
                if (exit == null)
                {
                    dbAdapter.connect();
                    dbAdapter.createStoredProceder("sp_Post_AspNetUser");
                    dbAdapter.sqlCommand.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = data.FullName;
                    dbAdapter.sqlCommand.Parameters.Add("@MaNhanVien", SqlDbType.NVarChar).Value = data.MaNhanVien;
                    dbAdapter.sqlCommand.Parameters.Add("@HinhAnhUrl", SqlDbType.NVarChar).Value = await client.AnhNhanVien(data.MaNhanVien);
                    dbAdapter.sqlCommand.Parameters.Add("@DonViTraLuong_Id", SqlDbType.UniqueIdentifier).Value = data.DonViTraLuong_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = data.GhiChu;
                    dbAdapter.sqlCommand.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = data.MaNhanVien;
                    dbAdapter.sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = data.Email.ToLower();
                    dbAdapter.sqlCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = Commons.HashPassword(data.MaNhanVien);
                    dbAdapter.sqlCommand.Parameters.Add("@SecurityStamp", SqlDbType.NVarChar).Value = SecurityStampRandom();
                    dbAdapter.sqlCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = data.PhoneNumber;
                    dbAdapter.sqlCommand.Parameters.Add("@ChucDanh_Id", SqlDbType.UniqueIdentifier).Value = data.ChucDanh_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@ChucVu_Id", SqlDbType.UniqueIdentifier).Value = data.ChucVu_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@ChuyenNganh", SqlDbType.NVarChar).Value = data.ChuyenNganh;
                    dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = data.DonVi_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@EmailThongBao", SqlDbType.NVarChar).Value = data.EmailThongBao;
                    dbAdapter.sqlCommand.Parameters.Add("@MaPhongBanHRM", SqlDbType.NVarChar).Value = data.MaPhongBanHRM;
                    dbAdapter.sqlCommand.Parameters.Add("@NgaySinh", SqlDbType.DateTime2).Value = NgaySinh;
                    dbAdapter.sqlCommand.Parameters.Add("@NgayVaoLam", SqlDbType.DateTime2).Value = NgayVaoLam;
                    dbAdapter.sqlCommand.Parameters.Add("@ThanhPhan_Id", SqlDbType.UniqueIdentifier).Value = data.ThanhPhan_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@TrinhDoChuyenMon", SqlDbType.NVarChar).Value = data.TrinhDoChuyenMon;
                    dbAdapter.sqlCommand.Parameters.Add("@Truong", SqlDbType.NVarChar).Value = data.Truong;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoNhanSu_Id", SqlDbType.UniqueIdentifier).Value = data.CapDoNhanSu_Id;
                    var result = dbAdapter.runStoredNoneQuery();
                    dbAdapter.deConnect();
                    if (result == 0) return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên đã tồn tại.");
                    return Ok("Thêm CBNV thành công!");
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Thông tin email tài khoản đã tồn tại");
                }
            }
        }
        public class UserInfoModel
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            [StringLength(10)]
            [Required]
            [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Mã nhân viên chỉ được chứa số và có độ dài không quá 10 chữ số.")]
            public string MaNhanVien { get; set; }
            [StringLength(100)]
            public string FullName { get; set; }
            public Guid? ChucDanh_Id { get; set; }
            public Guid? ChucVu_Id { get; set; }
            public Guid? ThanhPhan_Id { get; set; }
            public Guid DonVi_Id { get; set; }
            public Guid? DonViTraLuong_Id { get; set; }
            public Guid? CapDoNhanSu_Id { get; set; }
            [StringLength(30)]
            [Required]
            public string MaPhongBanHRM { get; set; }
            [StringLength(40)]
            public string TrinhDoChuyenMon { get; set; }
            [StringLength(150)]
            public string Truong { get; set; }
            [StringLength(100)]
            public string ChuyenNganh { get; set; }
            [StringLength(100)]
            public string EmailThongBao { get; set; }
            [Required]
            public string NgaySinh { get; set; }
            [Required]
            public string NgayVaoLam { get; set; }
            [StringLength(250)]
            public string GhiChu { get; set; }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, UserInfoModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != data.Id)
            {
                return BadRequest();
            }
            data.MaNhanVien = data.MaNhanVien.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "");
            var maPhongBan = uow.PhongBanHRMs.FirstOrDefault(x => x.MaPhongBanHRM == data.MaPhongBanHRM);
            if (maPhongBan == null) return StatusCode(StatusCodes.Status409Conflict, "Mã phòng ban không tồn tại.");
            data.DonVi_Id = maPhongBan.DonVi_Id;
            if (!DateTime.TryParseExact(data.NgaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgaySinh))
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Ngày sinh không đúng định dạng!");
            }
            if (!DateTime.TryParseExact(data.NgayVaoLam, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgayVaoLam))
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Ngày vào làm không đúng định dạng!");
            }
            if (Regex.IsMatch(data.MaNhanVien, @"^\d{1,10}$"))
            {
                if (data.MaNhanVien.Length > 10)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên quá dài!");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên chỉ chứa kí tự số!");
            }
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Put_AspNetUser");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                dbAdapter.sqlCommand.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = data.FullName;
                dbAdapter.sqlCommand.Parameters.Add("@MaNhanVien", SqlDbType.NVarChar).Value = data.MaNhanVien;
                dbAdapter.sqlCommand.Parameters.Add("@HinhAnhUrl", SqlDbType.NVarChar).Value = await client.AnhNhanVien(data.MaNhanVien);
                dbAdapter.sqlCommand.Parameters.Add("@DonViTraLuong_Id", SqlDbType.UniqueIdentifier).Value = data.DonViTraLuong_Id;
                dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = data.GhiChu;
                dbAdapter.sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = data.Email;
                dbAdapter.sqlCommand.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = data.MaNhanVien;
                dbAdapter.sqlCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = data.PhoneNumber;
                dbAdapter.sqlCommand.Parameters.Add("@ChucDanh_Id", SqlDbType.UniqueIdentifier).Value = data.ChucDanh_Id;
                dbAdapter.sqlCommand.Parameters.Add("@ChucVu_Id", SqlDbType.UniqueIdentifier).Value = data.ChucVu_Id;
                dbAdapter.sqlCommand.Parameters.Add("@ChuyenNganh", SqlDbType.NVarChar).Value = data.ChuyenNganh;
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = data.DonVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@EmailThongBao", SqlDbType.NVarChar).Value = data.EmailThongBao;
                dbAdapter.sqlCommand.Parameters.Add("@MaPhongBanHRM", SqlDbType.NVarChar).Value = data.MaPhongBanHRM;
                dbAdapter.sqlCommand.Parameters.Add("@NgaySinh", SqlDbType.DateTime2).Value = NgaySinh;
                dbAdapter.sqlCommand.Parameters.Add("@NgayVaoLam", SqlDbType.DateTime2).Value = NgayVaoLam;
                dbAdapter.sqlCommand.Parameters.Add("@ThanhPhan_Id", SqlDbType.UniqueIdentifier).Value = data.ThanhPhan_Id;
                dbAdapter.sqlCommand.Parameters.Add("@TrinhDoChuyenMon", SqlDbType.NVarChar).Value = data.TrinhDoChuyenMon;
                dbAdapter.sqlCommand.Parameters.Add("@Truong", SqlDbType.NVarChar).Value = data.Truong;
                dbAdapter.sqlCommand.Parameters.Add("@CapDoNhanSu_Id", SqlDbType.UniqueIdentifier).Value = data.CapDoNhanSu_Id;
                var result = dbAdapter.runStoredNoneQuery();
                dbAdapter.deConnect();
                if (result == 0) return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên đã tồn tại.");
                return Ok("Sửa CBNV thành công!");
            }
            else
            {
                var exit = userManager.Users.FirstOrDefault(x => !x.IsDeleted && x.Email == data.Email && x.Id != id);
                if (exit == null)
                {
                    dbAdapter.connect();
                    dbAdapter.createStoredProceder("sp_Put_AspNetUser");
                    dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                    dbAdapter.sqlCommand.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = data.FullName;
                    dbAdapter.sqlCommand.Parameters.Add("@MaNhanVien", SqlDbType.NVarChar).Value = data.MaNhanVien;
                    dbAdapter.sqlCommand.Parameters.Add("@HinhAnhUrl", SqlDbType.NVarChar).Value = await client.AnhNhanVien(data.MaNhanVien);
                    dbAdapter.sqlCommand.Parameters.Add("@DonViTraLuong_Id", SqlDbType.UniqueIdentifier).Value = data.DonViTraLuong_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = data.GhiChu;
                    dbAdapter.sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = data.Email.ToLower();
                    dbAdapter.sqlCommand.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = data.MaNhanVien;
                    dbAdapter.sqlCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = data.PhoneNumber;
                    dbAdapter.sqlCommand.Parameters.Add("@ChucDanh_Id", SqlDbType.UniqueIdentifier).Value = data.ChucDanh_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@ChucVu_Id", SqlDbType.UniqueIdentifier).Value = data.ChucVu_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@ChuyenNganh", SqlDbType.NVarChar).Value = data.ChuyenNganh;
                    dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = data.DonVi_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@EmailThongBao", SqlDbType.NVarChar).Value = data.EmailThongBao;
                    dbAdapter.sqlCommand.Parameters.Add("@MaPhongBanHRM", SqlDbType.NVarChar).Value = data.MaPhongBanHRM;
                    dbAdapter.sqlCommand.Parameters.Add("@NgaySinh", SqlDbType.DateTime2).Value = NgaySinh;
                    dbAdapter.sqlCommand.Parameters.Add("@NgayVaoLam", SqlDbType.DateTime2).Value = NgayVaoLam;
                    dbAdapter.sqlCommand.Parameters.Add("@ThanhPhan_Id", SqlDbType.UniqueIdentifier).Value = data.ThanhPhan_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@TrinhDoChuyenMon", SqlDbType.NVarChar).Value = data.TrinhDoChuyenMon;
                    dbAdapter.sqlCommand.Parameters.Add("@Truong", SqlDbType.NVarChar).Value = data.Truong;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoNhanSu_Id", SqlDbType.UniqueIdentifier).Value = data.CapDoNhanSu_Id;
                    var result = dbAdapter.runStoredNoneQuery();
                    dbAdapter.deConnect();
                    if (result == 0) return StatusCode(StatusCodes.Status409Conflict, "Mã nhân viên đã tồn tại.");
                    return Ok("Sửa CBNV thành công!");
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Thông tin email tài khoản đã tồn tại");
                }
            }
        }
        public class NghiViecModel
        {
            public Guid Id { get; set; }
            public string NgayNghiViec { get; set; }
            public string GhiChu { get; set; }
        }

        [HttpPut("nghi-viec/{id}")]
        public ActionResult PutNghiViec(Guid id, NghiViecModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (!DateTime.TryParseExact(model.NgayNghiViec, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgayNghiViec))
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Ngày nghỉ việc không đúng định dạng!");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_Put_CBNVNghiViec");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@NgayNghiViec", SqlDbType.DateTime2).Value = NgayNghiViec;
            dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = model.GhiChu;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            //Hủy đào tạo

            return Ok();
        }
        [HttpGet("{id}")]
        public ActionResult GetById(Guid id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_Get_CBNVById");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            var result = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpGet("list-cbnv-thuoc-don-vi-va-co-quyen")]
        public ActionResult GetListUserNonRole(Guid? donvi_Id, string tenPhongBan, string keyword)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNVThuocDonViVaCoQuyenDonVi");
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@TenPhongBan", SqlDbType.NVarChar).Value = tenPhongBan;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpGet("list-cbnv-khong-co-quyen-trong-phan-mem")]
        public ActionResult GetListUserNonRole(Guid? donvi_Id, Guid? phanMem_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNV_NonRole");
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_Id;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpGet("list-cbnv-chua-kich-hoat")]
        public ActionResult GetListUserNonActive(string keyword, Guid? donvi_Id)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;
            else keyword = keyword.Trim();
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNV_NonActive");
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpGet("list-cbnv-nghi-viec")]
        public ActionResult GetListUserNghiViec(string keyword, Guid? donvi_Id)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;
            else keyword = keyword.Trim();
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNV_NghiViec");
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpGet]
        public ActionResult GetListCBNV(string tenPhongBan, int page = 1, string keyword = null, Guid? donvi_Id = null)
        {
            var pageSizeData = uow.Configs.GetAll(x => !x.IsDeleted).FirstOrDefault();
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNV");
            dbAdapter.sqlCommand.Parameters.Add("@tenPhongBan", SqlDbType.NVarChar).Value = tenPhongBan;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            int totalRow = result.Count();
            int pageSize = pageSizeData.PageSize;
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPage)
            {
                page = totalPage;
            }
            var datalist = result.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                datalist
            });
        }
        [HttpGet("get-cbnv-add-role")]
        public ActionResult GetCBNVAddRole(string keyword = null)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;
            else keyword = keyword.Trim();
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNVAddRole");
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
        public class ClassCBNVActive
        {
            public Guid Id { get; set; }
            public Guid PhanMem_Id { get; set; }
            public Guid DonVi_Id { get; set; }
            public bool IsActive_Role { get; set; }
            public List<ClassRoleCBNV> ChiTietRoles { get; set; }
        }
        public class ClassRoleCBNV
        {
            public Guid Role_Id { get; set; }
        }
        [HttpPut("role-cbnv")]
        public ActionResult PutRoleCBNV(Guid id, ClassCBNVActive model)
        {
            lock (Commons.LockObjectState)
            {
                if (id != model.Id)
                {
                    return BadRequest();
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_DeleteRoleByUserPhanMemDonVi");
                dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = model.PhanMem_Id;
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = model.DonVi_Id;
                var result = dbAdapter.runStoredNoneQuery();
                foreach (var item in model.ChiTietRoles)
                {
                    dbAdapter.createStoredProceder("sp_Post_AspNetUserRoles");
                    dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@Role_Id", SqlDbType.UniqueIdentifier).Value = item.Role_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = model.PhanMem_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = model.DonVi_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@IsActive_Role", SqlDbType.Bit).Value = model.IsActive_Role;
                    result += dbAdapter.runStoredNoneQuery();
                }
                dbAdapter.deConnect();
                return Ok();
            }
        }
        [HttpPost("role-cbnv")]
        public ActionResult PostRoleCBNV(Guid id, ClassCBNVActive model)
        {
            lock (Commons.LockObjectState)
            {
                if (id != model.Id)
                {
                    return BadRequest();
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_DeleteRoleByUserPhanMemDonVi");
                dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = model.PhanMem_Id;
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = model.DonVi_Id;
                var result = dbAdapter.runStoredNoneQuery();
                foreach (var item in model.ChiTietRoles)
                {
                    dbAdapter.createStoredProceder("sp_Post_AspNetUserRoles");
                    dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@Role_Id", SqlDbType.UniqueIdentifier).Value = item.Role_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = model.PhanMem_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = model.DonVi_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@IsActive_Role", SqlDbType.Bit).Value = model.IsActive_Role;
                    result += dbAdapter.runStoredNoneQuery();
                }
                dbAdapter.deConnect();
                return Ok();
            }
        }
        [HttpDelete("role-cbnv")]
        public ActionResult DeleteUserCBNV(Guid id, Guid role_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_Delete_AspNetUserRoles");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = role_Id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok("Xóa vai trò thành công!");
        }
        [HttpDelete("delete-all-role-cbnv")]
        public ActionResult DeleteAllRoleUserCBNV(Guid id, Guid phanMem_Id, Guid donVi_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteAllRoleCBNV");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_Id;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok("Xóa vai trò thành công!");
        }
        [HttpPut("Active/{id}")]
        public async Task<ActionResult> Active(string id)
        {
            var appUser = await userManager.FindByIdAsync(id);
            appUser.IsActive = !appUser.IsActive;
            appUser.UpdatedDate = DateTime.Now;
            var result = await userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                if (appUser.IsActive)
                {
                    return StatusCode(StatusCodes.Status200OK, "Mở khóa tài khoản thành công");
                }
                return StatusCode(StatusCodes.Status200OK, "Khóa tài khoản thành công");
            }
            return BadRequest(string.Join(",", result.Errors));
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteCBNV");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok(result);
        }
        private static string RandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@$#*!%&";
            string serialNumber = new(Enumerable.Repeat(chars, 8)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
            return serialNumber;
        }
        [HttpPut("ResetPassword/{id}")]
        public async Task<ActionResult> ResetPassword(string id)
        {
            var appUser = await userManager.FindByIdAsync(id);
            var email = uow.EmailPhongCongNgheThongTins.GetById(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE"));
            if (email != null && (!string.IsNullOrEmpty(appUser.Email) || !string.IsNullOrEmpty(appUser.EmailThongBao)))
            {
                string password = RandomPassword();
                var passwordHash = HashPassword(password);
                appUser.UpdatedDate = DateTime.Now;
                appUser.PasswordHash = passwordHash;
                var result = await userManager.UpdateAsync(appUser);
                List<string> emails = new();
                if (!string.IsNullOrEmpty(appUser.Email)) emails.Add(appUser.Email);
                if (!string.IsNullOrEmpty(appUser.EmailThongBao)) emails.Add(appUser.EmailThongBao);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByIdAsync(User.Identity.Name);
                    try
                    {
                        sendEmail.SendEmailCongNgheThongTin(
                            email.Email,
                            email.PasswordHash,
                            "Khôi phục mật khẩu đăng nhập thành công",
                            $"Bạn vừa được Admin {user.FullName} thực hiện khôi phục mật khẩu đăng nhập phần mềm ERP thành công." +
                            $"<br>Mật khẩu mới tài khoản cá nhân <b>{appUser.MaNhanVien}</b> của bạn là:" +
                            $"<br><div style=\"background-color:#ebebeb;color:#333;font-size:40px;letter-spacing:8px;padding:16px;text-align:center; border-radius:5px;\">{password}</div>",
                            emails,
                            true
                        );
                    }
                    catch
                    {

                    }
                    return StatusCode(StatusCodes.Status200OK, "Khôi phục thành công! Mật khẩu đăng nhập mới đã được gửi về email người dùng!");
                }
                return BadRequest(string.Join(",", result.Errors));
            }
            else
            {
                var Password = HashPassword(appUser.MaNhanVien);
                appUser.UpdatedDate = DateTime.Now;
                appUser.PasswordHash = Password;
                var result = await userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, "Khôi phục mật khẩu mặc định thành công");
                }
                return BadRequest(string.Join(",", result.Errors));
            }
        }
        [HttpPut("reset-password-erp/{id}")]
        public async Task<ActionResult> ResetPasswordERP(string id)
        {
            var user = await userManager.FindByIdAsync(User.Identity.Name);
            var hasRole = await userManager.IsInRoleAsync(user, "Administrator");
            if (!hasRole) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            var appUser = await userManager.FindByIdAsync(id);
            var PasswordHash = HashPassword(appUser.MaNhanVien);
            appUser.UpdatedDate = DateTime.Now;
            appUser.PasswordHash = PasswordHash;
            var result = await userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK, "Khôi phục mật khẩu mặc định thành công");
            }
            return BadRequest(string.Join(",", result.Errors));
        }
        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var appUser = await userManager.FindByIdAsync(User.Identity.Name);
            appUser.UpdatedDate = DateTime.Now;
            var checklogin = VerifyPassword(model.Password, appUser.PasswordHash);
            if (checklogin)
            {
                if (model.NewPassword == model.ConfirmNewPassword)
                {
                    appUser.PasswordHash = HashPassword(model.NewPassword);
                    var result = await userManager.UpdateAsync(appUser);
                    if (result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status200OK, "Đổi mật khẩu thành công");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status409Conflict, "Lỗi không xác định");
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Xác nhận mật khẩu mới không trùng khớp");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict, "Mật khẩu hiện tại không đúng");
            }
        }
        [HttpGet("get-user-role")]
        public IActionResult GetUserRole(Guid id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetRoleNguoiDungById");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = id;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            return Ok(result);
        }
        public class DropdownTreeNode
        {
            public Guid Id { get; set; }
            public string NameId { get; set; }
            public string Name { get; set; }
            public int Level { get; set; }
            public List<DropdownTreeNode> Children { get; set; }
            public bool? Disable { get; set; }
        }
        public class ClassCBNVImport
        {
            private string _email;
            public string Email
            {
                get { return _email; }
                set { _email = CleanUp(value); }
            }
            private string _phoneNumber;
            public string PhoneNumber
            {
                get { return _phoneNumber; }
                set { _phoneNumber = CleanUp(value); }
            }
            private string _maNhanVien;
            [StringLength(10)]
            [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Mã nhân viên chỉ được chứa số và có độ dài không quá 10 chữ số.")]
            [Required]
            public string MaNhanVien
            {
                get { return _maNhanVien; }
                set { _maNhanVien = CleanUp(value); }
            }
            private string _fullName;
            [StringLength(100)]
            [Required]
            public string FullName
            {
                get { return _fullName; }
                set { _fullName = CleanUp(value); }
            }
            private string _tenChucDanh;
            [Required]
            public string TenChucDanh
            {
                get { return _tenChucDanh; }
                set { _tenChucDanh = CleanUp(value); }
            }
            private string _tenChucVu;
            [Required]
            public string TenChucVu
            {
                get { return _tenChucVu; }
                set { _tenChucVu = CleanUp(value); }
            }
            private string _tenThanhPhan;
            [Required]
            public string TenThanhPhan
            {
                get { return _tenThanhPhan; }
                set { _tenThanhPhan = CleanUp(value); }
            }
            private string _maPhongBanHRM;
            [StringLength(30)]
            [Required]
            public string MaPhongBanHRM
            {
                get { return _maPhongBanHRM; }
                set { _maPhongBanHRM = CleanUp(value); }
            }
            private string _tenDonViTraLuong;
            public string TenDonViTraLuong
            {
                get { return _tenDonViTraLuong; }
                set { _tenDonViTraLuong = CleanUp(value); }
            }
            private string _tenCapDoNhanSu;
            public string TenCapDoNhanSu
            {
                get { return _tenCapDoNhanSu; }
                set { _tenCapDoNhanSu = CleanUp(value); }
            }
            private string _trinhDoChuyenMon;
            [StringLength(40)]
            public string TrinhDoChuyenMon
            {
                get { return _trinhDoChuyenMon; }
                set { _trinhDoChuyenMon = CleanUp(value); }
            }
            private string _truong;
            [StringLength(150)]
            public string Truong
            {
                get { return _truong; }
                set { _truong = CleanUp(value); }
            }
            private string _chuyenNganh;
            [StringLength(100)]
            public string ChuyenNganh
            {
                get { return _chuyenNganh; }
                set { _chuyenNganh = CleanUp(value); }
            }
            private string _emailThongBao;
            [StringLength(100)]
            public string EmailThongBao
            {
                get { return _emailThongBao; }
                set { _emailThongBao = CleanUp(value); }
            }
            private string _ngaySinh;
            [Required]
            public string NgaySinh
            {
                get { return _ngaySinh; }
                set { _ngaySinh = CleanUp(value); }
            }
            private string _ngayVaoLam;
            [Required]
            public string NgayVaoLam
            {
                get { return _ngayVaoLam; }
                set { _ngayVaoLam = CleanUp(value); }
            }
            private string _ghiChu;
            [StringLength(250)]
            public string GhiChu
            {
                get { return _ghiChu; }
                set { _ghiChu = CleanUp(value); }
            }
            public string GhiChuImport { get; set; }
            public static string CleanUp(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return null;
                return input.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "");
            }
        }
        public class RegisterImport
        {
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string MaNhanVien { get; set; }
            public string FullName { get; set; }
            public Guid ChucDanh_Id { get; set; }
            public Guid ChucVu_Id { get; set; }
            public Guid ThanhPhan_Id { get; set; }
            public Guid DonVi_Id { get; set; }
            public Guid? DonViTraLuong_Id { get; set; }
            public Guid? CapDoNhanSu_Id { get; set; }
            public string MaPhongBanHRM { get; set; }
            public string TrinhDoChuyenMon { get; set; }
            public string Truong { get; set; }
            public string ChuyenNganh { get; set; }
            public string EmailThongBao { get; set; }
            public DateTime NgaySinh { get; set; }
            public DateTime NgayVaoLam { get; set; }
            public string GhiChu { get; set; }
            public bool IsNew { get; set; } = false;
        }
        [HttpPost("ImportExel")]
        public IActionResult ImportExel(List<ClassCBNVImport> data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (data == null || !data.Any()) return StatusCode(StatusCodes.Status409Conflict, "Chưa gửi dữ liệu!");
            List<RegisterImport> users = new();
            List<ClassCBNVImport> errors = new();
            var trungs = data
                    .GroupBy(x => new { x.MaNhanVien })
                    .Select(x => new { x.Key.MaNhanVien, SoLuong = x.Count() })
                    .Where(x => x.SoLuong > 1)
                    .Select(x => x.MaNhanVien);
            if (trungs.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, data.Where(x => trungs.Contains(x.MaNhanVien)).Select(x =>
                {
                    x.GhiChuImport = "Trùng mã CBNV";
                    return x;
                }));
            }
            var trung2s = data.Where(x => !string.IsNullOrEmpty(x.Email))
                    .GroupBy(x => new { Email = x.Email.ToLower() })
                    .Select(x => new { x.Key.Email, SoLuong = x.Count() })
                    .Where(x => x.SoLuong > 1)
                    .Select(x => x.Email);
            if (trung2s.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, data.Where(x => !string.IsNullOrEmpty(x.Email) && trung2s.Contains(x.Email.ToLower())).Select(x =>
                {
                    x.GhiChuImport = "Trùng email";
                    return x;
                }));
            }
            DateTime today = DateTime.Now.Date;
            var Users = userManager.Users.Where(x => true);
            var DonVis = uow.PhongBanHRMs.GetAll(x => true).ToDictionary(x => x.MaPhongBanHRM, x => x.DonVi_Id);
            var ChucVus = uow.chucVus.GetAll(x => true).ToDictionary(x => x.TenChucVu.ToUpper(), x => x.Id);
            var ChucDanhs = uow.ChucDanhs.GetAll(x => true).ToDictionary(x => x.TenChucDanh.ToUpper(), x => x.Id);
            var ThanhPhans = uow.ThanhPhans.GetAll(x => true).ToDictionary(x => x.TenThanhPhan.ToUpper(), x => x.Id);
            var DonViTraLuongs = uow.DonViTraLuongs.GetAll(x => true).ToDictionary(x => x.TenDonViTraLuong.ToUpper(), x => x.Id);
            var CapDoNhanSus = uow.CapDoNhanSus.GetAll(x => true).ToDictionary(x => x.TenCapDoNhanSu.ToUpper(), x => x.Id);
            var ChucVuGroups = data
                .Where(x => !string.IsNullOrEmpty(x.TenChucVu))
                .GroupBy(x => new { TenChucVu = x.TenChucVu.ToUpper() })
                .Select(x => x.Key.TenChucVu)
                .Where(x => !ChucVus.ContainsKey(x));
            List<DanhMucSave> ChucVuSaves = new();
            if (ChucVuGroups.Any())
            {
                foreach (var group in ChucVuGroups)
                {
                    Guid guid = Guid.NewGuid();
                    ChucVus.Add(group, guid);
                    ChucVuSaves.Add(new()
                    {
                        Id = guid,
                        Name = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.TenChucVu) && x.TenChucVu.ToUpper() == group).TenChucVu,
                    });
                }
            }
            var ChucDanhGroups = data
                .Where(x => !string.IsNullOrEmpty(x.TenChucDanh))
                .GroupBy(x => new { TenChucDanh = x.TenChucDanh.ToUpper() })
                .Select(x => x.Key.TenChucDanh)
                .Where(x => !ChucDanhs.ContainsKey(x));
            List<DanhMucSave> ChucDanhSaves = new();
            if (ChucDanhGroups.Any())
            {
                foreach (var group in ChucDanhGroups)
                {
                    Guid guid = Guid.NewGuid();
                    ChucDanhs.Add(group, guid);
                    ChucDanhSaves.Add(new()
                    {
                        Id = guid,
                        Name = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.TenChucDanh) && x.TenChucDanh.ToUpper() == group).TenChucDanh,
                    });
                }
            }
            var ThanhPhanGroups = data
                .Where(x => !string.IsNullOrEmpty(x.TenThanhPhan))
                .GroupBy(x => new { TenThanhPhan = x.TenThanhPhan.ToUpper() })
                .Select(x => x.Key.TenThanhPhan)
                .Where(x => !ThanhPhans.ContainsKey(x));
            List<DanhMucSave> ThanhPhanSaves = new();
            if (ThanhPhanGroups.Any())
            {
                foreach (var group in ThanhPhanGroups)
                {
                    Guid guid = Guid.NewGuid();
                    ThanhPhans.Add(group, guid);
                    ThanhPhanSaves.Add(new()
                    {
                        Id = guid,
                        Name = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.TenThanhPhan) && x.TenThanhPhan.ToUpper() == group).TenThanhPhan,
                    });
                }
            }
            var DonViTraLuongGroups = data
                .Where(x => !string.IsNullOrEmpty(x.TenDonViTraLuong))
                .GroupBy(x => new { TenDonViTraLuong = x.TenDonViTraLuong.ToUpper() })
                .Select(x => x.Key.TenDonViTraLuong)
                .Where(x => !DonViTraLuongs.ContainsKey(x));
            List<DanhMucSave> DonViTraLuongSaves = new();
            if (DonViTraLuongGroups.Any())
            {
                foreach (var group in DonViTraLuongGroups)
                {
                    Guid guid = Guid.NewGuid();
                    DonViTraLuongs.Add(group, guid);
                    DonViTraLuongSaves.Add(new()
                    {
                        Id = guid,
                        Name = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.TenDonViTraLuong) && x.TenDonViTraLuong.ToUpper() == group).TenDonViTraLuong,
                    });
                }
            }
            var CapDoNhanSuGroups = data
                .Where(x => !string.IsNullOrEmpty(x.TenCapDoNhanSu))
                .GroupBy(x => new { TenCapDoNhanSu = x.TenCapDoNhanSu.ToUpper() })
                .Select(x => x.Key.TenCapDoNhanSu)
                .Where(x => !CapDoNhanSus.ContainsKey(x));
            List<DanhMucSave> CapDoNhanSuSaves = new();
            if (CapDoNhanSuGroups.Any())
            {
                foreach (var group in CapDoNhanSuGroups)
                {
                    Guid guid = Guid.NewGuid();
                    CapDoNhanSus.Add(group, guid);
                    CapDoNhanSuSaves.Add(new()
                    {
                        Id = guid,
                        Name = data.FirstOrDefault(x => !string.IsNullOrEmpty(x.TenCapDoNhanSu) && x.TenCapDoNhanSu.ToUpper() == group).TenCapDoNhanSu,
                    });
                }
            }
            string testMaNhanhVien = @"^[0-9]+$", testEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            foreach (var item in data)
            {
                RegisterImport user = new();
                if (string.IsNullOrEmpty(item.MaNhanVien))
                {
                    item.GhiChuImport = $"Mã nhân viên bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                user.MaNhanVien = item.MaNhanVien;
                if (!Regex.IsMatch(user.MaNhanVien, testMaNhanhVien))
                {
                    item.GhiChuImport = $"Mã nhân viên {user.MaNhanVien} chỉ được chứa các chữ số!";
                    errors.Add(item);
                    continue;
                }
                if (!string.IsNullOrEmpty(item.Email))
                {
                    user.Email = item.Email.ToLower();
                    if (!Regex.IsMatch(user.Email, testEmail))
                    {
                        item.GhiChuImport = $"Email {user.Email} sai định dạng!";
                        errors.Add(item);
                        continue;
                    }
                    var userKhac = Users.FirstOrDefault(x => !x.IsDeleted && x.Email == user.Email && x.MaNhanVien != user.MaNhanVien);
                    if (userKhac != null)
                    {
                        item.GhiChuImport = $"Email {user.Email} đã có tại nhân viên có mã {userKhac.MaNhanVien}!";
                        errors.Add(item);
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(item.EmailThongBao))
                {
                    user.EmailThongBao = item.EmailThongBao.ToLower();
                    if (!Regex.IsMatch(user.EmailThongBao, testEmail))
                    {
                        item.GhiChuImport = $"Email nhận thông báo {user.EmailThongBao} sai định dạng!";
                        errors.Add(item);
                        continue;
                    }
                }
                user.PhoneNumber = item.PhoneNumber;
                if (string.IsNullOrEmpty(item.FullName))
                {
                    item.GhiChuImport = $"Tên nhân viên bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                user.FullName = item.FullName;
                if (string.IsNullOrEmpty(item.TenChucDanh))
                {
                    item.GhiChuImport = $"Tên chức danh bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                user.ChucDanh_Id = ChucDanhs[item.TenChucDanh.ToUpper()];
                if (string.IsNullOrEmpty(item.TenChucVu))
                {
                    item.GhiChuImport = $"Tên chức vụ bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                user.ChucVu_Id = ChucVus[item.TenChucVu.ToUpper()];
                if (string.IsNullOrEmpty(item.TenThanhPhan))
                {
                    item.GhiChuImport = $"Tên thành phần bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                user.ThanhPhan_Id = ThanhPhans[item.TenThanhPhan.ToUpper()];
                if (!string.IsNullOrEmpty(item.TenDonViTraLuong))
                {
                    user.DonViTraLuong_Id = DonViTraLuongs[item.TenDonViTraLuong.ToUpper()];
                }
                if (!string.IsNullOrEmpty(item.TenCapDoNhanSu))
                {
                    user.CapDoNhanSu_Id = CapDoNhanSus[item.TenCapDoNhanSu.ToUpper()];
                }
                if (string.IsNullOrEmpty(item.MaPhongBanHRM))
                {
                    item.GhiChuImport = $"Mã phòng ban bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                user.MaPhongBanHRM = item.MaPhongBanHRM;
                if (DonVis.ContainsKey(user.MaPhongBanHRM))
                {
                    user.DonVi_Id = DonVis[item.MaPhongBanHRM];
                }
                else
                {
                    item.GhiChuImport = $"Mã phòng ban {user.MaPhongBanHRM} không tồn tại!";
                    errors.Add(item);
                    continue;
                }
                user.TrinhDoChuyenMon = item.TrinhDoChuyenMon;
                user.Truong = item.Truong;
                user.ChuyenNganh = item.ChuyenNganh;
                user.GhiChu = item.GhiChu;
                if (string.IsNullOrEmpty(item.NgayVaoLam))
                {
                    item.GhiChuImport = $"Ngày vào làm bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                else
                {
                    if (!DateTime.TryParseExact(item.NgayVaoLam, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgayVaoLam))
                    {
                        item.GhiChuImport = $"Ngày vào làm {item.NgayVaoLam} sai định dạng!";
                        errors.Add(item);
                        continue;
                    }
                    else user.NgayVaoLam = NgayVaoLam;
                }
                if (string.IsNullOrEmpty(item.NgaySinh))
                {
                    item.GhiChuImport = $"Ngày sinh bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                else
                {
                    if (!DateTime.TryParseExact(item.NgaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgaySinh))
                    {
                        item.GhiChuImport = $"Ngày sinh {item.NgaySinh} sai định dạng!";
                        errors.Add(item);
                        continue;
                    }
                    else user.NgaySinh = NgaySinh;
                    if (today < NgaySinh)
                    {
                        item.GhiChuImport = $"Ngày sinh phải nhỏ hơn ngày hiện tại!";
                        errors.Add(item);
                        continue;
                    }
                }
                var userCheck = Users.FirstOrDefault(x => x.MaNhanVien == user.MaNhanVien);
                if (userCheck == null)
                {
                    user.IsNew = true;
                }
                users.Add(user);
            }
            if (errors.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, errors);
            }
            dbAdapter.connect();
            foreach (var item in ChucVuSaves)
            {
                dbAdapter.createStoredProceder("sp_Post_ChucVu");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                dbAdapter.sqlCommand.Parameters.Add("@TenChucVu", SqlDbType.NVarChar).Value = item.Name;
                dbAdapter.runStoredNoneQuery();
            }
            foreach (var item in ChucDanhSaves)
            {
                dbAdapter.createStoredProceder("sp_Post_ChucDanh");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                dbAdapter.sqlCommand.Parameters.Add("@TenChucDanh", SqlDbType.NVarChar).Value = item.Name;
                dbAdapter.runStoredNoneQuery();
            }
            foreach (var item in ThanhPhanSaves)
            {
                dbAdapter.createStoredProceder("sp_Post_ThanhPhan");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                dbAdapter.sqlCommand.Parameters.Add("@TenThanhPhan", SqlDbType.NVarChar).Value = item.Name;
                dbAdapter.runStoredNoneQuery();
            }
            foreach (var item in DonViTraLuongSaves)
            {
                dbAdapter.createStoredProceder("sp_Post_DonViTraLuong");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                dbAdapter.sqlCommand.Parameters.Add("@TenDonViTraLuong", SqlDbType.NVarChar).Value = item.Name;
                dbAdapter.runStoredNoneQuery();
            }
            foreach (var item in CapDoNhanSuSaves)
            {
                dbAdapter.createStoredProceder("sp_Post_CapDoNhanSu");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                dbAdapter.sqlCommand.Parameters.Add("@TenCapDoNhanSu", SqlDbType.NVarChar).Value = item.Name;
                dbAdapter.sqlCommand.Parameters.Add("@MaCapDoNhanSu", SqlDbType.NVarChar).Value = item.Name;
                dbAdapter.runStoredNoneQuery();
            }
            int result = 0;
            foreach (var user in users)
            {
                dbAdapter.createStoredProceder("sp_Post_Put_AspNetUser");
                dbAdapter.sqlCommand.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = user.FullName;
                dbAdapter.sqlCommand.Parameters.Add("@MaNhanVien", SqlDbType.NVarChar).Value = user.MaNhanVien;
                //dbAdapter.sqlCommand.Parameters.Add("@HinhAnhUrl", SqlDbType.NVarChar).Value = user.HinhAnhUrl;
                dbAdapter.sqlCommand.Parameters.Add("@DonViTraLuong_Id", SqlDbType.UniqueIdentifier).Value = user.DonViTraLuong_Id;
                dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = user.GhiChu;
                dbAdapter.sqlCommand.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = user.MaNhanVien;
                dbAdapter.sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar).Value = user.Email;
                if (user.IsNew)
                {
                    dbAdapter.sqlCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = HashPassword(user.MaNhanVien);
                    dbAdapter.sqlCommand.Parameters.Add("@SecurityStamp", SqlDbType.NVarChar).Value = SecurityStampRandom();
                }
                dbAdapter.sqlCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = user.PhoneNumber;
                dbAdapter.sqlCommand.Parameters.Add("@ChucDanh_Id", SqlDbType.UniqueIdentifier).Value = user.ChucDanh_Id;
                dbAdapter.sqlCommand.Parameters.Add("@ChucVu_Id", SqlDbType.UniqueIdentifier).Value = user.ChucVu_Id;
                dbAdapter.sqlCommand.Parameters.Add("@ChuyenNganh", SqlDbType.NVarChar).Value = user.ChuyenNganh;
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = user.DonVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@EmailThongBao", SqlDbType.NVarChar).Value = user.EmailThongBao;
                dbAdapter.sqlCommand.Parameters.Add("@MaPhongBanHRM", SqlDbType.NVarChar).Value = user.MaPhongBanHRM;
                dbAdapter.sqlCommand.Parameters.Add("@NgaySinh", SqlDbType.DateTime2).Value = user.NgaySinh;
                dbAdapter.sqlCommand.Parameters.Add("@NgayVaoLam", SqlDbType.DateTime2).Value = user.NgayVaoLam;
                dbAdapter.sqlCommand.Parameters.Add("@ThanhPhan_Id", SqlDbType.UniqueIdentifier).Value = user.ThanhPhan_Id;
                dbAdapter.sqlCommand.Parameters.Add("@TrinhDoChuyenMon", SqlDbType.NVarChar).Value = user.TrinhDoChuyenMon;
                dbAdapter.sqlCommand.Parameters.Add("@Truong", SqlDbType.NVarChar).Value = user.Truong;
                dbAdapter.sqlCommand.Parameters.Add("@CapDoNhanSu_Id", SqlDbType.UniqueIdentifier).Value = user.CapDoNhanSu_Id;
                result += dbAdapter.runStoredNoneQuery();
            }
            dbAdapter.createStoredProceder("sp_Delete_DataDanhMuc");
            result += dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok();
        }
        [HttpGet("list-user-administrator")]
        public ActionResult GetListUserAdministrator(int page = 1, string keyword = null, Guid? donviId = null)
        {
            var pageSizeData = uow.Configs.GetAll(x => !x.IsDeleted).FirstOrDefault();
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListUserAdministrator");
            dbAdapter.sqlCommand.Parameters.Add("@keyword", SqlDbType.NVarChar).Value = keyword;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donviId;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            int totalRow = result.Count();
            int pageSize = pageSizeData.PageSize;
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPage)
            {
                page = totalPage;
            }

            var datalist = result.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                datalist
            });
        }
        [HttpGet("user-administrator")]
        public ActionResult GetUserAdministratorById(Guid? id = null, Guid? role_Id = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetUserAdministratorById");
            dbAdapter.sqlCommand.Parameters.Add("@user_Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = role_Id;
            var result = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpPost("user-administrator")]
        public ActionResult PostUserAdmin(Guid id, List<ClassAdminActive> model)
        {
            dbAdapter.connect();
            foreach (var item in model)
            {
                dbAdapter.createStoredProceder("sp_GetRoleAdmin");
                dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = item.Role_Id;
                var result = dbAdapter.runStored2ObjectList();
                if (!result.Any())
                {
                    dbAdapter.deConnect();
                    return StatusCode(StatusCodes.Status409Conflict, "Vai trò phải là administrator!");
                }
            }
            foreach (var item in model)
            {
                dbAdapter.createStoredProceder("sp_GetCheckExistsRole");
                dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = item.Role_Id;
                var result = dbAdapter.runStored2Object();
                if (result is ExpandoObject expandoObj && expandoObj.Count() > 0)
                {
                    dbAdapter.deConnect();
                    return StatusCode(StatusCodes.Status409Conflict, "Vai trò này đã có ở người dùng này!");
                }
            }
            foreach (var item in model)
            {
                dbAdapter.createStoredProceder("sp_PostUserAdmin");
                dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = item.Role_Id;
                dbAdapter.sqlCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = item.IsActive;
                dbAdapter.runStoredNoneQuery();
            }
            dbAdapter.deConnect();
            return Ok("Thêm mới vai trò thành công!");
        }
        [HttpPut("user-administrator")]
        public ActionResult PutUserAdmin(ClassAdminActive model)
        {
            dbAdapter.connect();

            dbAdapter.createStoredProceder("sp_GetRoleAdmin");
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = model.Role_Id;
            var result = dbAdapter.runStored2Object();
            if (result is ExpandoObject expandoObj && expandoObj.Count() <= 0)
            {
                dbAdapter.deConnect();
                return StatusCode(StatusCodes.Status409Conflict, "Vai trò phải là administrator!");
            }

            dbAdapter.createStoredProceder("sp_GetCheckExistsRole");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = model.Id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = model.Role_Id;
            var result2 = dbAdapter.runStored2Object();
            if (result2 is ExpandoObject expandoObj2 && expandoObj2.Count() > 0)
            {
                dbAdapter.deConnect();
                return StatusCode(StatusCodes.Status409Conflict, "Vai trò đã có ở người dùng này!");
            }

            dbAdapter.createStoredProceder("sp_DeleteRoleAdmin");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = model.Id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = model.Roleold_Id;
            dbAdapter.runStoredNoneQuery();

            dbAdapter.createStoredProceder("sp_PostUserAdmin");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = model.Id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = model.Role_Id;
            dbAdapter.sqlCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = model.IsActive;
            dbAdapter.runStoredNoneQuery();

            dbAdapter.deConnect();
            return Ok("Chỉnh sửa vai trò thành công!");
        }

        [HttpDelete("user-administrator")]
        public ActionResult DeleteUserAdmin(Guid id, Guid role_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteRoleAdmin");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = role_Id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok("Xóa vai trò thành công!");
        }

        [HttpGet("list-user-erp")]
        public ActionResult GetListUserERP(int page = 1, string keyword = null, Guid? donviId = null)
        {
            var pageSizeData = uow.Configs.GetAll(x => !x.IsDeleted).FirstOrDefault();
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListUserERP");
            dbAdapter.sqlCommand.Parameters.Add("@keyword", SqlDbType.NVarChar).Value = keyword;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donviId;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            int totalRow = result.Count();
            int pageSize = pageSizeData.PageSize;
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPage)
            {
                page = totalPage;
            }

            var datalist = result.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                datalist
            });
        }

        [HttpGet("user-erp")]
        public ActionResult GetUserERPById(Guid? id = null, Guid? role_Id = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetUserERPById");
            dbAdapter.sqlCommand.Parameters.Add("@user_Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = role_Id;
            var result = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            return Ok(result);
        }

        [HttpGet("user-all-role-erp")]
        public ActionResult GetUserAllERP(string keyword, Guid? donVi_Id, Guid? phanMem_id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListRoleERP");
            dbAdapter.sqlCommand.Parameters.Add("@donVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@phanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_id;
            dbAdapter.sqlCommand.Parameters.Add("@keyword", SqlDbType.NVarChar).Value = keyword;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpGet("user-all-role-erp-by-id")]
        public ActionResult GetUserRolesById(Guid? user_Id, Guid? donVi_Id, Guid? phanMem_id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetRoleUserERPById");
            dbAdapter.sqlCommand.Parameters.Add("@user_Id", SqlDbType.UniqueIdentifier).Value = user_Id;
            dbAdapter.sqlCommand.Parameters.Add("@donVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@phanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_id;
            var data = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpPut("put-role-erp")]
        public ActionResult PutRoleERP(Guid id, ClassCBNVActive model)
        {
            lock (Commons.LockObjectState)
            {
                if (id != model.Id)
                {
                    return BadRequest();
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_DeleteRoleByUserERPPhanMemDonVi");
                dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = model.PhanMem_Id;
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = model.DonVi_Id;
                var result = dbAdapter.runStoredNoneQuery();
                foreach (var item in model.ChiTietRoles)
                {
                    dbAdapter.createStoredProceder("sp_Post_AspNetUserRoleERPs");
                    dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@Role_Id", SqlDbType.UniqueIdentifier).Value = item.Role_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = model.PhanMem_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = model.DonVi_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@IsActive_Role", SqlDbType.Bit).Value = model.IsActive_Role;
                    result += dbAdapter.runStoredNoneQuery();
                }
                dbAdapter.deConnect();
                return Ok();
            }
        }

        [HttpPost("post-role-erp")]
        public ActionResult PostRoleERP(Guid id, ClassCBNVActive model)
        {
            lock (Commons.LockObjectState)
            {
                if (id != model.Id)
                {
                    return BadRequest();
                }
                dbAdapter.connect();
                foreach (var item in model.ChiTietRoles)
                {
                    dbAdapter.createStoredProceder("sp_Post_AspNetUserRoleERPs");
                    dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = model.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@Role_Id", SqlDbType.UniqueIdentifier).Value = item.Role_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = model.PhanMem_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = model.DonVi_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@IsActive_Role", SqlDbType.Bit).Value = model.IsActive_Role;
                    dbAdapter.runStoredNoneQuery();
                }
                dbAdapter.deConnect();
                return Ok();
            }
        }

        [HttpDelete("delete-all-role-erp")]
        public ActionResult DeleteAllRoleUserERP(Guid id, Guid phanMem_Id, Guid donVi_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteAllRoleERP");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_Id;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok("Xóa vai trò thành công!");
        }

        [HttpPut("user-erp")]
        public ActionResult PutUserERP(ClassAdminActive model)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetCheckExistsRole");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = model.Id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = model.Role_Id;
            var result2 = dbAdapter.runStored2Object();
            if (result2 is ExpandoObject expandoObj2 && expandoObj2.Count() > 0)
            {
                dbAdapter.deConnect();
                return StatusCode(StatusCodes.Status409Conflict, "Vai trò đã có ở người dùng này!");
            }

            dbAdapter.createStoredProceder("sp_DeleteRoleERP");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = model.Id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = model.Roleold_Id;
            dbAdapter.runStoredNoneQuery();

            dbAdapter.createStoredProceder("sp_PostUserERP");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = model.Id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = model.Role_Id;
            dbAdapter.sqlCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = model.IsActive;
            dbAdapter.runStoredNoneQuery();

            dbAdapter.deConnect();
            return Ok("Chỉnh sửa vai trò thành công!");
        }

        [HttpDelete("user-erp")]
        public ActionResult DeleteUserERP(Guid id, Guid role_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteRoleERP");
            dbAdapter.sqlCommand.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@role_Id", SqlDbType.UniqueIdentifier).Value = role_Id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok("Xóa vai trò thành công!");
        }

        [HttpPost("ExportFileExcel")]
        public ActionResult ExportFileExcel()
        {
            using ExcelPackage package = new(new FileInfo("Uploads/Templates/FileMauImport_CBNV.xlsx"));
            if (package.Workbook.Worksheets.Count == 0)
            {
                package.Workbook.Worksheets.Add("Sheet1");
            }
            ExcelWorksheet worksheet = package.Workbook.Worksheets["DM"];
            if (worksheet == null)
            {
                worksheet = package.Workbook.Worksheets.Add("DM");
            }
            int index = 2;
            var CapDoNhanSus = uow.CapDoNhanSus.GetAll(x => true).OrderBy(x => x.TenCapDoNhanSu);
            foreach (var item in CapDoNhanSus)
            {
                worksheet.Cells[index, 1].Value = item.MaCapDoNhanSu;
                worksheet.Cells[index, 2].Value = item.TenCapDoNhanSu;
                ++index;
            }
            index = 2;
            var ChucDanhs = uow.ChucDanhs.GetAll(x => true).OrderBy(x => x.TenChucDanh);
            foreach (var item in ChucDanhs)
            {
                worksheet.Cells[index, 4].Value = item.TenChucDanh;
                ++index;
            }
            index = 2;
            var ChucVus = uow.chucVus.GetAll(x => true).OrderBy(x => x.TenChucVu);
            foreach (var item in ChucVus)
            {
                worksheet.Cells[index, 6].Value = item.TenChucVu;
                ++index;
            }
            index = 2;
            var ThanhPhans = uow.ThanhPhans.GetAll(x => true).OrderBy(x => x.TenThanhPhan);
            foreach (var item in ThanhPhans)
            {
                worksheet.Cells[index, 8].Value = item.TenThanhPhan;
                ++index;
            }
            index = 2;
            var PhongBanHRMs = uow.PhongBanHRMs.GetAll(x => true).OrderBy(x => x.MaPhongBanHRM);
            var DonViHRMs = uow.DonViHRMs.GetAll(x => true).ToDictionary(x => x.Id, x => x.TenDonViHRM);
            var DonVis = uow.DonVis.GetAll(x => true).ToDictionary(x => x.Id, x => x.TenDonVi);
            foreach (var item in PhongBanHRMs)
            {
                worksheet.Cells[index, 10].Value = item.MaPhongBanHRM;
                worksheet.Cells[index, 11].Value = DonViHRMs[item.DonViHRM_Id];
                worksheet.Cells[index, 12].Value = DonVis[item.DonVi_Id];
                worksheet.Cells[index, 13].Value = item.TenPhongBan;
                ++index;
            }
            var DonViTraLuongs = uow.DonViTraLuongs.GetAll(x => true).OrderBy(x => x.TenDonViTraLuong);
            foreach (var item in DonViTraLuongs)
            {
                worksheet.Cells[index, 15].Value = item.TenDonViTraLuong;
                ++index;
            }
            return Ok(new { dataexcel = package.GetAsByteArray() });
        }
        [HttpPost("ExportListExcel_DanhSachCBNV")]
        public ActionResult ExportFileExcel_DanhSachCBNV(string tenPhongBan, Guid donvi_Id, string keyword)
        {
            var donVi = uow.DonVis.GetById(donvi_Id);
            if (donVi == null || donVi.IsDeleted)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Đơn vị không tồn tại hoặc đã bị xóa!");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNV");
            dbAdapter.sqlCommand.Parameters.Add("@tenPhongBan", SqlDbType.NVarChar).Value = tenPhongBan;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            using ExcelPackage package = new(new FileInfo("Uploads/Templates/ExportDanhSachCBNV.xlsx"));
            if (package.Workbook.Worksheets.Count == 0)
            {
                package.Workbook.Worksheets.Add("Sheet1");
            }
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            int stt = 1;
            int startRow = 7;
            worksheet.Cells["C3"].Value = donVi.TenDonVi;
            foreach (dynamic item in result)
            {
                worksheet.InsertRow(startRow, 1, 6);
                worksheet.Row(startRow).Height = 20;
                worksheet.Cells["A" + startRow].Value = stt;
                worksheet.Cells["B" + startRow].Value = item.maNhanVien;
                worksheet.Cells["C" + startRow].Value = item.fullName;
                worksheet.Cells["D" + startRow].Value = item.email;
                worksheet.Cells["E" + startRow].Value = item.phoneNumber;
                worksheet.Cells["F" + startRow].Value = item.tenChucVu;
                worksheet.Cells["G" + startRow].Value = item.tenPhongBan;
                worksheet.Cells["H" + startRow].Value = item.tenDonViTraLuong;
                ++stt;
                ++startRow;
            }
            worksheet.DeleteRow(6);
            return Ok(new { dataexcel = package.GetAsByteArray() });
        }
        [HttpGet("cbnv-erp")]
        public ActionResult GetListCBNVERP(string maPhongBanHRM, string keyword, Guid? donvi_Id, int page)
        {
            if (string.IsNullOrWhiteSpace(keyword)) keyword = null;
            else keyword = keyword.Trim();
            var pageSizeData = uow.Configs.GetAll(x => !x.IsDeleted).FirstOrDefault();
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNVERP");
            dbAdapter.sqlCommand.Parameters.Add("@maPhongBanHRM", SqlDbType.NVarChar).Value = maPhongBanHRM;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            if (page == -1)
            {
                return Ok(result);
            }
            int totalRow = result.Count();
            int pageSize = pageSizeData.PageSize;
            int totalPage = (int)Math.Ceiling(totalRow / (double)pageSize);
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPage)
            {
                page = totalPage;
            }
            var datalist = result.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new
            {
                totalRow,
                totalPage,
                pageSize,
                datalist
            });
        }
        [HttpPost("export-excel-cbnv-erp")]
        public async Task<ActionResult> ExportExcelCBNVERP(Guid? donvi_Id, string maPhongBanHRM, string keyword)
        {
            var appUser = await userManager.FindByIdAsync(User.Identity.Name);
            var hasRole = await userManager.IsInRoleAsync(appUser, "QUANTRI_CBNV_ERP");
            if (!hasRole)
            {
                var isAdmin = await userManager.IsInRoleAsync(appUser, "Administrator");
                if (!isAdmin)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
                }
            }
            var donVi = uow.DonVis.GetById(donvi_Id);
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListCBNVERP");
            dbAdapter.sqlCommand.Parameters.Add("@maPhongBanHRM", SqlDbType.NVarChar).Value = maPhongBanHRM;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donvi_Id;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            using ExcelPackage package = new(new FileInfo("Uploads/Templates/FileMauExport_CBNV.xlsx"));
            if (package.Workbook.Worksheets.Count == 0)
            {
                package.Workbook.Worksheets.Add("Sheet1");
            }
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            int stt = 1;
            int startRow = 6;
            worksheet.Cells["B2"].Value = donVi?.TenDonVi;
            foreach (dynamic item in result)
            {
                worksheet.InsertRow(startRow, 1, 5);
                worksheet.Row(startRow).Height = 25;
                worksheet.Cells[startRow, 1].Value = stt;
                worksheet.Cells[startRow, 2].Value = item.maNhanVien;
                worksheet.Cells[startRow, 3].Value = item.fullName;
                worksheet.Cells[startRow, 4].Value = item.ngaySinh;
                worksheet.Cells[startRow, 5].Value = item.ngayVaoLam;
                worksheet.Cells[startRow, 6].Value = item.email;
                worksheet.Cells[startRow, 7].Value = item.phoneNumber;
                worksheet.Cells[startRow, 8].Value = item.tenCapDoNhanSu;
                worksheet.Cells[startRow, 9].Value = item.tenChucDanh;
                worksheet.Cells[startRow, 10].Value = item.tenChucVu;
                worksheet.Cells[startRow, 11].Value = item.tenThanhPhan;
                worksheet.Cells[startRow, 12].Value = item.maPhongBanHRM;
                worksheet.Cells[startRow, 13].Value = item.tenPhongBan;
                worksheet.Cells[startRow, 14].Value = item.tenDonVi;
                worksheet.Cells[startRow, 15].Value = item.tenDonViTraLuong;
                worksheet.Cells[startRow, 16].Value = item.trinhDoChuyenMon;
                worksheet.Cells[startRow, 17].Value = item.truong;
                worksheet.Cells[startRow, 18].Value = item.chuyenNganh;
                worksheet.Cells[startRow, 19].Value = item.ghiChu;
                ++stt;
                ++startRow;
            }
            worksheet.DeleteRow(5);
            worksheet.Cells.AutoFitColumns();
            worksheet.Column(2).Width = 12;
            return Ok(new { dataexcel = package.GetAsByteArray() });
        }
        public class ClassCBNVNghiViecDieuChuyenImport
        {
            private string _maNhanVien;
            [StringLength(10)]
            [RegularExpression(@"^\d{1,10}$", ErrorMessage = "Mã nhân viên chỉ được chứa số và có độ dài không quá 10 chữ số.")]
            [Required]
            public string MaNhanVien
            {
                get { return _maNhanVien; }
                set { _maNhanVien = CleanUp(value); }
            }
            private string _ngayNghiViec;
            public string NgayNghiViec
            {
                get { return _ngayNghiViec; }
                set { _ngayNghiViec = CleanUp(value); }
            }
            private string _ghiChu;
            [StringLength(250)]
            public string GhiChu
            {
                get { return _ghiChu; }
                set { _ghiChu = CleanUp(value); }
            }
            public string GhiChuImport { get; set; }
            public static string CleanUp(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return null;
                return input.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "");
            }
        }
        public class ClassCBNVNghiViecDieuChuyenSave
        {
            public Guid Id { get; set; }
            public DateTime? NgayNghiViec { get; set; }
            public string GhiChu { get; set; }
        }
        [HttpPost("import-nghi-viec-dieu-chuyen")]
        public IActionResult ImportNghiViecDieuChuyen(List<ClassCBNVNghiViecDieuChuyenImport> data)
        {
            if (data == null || !data.Any()) return StatusCode(StatusCodes.Status409Conflict, "Chưa gửi dữ liệu!");
            List<ClassCBNVNghiViecDieuChuyenSave> users = new();
            List<ClassCBNVNghiViecDieuChuyenImport> errors = new();
            var Users = userManager.Users.Where(x => !x.IsDeleted);
            foreach (var item in data)
            {
                ClassCBNVNghiViecDieuChuyenSave user = new();
                if (string.IsNullOrEmpty(item.MaNhanVien))
                {
                    item.GhiChuImport = $"Mã nhân viên bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                var userByMaNhanVien = Users.FirstOrDefault(x => x.MaNhanVien == item.MaNhanVien);
                if (userByMaNhanVien == null)
                {
                    item.GhiChuImport = $"Mã nhân viên không tồn tại!";
                    errors.Add(item);
                    continue;
                }
                if (userByMaNhanVien.NghiViec)
                {
                    item.GhiChuImport = $"Nhân viên đã nghỉ việc!";
                    errors.Add(item);
                    continue;
                }
                if (string.IsNullOrEmpty(userByMaNhanVien.MaPhongBanHRM) && !userByMaNhanVien.IsActive)
                {
                    item.GhiChuImport = $"Nhân viên đã điều chuyển!";
                    errors.Add(item);
                    continue;
                }
                if (!string.IsNullOrEmpty(item.NgayNghiViec))
                {
                    if (!DateTime.TryParseExact(item.NgayNghiViec, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime NgayNghiViec))
                    {
                        item.GhiChuImport = $"Ngày nghỉ việc {item.NgayNghiViec} sai định dạng!";
                        errors.Add(item);
                        continue;
                    }
                    else user.NgayNghiViec = NgayNghiViec;
                }
                user.Id = userByMaNhanVien.Id;
                user.GhiChu = item.GhiChu;
                users.Add(user);
            }
            if (errors.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, errors);
            }
            dbAdapter.connect();
            int result = 0;
            Guid userId = Guid.Parse(User.Identity.Name);
            foreach (var user in users)
            {
                dbAdapter.createStoredProceder("sp_Put_AspNetUser_NghiViecDieuChuyen");
                dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = user.Id;
                dbAdapter.sqlCommand.Parameters.Add("@NgayNghiViec", SqlDbType.DateTime2).Value = user.NgayNghiViec;
                dbAdapter.sqlCommand.Parameters.Add("@GhiChu", SqlDbType.NVarChar).Value = user.GhiChu;
                result += dbAdapter.runStoredNoneQuery();
                if (user.NgayNghiViec.HasValue)
                {
                    dbAdapter.createStoredProceder("sp_vptq_lms_Put_LopHocChiTiet_HuyDaoTaoNghiViec");
                    dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = user.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier).Value = userId;
                    dbAdapter.runStoredNoneQuery();
                }
            }
            dbAdapter.deConnect();
            return Ok();
        }
        [HttpPost("export-excel-nghi-viec-dieu-chuyen")]
        public ActionResult ExportFileExcelNghiViecDieuChuyen()
        {
            using ExcelPackage package = new(new FileInfo("Uploads/Templates/FileMauImport_CBNVNghiViecDieuChuyen.xlsx"));
            return Ok(new { dataexcel = package.GetAsByteArray() });
        }
        [HttpGet("phong-ban-by-don-vi")]
        public ActionResult GetPhongBanByDonVi(Guid donVi_Id)
        {
            var data = uow.PhongBanHRMs.GetAll(x => x.DonVi_Id == donVi_Id).Select(x => new
            {
                x.MaPhongBanHRM,
                x.TenPhongBan
            }).Distinct();
            return Ok(data);
        }
        [HttpGet("user-by-dv-pb")]
        public ActionResult GetUserByDVPB(Guid donVi_Id, string tenPhongBan, string keyword)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_Get_ListUserByDonVi_Id");
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@TenPhongBan", SqlDbType.NVarChar).Value = tenPhongBan;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }
        [HttpGet("user-by-dv-pb-thaco")]
        public ActionResult GetUserByDVPBThaco(Guid? donVi_Id, Guid? phongBanThaco_Id, string keyword)
        {
            keyword = keyword?.Trim();
            if (string.IsNullOrEmpty(keyword)) keyword = null;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetList_UserByDonViId_PhongBanThacoId");
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@PhongBanThaco_Id", SqlDbType.UniqueIdentifier).Value = phongBanThaco_Id;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }
    }
}