using ERP.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using System.ComponentModel.DataAnnotations;
using ERP.Models.Default;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PhanMemController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DbAdapter dbAdapter;
        private readonly RoleManager<ApplicationRole> roleManager;
        public PhanMemController(IConfiguration _configuration, IUnitofWork _uow, UserManager<ApplicationUser> _userManager, RoleManager<ApplicationRole> _roleManager)
        {
            uow = _uow;
            userManager = _userManager;
            roleManager = _roleManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        public static readonly Guid PhanMemDaoTao_Id = Guid.Parse("08CD2D3C-9594-48CC-9444-1CEF15082754");
        public static readonly Guid PhanMemQTSX_Id = Guid.Parse("840C40FD-D368-405F-928B-3B9B92B6D220");
        [HttpGet]
        public ActionResult Get(string keyword, int page = 1)
        {
            if (keyword == null) keyword = "";
            var pageSizeData = uow.Configs.GetAll(x => !x.IsDeleted).ToList();
            var data = uow.phanMems.GetAll(t => !t.IsDeleted && (t.MaPhanMem.ToLower().Contains(keyword.ToLower()) || t.TenPhanMem.ToLower().Contains(keyword.ToLower()))).Select(x => new
            {
                x.Id,
                x.MaPhanMem,
                x.TenPhanMem,
                x.Icon,
                x.HinhAnh,
                x.NguoiQuanLy_Id,
                x.IsDungChung,
                x.IsSuDungNgoai,
                x.UrlPhamMemNgoai,
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


        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var query = uow.phanMems.GetAll(x => x.Id == id).Select(x => new
            {
                x.Id,
                x.MaPhanMem,
                x.TenPhanMem,
                x.Icon,
                x.HinhAnh,
                x?.NguoiQuanLy_Id,
                x.IsDungChung,
                x.IsSuDungNgoai,
                x.UrlPhamMemNgoai,
            }).FirstOrDefault();
            if (query == null)
            {
                return NotFound();
            }
            return Ok(query);
        }

        [HttpGet("phan-mem-by-user")]
        public ActionResult GetPMByUser(Guid? user_Id, Guid? donVi_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListPhanMem");
            dbAdapter.sqlCommand.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = user_Id;
            dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpGet("user-all-role")]
        public ActionResult GetUserAllRoles(string keyword, Guid? donVi_Id, Guid? phanMem_id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListRoleCBNV");
            dbAdapter.sqlCommand.Parameters.Add("@donVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@phanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_id;
            dbAdapter.sqlCommand.Parameters.Add("@keyword", SqlDbType.NVarChar).Value = keyword;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpGet("user-cbnv-role-by-id")]
        public ActionResult GetUserRolesById(Guid? user_Id, Guid? donVi_Id, Guid? phanMem_id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetRoleCBNVById");
            dbAdapter.sqlCommand.Parameters.Add("@user_Id", SqlDbType.UniqueIdentifier).Value = user_Id;
            dbAdapter.sqlCommand.Parameters.Add("@donVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@phanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_id;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpGet("phan-mem-by-user-only")]
        public ActionResult GetListPhanMemOnlyExceptDonVi(Guid? user_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListPhanMemOnlyExceptDonVi");
            dbAdapter.sqlCommand.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = user_Id;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpGet("list-phan-mem-by-donvi")]
        public ActionResult GetListPhanMemBYDonVi(Guid? donVi_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListPhanMemByDonVi");
            dbAdapter.sqlCommand.Parameters.Add("@donVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpGet("phan-mem-for-menu")]
        public ActionResult GetPMForMenu()
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetListPhanMemForMenu");
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpGet("list-don-vi-for-user")]
        public ActionResult GetListDonViForUser()
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_Get_ListDonViByUser_Id");
            dbAdapter.sqlCommand.Parameters.Add("@User_Id", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var data = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(data);
        }

        [HttpPost("default-phan-mem-for-user")]
        public ActionResult PostDefaultPMForMenu(Guid user_Id, Guid role_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_PutDefaultPhanMemByUser");
            dbAdapter.sqlCommand.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = user_Id;
            dbAdapter.sqlCommand.Parameters.Add("@roleId", SqlDbType.UniqueIdentifier).Value = role_Id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok();
        }

        [HttpPost("active-phan-mem-for-user")]
        public ActionResult PostActivePMForUser(Guid user_Id, Guid role_Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_PutActiveRoleByPhanMem");
            dbAdapter.sqlCommand.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = user_Id;
            dbAdapter.sqlCommand.Parameters.Add("@roleId", SqlDbType.UniqueIdentifier).Value = role_Id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok();
        }
        [HttpPost("active-all-pm-for-cbnv")]
        public ActionResult PostActivePMforCBNV(Guid? user_Id, Guid? donVi_Id, Guid? phanMem_id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_PutActiveAllRoleforUser");
            dbAdapter.sqlCommand.Parameters.Add("@user_Id", SqlDbType.UniqueIdentifier).Value = user_Id;
            dbAdapter.sqlCommand.Parameters.Add("@donVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
            dbAdapter.sqlCommand.Parameters.Add("@phanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_id;
            dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            return Ok();
        }

        [HttpPost]
        public ActionResult Post(PhanMem data)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (uow.phanMems.Exists(x => x.MaPhanMem == data.MaPhanMem && !x.IsDeleted))
                    return StatusCode(StatusCodes.Status409Conflict, "Mã " + data.MaPhanMem + " đã tồn tại trong hệ thống");
                if (data.IsSuDungNgoai && data.UrlPhamMemNgoai == null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Phần mềm sử dụng ngoài bắt buộc phải nhập đường dẫn");
                }
                PhanMem bp = new PhanMem();
                Guid id = Guid.NewGuid();
                bp.Id = id;
                bp.MaPhanMem = data.MaPhanMem;
                bp.TenPhanMem = data.TenPhanMem;
                bp.Icon = data.Icon;
                bp.HinhAnh = data.HinhAnh;
                bp.NguoiQuanLy_Id = data.NguoiQuanLy_Id;
                bp.CreatedDate = DateTime.Now;
                bp.CreatedBy = Guid.Parse(User.Identity.Name);
                bp.UrlPhamMemNgoai = data.UrlPhamMemNgoai;
                bp.IsDungChung = data.IsDungChung;
                bp.IsSuDungNgoai = data.IsSuDungNgoai;
                uow.phanMems.Add(bp);
                uow.Complete();
                return Ok();
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(Guid id, PhanMem data)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (id != data.Id)
                {
                    return BadRequest();
                }
                if (id == PhanMemDaoTao_Id)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Phần mềm đã được sử dụng! Không thể sửa!");
                }
                if (data.IsSuDungNgoai && data.UrlPhamMemNgoai == null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Phần mềm sử dụng ngoài bắt buộc phải nhập đường dẫn");
                }
                if (uow.phanMems.Exists(x => x.MaPhanMem == data.MaPhanMem && x.Id != data.Id && !x.IsDeleted))
                    return StatusCode(StatusCodes.Status409Conflict, "Mã " + data.MaPhanMem + " đã tồn tại trong hệ thống");
                else
                {
                    var d = uow.phanMems.GetById(id);
                    d.UpdatedBy = Guid.Parse(User.Identity.Name);
                    d.UpdatedDate = DateTime.Now;
                    d.MaPhanMem = data.MaPhanMem;
                    d.TenPhanMem = data.TenPhanMem;
                    d.Icon = data.Icon;
                    d.HinhAnh = data.HinhAnh;
                    d.NguoiQuanLy_Id = data.NguoiQuanLy_Id;
                    d.UrlPhamMemNgoai = data.UrlPhamMemNgoai;
                    d.IsDungChung = data.IsDungChung;
                    d.IsSuDungNgoai = data.IsSuDungNgoai;
                    uow.phanMems.Update(d);
                }
                uow.Complete();
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            lock (Commons.LockObjectState)
            {
                PhanMem duLieu = uow.phanMems.GetById(id);
                if (duLieu == null)
                {
                    return NotFound();
                }
                var check = roleManager.Roles.FirstOrDefault(x => x.PhanMem_Id == id);
                if (uow.Menus.Exists(x => x.PhanMem_Id == id && !x.IsDeleted)
                    || uow.phienBans.Exists(x => x.PhanMem_Id == id && !x.IsDeleted)
                    || check != null
                    || id == PhanMemDaoTao_Id
                )
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Phần mềm đã được sử dụng! Không thể xóa!");
                }
                duLieu.DeletedDate = DateTime.Now;
                duLieu.DeletedBy = Guid.Parse(User.Identity.Name);
                duLieu.IsDeleted = true;
                uow.phanMems.Update(duLieu);
                uow.Complete();
                return Ok(duLieu);
            }
        }
        [HttpPost("export-file-mau-phan-quyen")]
        public ActionResult PostExportFileMauPhanQuyen(Guid? donVi_Id, Guid phanMem_Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = roleManager.Roles.FirstOrDefault(x => !x.IsDeleted && x.PhanMem_Id == phanMem_Id && x.DonVi_Id == donVi_Id && x.Name.ToUpper().Contains("ADMINISTRATOR"));
            //if (role == null) NotFound(); //Không cần cũng được
            var appUser = userManager.FindByIdAsync(User.Identity.Name).Result;
            var IsAdmin = userManager.IsInRoleAsync(appUser, role.Name).Result;
            if (!IsAdmin) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            using (ExcelPackage package = new ExcelPackage(new FileInfo("Uploads/Templates/FileMauImport_PhanQuyenCBNVTheoPhanMemDonVi.xlsx")))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    package.Workbook.Worksheets.Add("Sheet1");
                }
                var donVi = uow.DonVis.GetById(donVi_Id);
                if (donVi == null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Đơn vị không tồn tại!");
                }
                var phanMem = uow.phanMems.GetById(phanMem_Id);
                if (phanMem == null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Phần mềm không tồn tại!");
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Get_UserByDonVi_Id");
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
                var users = dbAdapter.runStored2ObjectList();
                dbAdapter.createStoredProceder("sp_Get_RoleByDonVi_IdPhanMem_Id");
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = donVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = phanMem_Id;
                var roles = dbAdapter.runStored2ObjectList();
                dbAdapter.deConnect();
                if (users.Count() == 0)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Đơn vị chưa có dữ liệu CBNV!");
                }
                if (roles.Count() == 0)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Đơn vị chưa được phân quyền phần mềm hoặc chưa tạo quyền!");
                }
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Import"];
                worksheet.Cells["C2"].Value = donVi.TenDonVi;
                worksheet.Cells["C3"].Value = phanMem.TenPhanMem;
                worksheet = package.Workbook.Worksheets["CBNV"];
                int indexrow = 5, stt = 1;
                foreach (dynamic item in users)
                {
                    worksheet.InsertRow(indexrow, 1, 4);
                    worksheet.Row(indexrow).Height = 25;
                    worksheet.Cells[indexrow, 1].Value = stt;
                    worksheet.Cells[indexrow, 2].Value = item.maNhanVien;
                    worksheet.Cells[indexrow, 3].Value = item.fullName;
                    worksheet.Cells[indexrow, 4].Value = item.tenPhongBan;
                    worksheet.Cells[indexrow, 5].Value = donVi.TenDonVi;
                    stt++;
                    indexrow++;
                }
                worksheet.DeleteRow(4);
                worksheet = package.Workbook.Worksheets["Quyền"];
                indexrow = 5;
                stt = 1;
                foreach (dynamic item in roles)
                {
                    worksheet.InsertRow(indexrow, 1, 4);
                    worksheet.Row(indexrow).Height = 25;
                    worksheet.Cells[indexrow, 1].Value = stt;
                    worksheet.Cells[indexrow, 2].Value = item.maQuyen;
                    worksheet.Cells[indexrow, 3].Value = item.tenQuyen;
                    stt++;
                    indexrow++;
                }
                worksheet.DeleteRow(4);
                /*MemoryStream stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/octet-stream", "FileExportExcel.xlsx");*/
                return Ok(new { dataexcel = package.GetAsByteArray() });
            }
        }
        public class Class_ImportQuyenCBNV
        {
            public Guid DonVi_Id { get; set; }
            public Guid PhanMem_Id { get; set; }
            public List<Class_ImportQuyenCBNVChiTiet> list_ChiTiets { get; set; }
        }
        public class Class_ImportQuyenCBNVChiTiet
        {
            public string MaNhanVien { get; set; }
            public string MaQuyen { get; set; }
            public string GhiChuImport { get; set; }
        }
        public class Class_ImportQuyenCBNVChiTietSave
        {
            public Guid User_Id { get; set; }
            public List<Guid> Role_Ids { get; set; }
        }
        [HttpPost("import")]
        public async Task<ActionResult> Import(Class_ImportQuyenCBNV data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (data == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Chưa có dữ liệu!");
            }
            var role = roleManager.Roles.FirstOrDefault(x => !x.IsDeleted && x.PhanMem_Id == data.PhanMem_Id && x.DonVi_Id == data.DonVi_Id && x.Name.ToUpper().Contains("ADMINISTRATOR"));
            //if (role == null) NotFound(); //Không cần cũng được
            var appUser = await userManager.FindByIdAsync(User.Identity.Name);
            var IsAdmin = await userManager.IsInRoleAsync(appUser, role.Name);
            if (!IsAdmin) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            if (data.list_ChiTiets == null || !data.list_ChiTiets.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, "Chưa có danh sách CBNV!");
            }
            lock (Commons.LockObjectState)
            {
                var trungs = data.list_ChiTiets.Select(x => new { MaNhanVien = x.MaNhanVien.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim() })
                    .GroupBy(x => new { x.MaNhanVien })
                    .Select(x => new { x.Key.MaNhanVien, SoLuong = x.Count() })
                    .Where(x => x.SoLuong > 1)
                    .Select(x => x.MaNhanVien);
                if (trungs.Any())
                {
                    return StatusCode(StatusCodes.Status409Conflict, data.list_ChiTiets.Where(x => trungs.Contains(x.MaNhanVien.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim())).Select(x => new
                    {
                        x.MaQuyen,
                        x.MaNhanVien,
                        GhiChuImport = "Trùng CBNV"
                    }));
                }
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Get_UserByDonVi_Id");
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = data.DonVi_Id;
                var users = dbAdapter.runStored2ObjectList();
                dbAdapter.createStoredProceder("sp_Get_RoleByDonVi_IdPhanMem_Id");
                dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = data.DonVi_Id;
                dbAdapter.sqlCommand.Parameters.Add("@PhanMem_Id", SqlDbType.UniqueIdentifier).Value = data.PhanMem_Id;
                var roles = dbAdapter.runStored2ObjectList();
                if (!users.Any())
                {
                    dbAdapter.deConnect();
                    return StatusCode(StatusCodes.Status409Conflict, "Đơn vị chưa có dữ liệu CBNV!");
                }
                if (!roles.Any())
                {
                    dbAdapter.deConnect();
                    return StatusCode(StatusCodes.Status409Conflict, "Đơn vị chưa được phân quyền phần mềm!");
                }
                Dictionary<string, Guid> nhanViens = new();
                Dictionary<string, Guid> quyens = new();
                foreach (dynamic item in users)
                {
                    nhanViens.Add((string)item.maNhanVien, (Guid)item.user_Id);
                }
                foreach (dynamic item in roles)
                {
                    quyens.Add(((string)item.maQuyen).ToUpper(), (Guid)item.role_Id);
                }
                List<Class_ImportQuyenCBNVChiTiet> errors = new List<Class_ImportQuyenCBNVChiTiet>();
                List<Class_ImportQuyenCBNVChiTietSave> saves = new List<Class_ImportQuyenCBNVChiTietSave>();
                foreach (var item in data.list_ChiTiets)
                {
                    Class_ImportQuyenCBNVChiTietSave save = new Class_ImportQuyenCBNVChiTietSave();
                    if (string.IsNullOrWhiteSpace(item.MaNhanVien))
                    {
                        item.GhiChuImport = "Mã nhân viên bắt buộc nhập";
                        errors.Add(item);
                        continue;
                    }
                    string maNhanVien = item.MaNhanVien.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                    if (!nhanViens.ContainsKey(maNhanVien))
                    {
                        item.GhiChuImport = $"CBNV có mã {maNhanVien} không tồn tại hoặc không thuộc đơn vị";
                        errors.Add(item);
                        continue;
                    }
                    else
                    {
                        save.User_Id = nhanViens[maNhanVien];
                    }
                    save.Role_Ids = new List<Guid>();
                    var maQuyens = item.MaQuyen.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace(" ", "").Split(",").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToUpper());
                    if (maQuyens.Any())
                    {
                        foreach (var maQuyen in maQuyens)
                        {
                            if (!quyens.ContainsKey(maQuyen))
                            {
                                item.GhiChuImport = $"Mã quyền {maQuyen} không tồn tại";
                                errors.Add(item);
                                continue;
                            }
                            else save.Role_Ids.Add(quyens[maQuyen]);
                        }
                    }
                    else
                    {
                        item.GhiChuImport = $"Mã quyền bắt buộc nhập";
                        errors.Add(item);
                        continue;
                    }
                    saves.Add(save);
                }
                if (errors.Any())
                {
                    dbAdapter.deConnect();
                    return StatusCode(StatusCodes.Status409Conflict, errors);
                }
                foreach (var item in saves)
                {
                    foreach (var Role_Id in item.Role_Ids)
                    {
                        dbAdapter.createStoredProceder("sp_Post_AspNetUserRole");
                        dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = item.User_Id;
                        dbAdapter.sqlCommand.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = Role_Id;
                        dbAdapter.runStoredNoneQuery();
                    }
                }
                dbAdapter.deConnect();
                return Ok();
            }
        }

        public class Class_ImportQuyenCBNVChiTietPost
        {
            private string _maNhanVien;
            [Required]
            public string MaNhanVien
            {
                get { return _maNhanVien; }
                set { _maNhanVien = CleanUp(value); }
            }
            private string _maPhanMem;
            [Required]
            public string MaPhanMem
            {
                get { return _maPhanMem; }
                set { _maPhanMem = CleanUp(value); }
            }
            private string _maDonVi;
            public string MaDonVi
            {
                get { return _maDonVi; }
                set { _maDonVi = CleanUp(value); }
            }
            private string _maQuyen;
            [Required]
            public string MaQuyen
            {
                get { return _maQuyen; }
                set { _maQuyen = CleanUp(value); }
            }
            public Guid? DonVi_Id { get; set; }
            public Guid PhanMem_Id { get; set; }
            public Guid Role_Id { get; set; }
            public string GhiChuImport { get; set; }
            public static string CleanUp(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return null;
                return input.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "").ToUpper();
            }
        }
        public class Class_ImportQuyenCBNVChiTietSavePost
        {
            public Guid User_Id { get; set; }
            public List<Guid> Role_Ids { get; set; } = new();
        }
        [HttpPost("import-cbnv-dv-pm")]
        public async Task<ActionResult> ImportQuyenCBNVNhieuDVPM(List<Class_ImportQuyenCBNVChiTietPost> data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (data == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Chưa có dữ liệu!");
            }
            var role = roleManager.Roles.FirstOrDefault(x => !x.IsDeleted && x.PhanMem_Id == Guid.Parse("8b1c18ab-d65f-4065-99ed-5eb47a03153f") && x.DonVi_Id == Guid.Parse("d12ca19c-2e1a-41b7-86f3-3eb3c7d81a90") && x.Name.ToUpper().Contains("ADMINISTRATOR"));
            //if (role == null) NotFound(); //Không cần cũng được
            var appUser = await userManager.FindByIdAsync(User.Identity.Name);
            var IsAdmin = await userManager.IsInRoleAsync(appUser, role.Name);
            if (!IsAdmin) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            if (data == null || !data.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, "Chưa có danh sách CBNV!");
            }
            lock (Commons.LockObjectState)
            {
                var trungs = data.GroupBy(x => new { x.MaNhanVien, x.MaDonVi, x.MaPhanMem })
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key.MaNhanVien);
                if (trungs.Any())
                {
                    return StatusCode(StatusCodes.Status409Conflict, data.Where(x => trungs.Contains(x.MaNhanVien)).Select(x =>
                    {
                        x.GhiChuImport = "Trùng dữ liệu";
                        return x;
                    }));
                }
                var DonVis = uow.DonVis.GetAll(x => !x.IsDeleted).ToDictionary(x => x.MaDonVi.ToUpper(), x => x.Id);
                var PhanMems = uow.phanMems.GetAll(x => !x.IsDeleted).ToDictionary(x => x.MaPhanMem.ToUpper(), x => x.Id);
                var Roles = roleManager.Roles.Where(x => !x.IsDeleted).Select(x => new
                {
                    x.Id,
                    x.PhanMem_Id,
                    x.DonVi_Id,
                    Name = x.Name.ToUpper(),
                });
                var Users = userManager.Users.Where(x => !x.IsDeleted).ToDictionary(x => x.MaNhanVien, x => x.Id);
                List<Class_ImportQuyenCBNVChiTietPost> errors = new List<Class_ImportQuyenCBNVChiTietPost>();
                List<Class_ImportQuyenCBNVChiTietSavePost> saves = new List<Class_ImportQuyenCBNVChiTietSavePost>();
                foreach (var item in data)
                {
                    Class_ImportQuyenCBNVChiTietSavePost save = new();
                    if (item.MaDonVi != null)
                    {
                        if (DonVis.ContainsKey(item.MaDonVi))
                        {
                            item.DonVi_Id = DonVis[item.MaDonVi];
                        }
                        else
                        {
                            item.GhiChuImport = $"Mã đơn vị {item.MaDonVi} không tồn tại!";
                            errors.Add(item);
                            continue;
                        }
                    }
                    else
                    {
                        item.DonVi_Id = null;
                    }
                    if (PhanMems.ContainsKey(item.MaPhanMem))
                    {
                        item.PhanMem_Id = PhanMems[item.MaPhanMem];
                    }
                    else
                    {
                        item.GhiChuImport = $"Mã phần mềm {item.MaPhanMem} không tồn tại!";
                        errors.Add(item);
                        continue;
                    }
                    if (!Users.ContainsKey(item.MaNhanVien))
                    {
                        item.GhiChuImport = $"Mã nhân viên {item.MaNhanVien} không tồn tại!";
                        errors.Add(item);
                        continue;
                    }
                    else
                    {
                        save.User_Id = Users[item.MaNhanVien];
                    }
                    var RolePMDVs = Roles.Where(x => x.DonVi_Id == item.DonVi_Id && x.PhanMem_Id == item.PhanMem_Id);
                    var MaQuyens = item.MaQuyen.Replace(" ", "").Split(",").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToUpper());
                    foreach (var MaQuyen in MaQuyens)
                    {
                        var Role = RolePMDVs.FirstOrDefault(x => x.Name == MaQuyen);
                        if (Role == null)
                        {
                            item.GhiChuImport = $"Mã quyền {item.MaQuyen} không tồn tại";
                            errors.Add(item);
                            continue;
                        }
                        else
                        {
                            save.Role_Ids.Add(Role.Id);
                        }
                    }
                    saves.Add(save);
                }
                if (errors.Any())
                {
                    return StatusCode(StatusCodes.Status409Conflict, errors);
                }
                dbAdapter.connect();
                foreach (var item in saves)
                {
                    foreach (var Role_Id in item.Role_Ids)
                    {
                        dbAdapter.createStoredProceder("sp_Post_AspNetUserRole");
                        dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = item.User_Id;
                        dbAdapter.sqlCommand.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = Role_Id;
                        dbAdapter.runStoredNoneQuery();
                    }
                }
                dbAdapter.deConnect();
                return Ok();
            }
        }

        [HttpPost("export-file-mau-phan-quyen-toan-don-vi")]
        public ActionResult PostExportFileMauPhanQuyenToanDonVi()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = roleManager.Roles.FirstOrDefault(x => !x.IsDeleted && x.PhanMem_Id == Guid.Parse("8b1c18ab-d65f-4065-99ed-5eb47a03153f") && x.DonVi_Id == Guid.Parse("d12ca19c-2e1a-41b7-86f3-3eb3c7d81a90") && x.Name.ToUpper().Contains("ADMINISTRATOR"));
            //if (role == null) NotFound(); //Không cần cũng được
            var appUser = userManager.FindByIdAsync(User.Identity.Name).Result;
            var IsAdmin = userManager.IsInRoleAsync(appUser, role.Name).Result;
            if (!IsAdmin) return StatusCode(StatusCodes.Status409Conflict, "Bạn không có quyền thực hiện chức năng này!");
            using ExcelPackage package = new(new FileInfo("Uploads/Templates/FileMauImport_PhanQuyenCBNVTuNhieuPhanMemDonVi.xlsx"));
            if (package.Workbook.Worksheets.Count == 0)
            {
                package.Workbook.Worksheets.Add("Sheet1");
            }
            return Ok(new { dataexcel = package.GetAsByteArray() });
        }
    }
}
