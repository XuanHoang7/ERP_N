using ERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using ThacoLibs;
using ERP.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;
using System.Text.RegularExpressions;
using Google.Apis.Util;
using Spire.Xls;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Workbook = Spire.Xls.Workbook;
using DocumentFormat.OpenXml.Office2010.Excel;
using iTextSharp.text;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using ERP.Models.DanhMuc;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DanhMucPIController : Controller
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public DanhMucPIController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitof)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitof;
        }
        [HttpPost]
        public IActionResult AddDanhMucPI([FromBody] DanhMucPIDTO model)
        {
            if (model == null)
                return BadRequest("Dữ liệu không hợp lệ.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var donViDanhGia = model.IdDonViDanhGia == null ? _uow.DonViDanhGias.GetSingle(dv => dv.IdDonViKPI == null && !dv.IsDeleted)
                : _uow.DonViDanhGias.GetById(model.IdDonViDanhGia);
            if (donViDanhGia == null)
                return BadRequest("Đơn vị đánh giá không hợp lệ.");
            var danhMucPI = _uow.DanhMucPIs
            .GetAll(dm => dm.IdDMDonViDanhGia == donViDanhGia.Id)
            .OrderByDescending(dm => dm.PhienBan) // Sắp xếp giảm dần theo phiên bản
            .FirstOrDefault();
            if (model.DanhMucPIChiTiets == null)
                return BadRequest("Danh mục pi chi tiết không tồn tại.");
            int version = 1;
            if (danhMucPI != null)
                version = danhMucPI.PhienBan + 1;
            int count = 0;
            foreach (var item in model.DanhMucPIChiTiets)
            {
                var ChucDanhSet = new HashSet<(Guid ChucDanhId, NhomChucDanh NhomChucDanh)>();
                var DM_KQDG = new HashSet<Guid>();
                count++;
                if (!string.IsNullOrWhiteSpace(item.DuLieuThamDinh) && item.IdNguoiThamDinh == null)
                    return BadRequest("Dữ liệu thẩm định tồn tại thì phải có người thẩm định");
                if (!_uow.DanhMucTrongYeus.Exists(dm => dm.Id == item.IdDMTrongYeu))
                    return BadRequest("ID danh mục trọng yếu không tồn tại");
                if(!_uow.DanhMucNhomPIs.Exists(dm => dm.Id == item.IdNhomPI))
                    return BadRequest("ID danh mục nhóm PI không tồn tại");
                if (item.DoiTuongApDungs != null)
                {
                    foreach (var itemChild in item.DoiTuongApDungs)
                    {
                        if(!_uow.ChucDanhs.Exists(cd => cd.Id == itemChild.ChucDanhId))
                            return BadRequest("ID chức danh không tồn tại");
                        else
                        {
                            if (ChucDanhSet.Contains((itemChild.ChucDanhId, itemChild.NhomChucDanh)))
                                return BadRequest("Đã trùng lặp chức danh");
                            ChucDanhSet.Add((itemChild.ChucDanhId, itemChild.NhomChucDanh));
                            if(ChucDanhSet.Contains((itemChild.ChucDanhId, NhomChucDanh.CTYANDVPTQ)))
                            {
                                if(ChucDanhSet.Contains((itemChild.ChucDanhId, NhomChucDanh.CTY)) ||
                                    ChucDanhSet.Contains((itemChild.ChucDanhId, NhomChucDanh.VPTQ)))
                                    return BadRequest("Chức danh này đã ở cty và VPTQ rồi không thể ở 1 trong 2 loại này nữa");
                            }
                        }
                    }
                }
                if(item.KieuDanhGia == 0)
                {
                    if(item.ChieuHuongTot == null || !Enum.IsDefined(typeof(ChieuHuongTot), item.ChieuHuongTot))
                        return BadRequest("Chiều hướng tốt không nằm trong dãy giá trị cho phép là 0 hoặc 1");
                    if (item.HeSoHoanThanhK == null || !Enum.IsDefined(typeof(HeSoHoanThanhK), item.HeSoHoanThanhK))
                        return BadRequest("Hệ số hoàn thành K không nằm trong dãy giá trị cho phép là 0 hoặc 1");
                    if(!_uow.DonViDos.Exists(dv => dv.Id == item.IdDonViDo))
                        return BadRequest("Id đơn vị đo không tồn tại trong datase");
                }
                else
                {
                    item.ChieuHuongTot = null;
                    item.HeSoHoanThanhK = null;
                    item.IdDonViDo = null;
                }
                if (item.KetQuaDanhGiaDTOs == null)
                {
                    return BadRequest("Kết quả đánh giá không được null");
                }
                else
                {
                   
                    foreach (var itemChild in item.KetQuaDanhGiaDTOs)
                    {
                        if (!_uow.KetQuaDanhGias.Exists(dm => dm.Id == itemChild.IdDMKetQuaDanhGia))
                            return BadRequest("ID danh mục đánh giá không tồn tại");
                        else
                        {
                            if (DM_KQDG.Contains(itemChild.IdDMKetQuaDanhGia))
                                return BadRequest("Đã trùng lặp danh mục kết quả đánh giá");
                            DM_KQDG.Add(itemChild.IdDMKetQuaDanhGia);
                        }
                        if(item.KieuDanhGia == 0)
                        {
                            string pattern = @"^(\d+(?:\.\d+)?\s*(<=|<)\s*)?(K)(\s*(<=|<)\s*\d+(?:\.\d+)?)?$";
                            if (!Regex.IsMatch(itemChild.KhoangGiaTriK, pattern))
                                return BadRequest("Biểu thức không đúng định dạng");
                        }
                    }
                }
                if(item.PIPhuThuocs.GroupBy(x => x.MaSo).Any(g => g.Count() > 1))
                    return BadRequest("Trong list PI con, có 2 item trùng mã");
                if (item.PIPhuThuocs != null)
                {
                    foreach (var itemChild in item.PIPhuThuocs)
                    {
                        count++;
                        if(itemChild.MaSo == item.MaSo)
                            return BadRequest("PI con có mã trùng với pi cha");
                    }
                }
            }
            var IdDMLanhDaoDonVi = Guid.Empty;
            if (model.IdDonViDanhGia != null)
            {
                DM_DonViDanhGia donViDG = _uow.DonViDanhGias.GetSingle(dv => dv.Id == model.IdDonViDanhGia);
                if(donViDG.IdDonViKPI != null)
                    IdDMLanhDaoDonVi = _uow.DanhMucLanhDaoDonVis.GetSingle(dm => dm.DonViID == donViDG.IdDonViKPI).Id;
                if(IdDMLanhDaoDonVi == Guid.Empty)
                    return BadRequest("Đơn vị này chưa có lãnh đạo");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_AddDanhMucPI");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDMDonViDanhGia", donViDanhGia.Id);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@PhienBan", version);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ApDungDen", model.ApDungDen);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@SoLuongPI", count);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDMLanhDaoDonVi", IdDMLanhDaoDonVi == Guid.Empty ? null : IdDMLanhDaoDonVi);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy", Guid.Parse(User.Identity.Name));
            // Lấy giá trị trả về từ stored procedure
            var result = dbAdapter.sqlCommand.ExecuteScalar();

            Guid newDanhMucPIId;
            if (result != null && Guid.TryParse(result.ToString(), out newDanhMucPIId))
            {
                foreach (var item in model.DanhMucPIChiTiets)
                {
                    if(!AddDanhMucPIChiTiet(item, newDanhMucPIId))
                        return BadRequest("Thêm lỗi với item này.");
                }
            }
            else
            {
                if(IdDMLanhDaoDonVi == Guid.Empty)
                    return BadRequest("Thêm thất bại do bạn không có quyền thêm PI dùng chung.");
                return BadRequest("Thêm thất bại hoặc giá trị trả về không hợp lệ.");
            }    
            return Ok("Thêm thành công.");
        }
        private bool AddDanhMucPIChiTiet(DanhMucPIChiTietDTO model, Guid IdDanhMucPI)
        {
            // Convert DoiTuongApDung list to DataTable
            var doiTuongApDungTable = new DataTable();
            doiTuongApDungTable.Columns.Add("NhomChucDanh", typeof(int));
            doiTuongApDungTable.Columns.Add("IdChucDanh", typeof(Guid));

            foreach (var item in model.DoiTuongApDungs)
            {
                doiTuongApDungTable.Rows.Add((int)item.NhomChucDanh, item.ChucDanhId);
            }

            // Convert DanhMucPIChiTietChild list to DataTable
            var danhMucPIChiTietChildTable = new DataTable();
            danhMucPIChiTietChildTable.Columns.Add("MaSo", typeof(string));
            danhMucPIChiTietChildTable.Columns.Add("ChiSoDanhGia", typeof(string));
            danhMucPIChiTietChildTable.Columns.Add("ChiTietChiSoDanhGia", typeof(string));

            foreach (var item in model.PIPhuThuocs)
            {
                danhMucPIChiTietChildTable.Rows.Add(item.MaSo, item.ChiSoDanhGia, item.ChiTietChiSoDanhGia);
            }
            // Create SQL parameters
            SqlParameter doiTuongApDungParam = new()
            {
                ParameterName = "@DoiTuongApDungList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.DoiTuongApDungType", // Đặt đúng tên của TVP trong database
                Value = doiTuongApDungTable
            };
            SqlParameter danhMucPIChiTietChildParam = new SqlParameter
            {
                ParameterName = "@DanhMucPIChiTietChildList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.DanhMucPIChiTietChildType", // Đặt đúng tên của TVP trong database
                Value = danhMucPIChiTietChildTable
            };

            // Convert DanhMucPIChiTietChild list to DataTable
            var ketQuaDanhGiaTable = new DataTable();
            ketQuaDanhGiaTable.Columns.Add("IdDMKetQuaDanhGia", typeof(Guid));
            ketQuaDanhGiaTable.Columns.Add("KhoangGiaTriK", typeof(string));

            foreach (var item in model.KetQuaDanhGiaDTOs)
            {
                ketQuaDanhGiaTable.Rows.Add(item.IdDMKetQuaDanhGia, item.KhoangGiaTriK);
            }
            // Create SQL parameters
            SqlParameter ketQuaDanhGiaParam = new()
            {
                ParameterName = "@KetQuaDanhGiaList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.KetQuaDanhGiaType", // Đặt đúng tên của TVP trong database
                Value = ketQuaDanhGiaTable
            };
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_AddDanhMucPIChiTiet");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDanhMucPI", IdDanhMucPI);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@MaSo", model.MaSo);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdNhomPI", model.IdNhomPI);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDMTrongYeu", model.IdDMTrongYeu);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChiSoDanhGia", model.ChiSoDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DuLieuThamDinh", model.DuLieuThamDinh ?? (object)DBNull.Value);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdNguoiThamDinh", model.IdNguoiThamDinh ?? (object)DBNull.Value);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", (int)model.ChuKy);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChiTietChiSoDanhGia", model.ChiTietChiSoDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@TrangThaiSuDung", model.TrangThaiSuDung);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@KieuDanhGia", (int)model.KieuDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy", Guid.Parse(User.Identity.Name));
            dbAdapter.sqlCommand.Parameters.Add(doiTuongApDungParam);
            dbAdapter.sqlCommand.Parameters.Add(danhMucPIChiTietChildParam);
            dbAdapter.sqlCommand.Parameters.Add(ketQuaDanhGiaParam);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return true;
            return false;
        }
        [HttpPut]
        public IActionResult UpdateDanhMucPI([FromBody] DanhMucPIDTO model)
        {
            if (model == null)
                return BadRequest("Dữ liệu không hợp lệ.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (model.Id == null || _uow.DanhMucPIs.Exists(dm => dm.Id == model.Id && !dm.IsDeleted))
                return BadRequest("Id không tồn tại trong database");
            var donViDanhGia = model.IdDonViDanhGia == null ? _uow.DonViDanhGias.GetSingle(dv => dv.IdDonViKPI == null && !dv.IsDeleted)
                : _uow.DonViDanhGias.GetById(model.IdDonViDanhGia);
            if (donViDanhGia == null)
                return BadRequest("Đơn vị đánh giá không hợp lệ.");
            var danhMucPI = _uow.DanhMucPIs
            .GetAll(dm => dm.IdDMDonViDanhGia == dm.IdDMDonViDanhGia)
            .OrderByDescending(dm => dm.PhienBan) // Sắp xếp giảm dần theo phiên bản
            .FirstOrDefault();
            if (model.DanhMucPIChiTiets == null)
                return BadRequest("Danh mục pi chi tiết không tồn tại.");
            int version = 1;
            if (danhMucPI != null)
                version = danhMucPI.PhienBan + 1;
            int count = 0;
            foreach (var item in model.DanhMucPIChiTiets)
            {
                var ChucDanhSet = new HashSet<(Guid ChucDanhId, NhomChucDanh NhomChucDanh)>();
                var DM_KQDG = new HashSet<Guid>();
                count++;
                if (!string.IsNullOrWhiteSpace(item.DuLieuThamDinh) && item.IdNguoiThamDinh == null)
                    return BadRequest("Dữ liệu thẩm định tồn tại thì phải có người thẩm định");
                if (!_uow.DanhMucTrongYeus.Exists(dm => dm.Id == item.IdDMTrongYeu))
                    return BadRequest("ID danh mục trọng yếu không tồn tại");
                if (!_uow.DanhMucNhomPIs.Exists(dm => dm.Id == item.IdNhomPI))
                    return BadRequest("ID danh mục nhóm PI không tồn tại");
                if (item.DoiTuongApDungs != null)
                {
                    foreach (var itemChild in item.DoiTuongApDungs)
                    {
                        if (!_uow.ChucDanhs.Exists(cd => cd.Id == itemChild.ChucDanhId))
                            return BadRequest("ID chức danh không tồn tại");
                        else
                        {
                            if (ChucDanhSet.Contains((itemChild.ChucDanhId, itemChild.NhomChucDanh)))
                                return BadRequest("Đã trùng lặp chức danh");
                            ChucDanhSet.Add((itemChild.ChucDanhId, itemChild.NhomChucDanh));
                            if (ChucDanhSet.Contains((itemChild.ChucDanhId, NhomChucDanh.CTYANDVPTQ)))
                            {
                                if (ChucDanhSet.Contains((itemChild.ChucDanhId, NhomChucDanh.CTY)) ||
                                    ChucDanhSet.Contains((itemChild.ChucDanhId, NhomChucDanh.VPTQ)))
                                {
                                    return BadRequest("Chức danh này đã ở cty và VPTQ rồi không thể ở 1 trong 2 loại này nữa");
                                }
                            }
                        }
                    }
                }
                if (item.KieuDanhGia == 0)
                {
                    if (item.ChieuHuongTot == null || !Enum.IsDefined(typeof(ChieuHuongTot), item.ChieuHuongTot))
                        return BadRequest("Chiều hướng tốt không nằm trong dãy giá trị cho phép là 0 hoặc 1");
                    if (item.HeSoHoanThanhK == null || !Enum.IsDefined(typeof(HeSoHoanThanhK), item.HeSoHoanThanhK))
                        return BadRequest("Hệ số hoàn thành K không nằm trong dãy giá trị cho phép là 0 hoặc 1");
                    if (!_uow.DonViDos.Exists(dv => dv.Id == item.IdDonViDo))
                        return BadRequest("Id đơn vị đo không tồn tại trong datase");
                }
                else
                {
                    item.ChieuHuongTot = null;
                    item.HeSoHoanThanhK = null;
                    item.IdDonViDo = null;
                }
                if (item.KetQuaDanhGiaDTOs == null)
                {
                    return BadRequest("Kết quả đánh giá không được null");
                }
                else
                {
                    foreach (var itemChild in item.KetQuaDanhGiaDTOs)
                    {
                        if (!_uow.KetQuaDanhGias.Exists(dm => dm.Id == itemChild.IdDMKetQuaDanhGia))
                            return BadRequest("ID danh mục đánh giá không tồn tại");
                        else
                        {
                            if (DM_KQDG.Contains(itemChild.IdDMKetQuaDanhGia))
                                return BadRequest("Đã trùng lặp danh mục kết quả đánh giá");
                            DM_KQDG.Add(itemChild.IdDMKetQuaDanhGia);
                        }
                        if (item.KieuDanhGia == 0 && !float.TryParse(itemChild.KhoangGiaTriK, out float r))
                            return BadRequest("Phải có định dạng số");
                    }
                }
                if (item.PIPhuThuocs.GroupBy(x => x.MaSo).Any(g => g.Count() > 1))
                    return BadRequest("Trong list PI con, có 2 item trùng mã");
                if (item.PIPhuThuocs != null)
                {
                    foreach (var itemChild in item.PIPhuThuocs)
                    {
                        count++;
                        if (itemChild.MaSo == item.MaSo)
                            return BadRequest("PI con có mã trùng với pi cha");
                    }
                }
            }
            var IdDMLanhDaoDonVi = Guid.Empty;
            if (model.IdDonViDanhGia != null)
            {
                DM_DonViDanhGia donViDG = _uow.DonViDanhGias.GetSingle(dv => dv.Id == model.IdDonViDanhGia);
                if (donViDG != null)
                    IdDMLanhDaoDonVi = _uow.DanhMucLanhDaoDonVis.GetSingle(dm => dm.DonViID == donViDG.IdDonViKPI).Id;
                if (IdDMLanhDaoDonVi == Guid.Empty)
                    return BadRequest("Đơn vị này chưa có lãnh đạo");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_UpdateDanhMucPI");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDanhMucPI", model.Id); // IdDanhMucPI cần update
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDMDonViDanhGia", model.IdDonViDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@SoLuongPI", count);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ApDungDen", model.ApDungDen);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy", Guid.Parse(User.Identity.Name));

            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            foreach (var item in model.DanhMucPIChiTiets)
            {
                if (!AddDanhMucPIChiTiet(item, (Guid)model.Id))
                    return BadRequest("thêm lỗi với item này.");
            }
            return Ok("Cập nhật thành công.");
        }
        [HttpDelete]
        public IActionResult DeleteDanhMucPI(Guid id)
        {
            if (id == Guid.Empty || !_uow.DanhMucPIs.Exists(dm => dm.Id == id && !dm.IsDeleted))
                return BadRequest("Id không tồn tại trong database");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteDanhMucPI");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDanhMucPI", id);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Xóa thành công.");
            else
                return BadRequest("Xóa thất bại.");
        }
        [HttpGet]
        public IActionResult GetAll(DateTime? date = null, bool isTong = true, Guid? dMDonViDanhGiaId = null, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucPI");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsTong", isTong);
            dbAdapter.sqlCommand.Parameters.Add("@DonVi", SqlDbType.UniqueIdentifier).Value = dMDonViDanhGiaId;
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.Add("@ApDungDen", SqlDbType.DateTime).Value = date;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name); ;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(new
            {
                jsonObject.totalRow,
                jsonObject.totalPage,
                jsonObject.pageSize,
                jsonObject.dataList
                // Giả sử datalist nằm trong jsonObject
            });
        }
        [HttpGet("get-list-by-Danh-Muc-PI-Id")]
        public IActionResult GetAllDanhMucPIChiTietByIdDanhMucPI(Guid id, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllDanhMucPIChiTietByIdDanhMucPI");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(new
            {
                jsonObject.totalRow,
                jsonObject.totalPage,
                jsonObject.pageSize,
                jsonObject.dataList
                // Giả sử datalist nằm trong jsonObject
            });
        }
        [HttpGet("get-dm-pi-chitiet-byId")]
        public IActionResult GetDanhMucPIChiTietById(Guid id, string maso = null)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucPIChiTietById");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@MaSo", maso);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }

        [HttpGet("get-dm-pi-Tong")]
        public IActionResult GetDanhMucPITong(int page = 1, 
            int pageSize = 10,
            Guid? dMMucTieuTrongYeuId = null,
            ChuKy? chuky = null,
            string keyword = "")
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllDanhMucPITongV2");
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            dbAdapter.sqlCommand.Parameters.Add("@TrongYeu", SqlDbType.UniqueIdentifier).Value = dMMucTieuTrongYeuId;
            dbAdapter.sqlCommand.Parameters.Add("@ChuKy", SqlDbType.Int).Value = chuky;
            dbAdapter.sqlCommand.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            //var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(new
            {
                jsonObject.totalRow,
                jsonObject.totalPage,
                jsonObject.pageSize,
                jsonObject.dataList
                // Giả sử datalist nằm trong jsonObject
            });

        }

        [HttpGet("get-dm-duyet-PI")]
        public IActionResult GetDanhMucDuyetPI(int page = 1, 
            int pageSize = 10,
            Guid? dMDonViDanhGiaId = null
            )
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucDuyetPI");
            //dbAdapter.sqlCommand.Parameters.AddWithValue("@IsTong", isTong);
            dbAdapter.sqlCommand.Parameters.Add("@DonVi", SqlDbType.UniqueIdentifier).Value = dMDonViDanhGiaId;
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(new
            {
                jsonObject.totalRow,
                jsonObject.totalPage,
                jsonObject.pageSize,
                jsonObject.dataList
                // Giả sử datalist nằm trong jsonObject
            });
        }
        [HttpGet("get-Status-Danh-Muc-Duyet-PI")]
        public IActionResult GetStatusDMDuyetPI()
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucDuyetPI_TrangThai");
            //dbAdapter.sqlCommand.Parameters.AddWithValue("@IsTong", isTong);
            //dbAdapter.sqlCommand.Parameters.Add("@DonVi", SqlDbType.UniqueIdentifier).Value = dMDonViDanhGiaId;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpGet("get-Status-Danh-Muc-PI")]
        public IActionResult GetStatusDMPI(bool isTong = true)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucPI_TrangThai");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsTong", isTong);
            //dbAdapter.sqlCommand.Parameters.Add("@DonVi", SqlDbType.UniqueIdentifier).Value = dMDonViDanhGiaId;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpGet("get-Ly-Do-TuCHoi")]
        public IActionResult Get(Guid DanhMucPIId)
        {
            if (DanhMucPIId == Guid.Empty || !_uow.DanhMucPIs.Exists(dm => dm.Id == DanhMucPIId && !dm.IsDeleted))
                return BadRequest("Id không tồn tại trong database");
            var duyetPI = _uow.duyetPIs.GetSingle(dm => dm.DanhMucPIId == DanhMucPIId);
            if(duyetPI != null && duyetPI.IsRefuse == false)
                return BadRequest("Danh Mục này chưa bị từ chối duyệt");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetRefuseDuyetPI");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DanhMucPIId", DanhMucPIId);
            var result = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpGet("Export-pdf")]
        public IActionResult Export(Guid id)
        {
            //if (page < 1) page = 1;
            //if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllDanhMucPITongByIdExport");
            dbAdapter.sqlCommand.Parameters.Add("@DanhMucPIId", SqlDbType.UniqueIdentifier).Value = id;
            var result = dbAdapter.runStored2JSON();
            var jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            var templatePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "Export_Danh_muc_PI.xlsx");
            if (!System.IO.File.Exists(templatePath))
                return NotFound(new { message = "File mẫu không tồn tại." });
            dbAdapter.createStoredProceder("sp_GetDanhMucPiCauHinhDuyetById");
            dbAdapter.sqlCommand.Parameters.Add("@IdDanhMucPI", SqlDbType.UniqueIdentifier).Value = id;
            List<dynamic> cauHinhDuyetResult = dbAdapter.runStored2ObjectList();
            dbAdapter.createStoredProceder("sp_GetAllDM_KetQuaDanhGia");
            List<dynamic> resultDM_KetQuaDanhGia = dbAdapter.runStored2ObjectList();

            dbAdapter.createStoredProceder("sp_GetNameDonVIDanhGiaAndApDungDen");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dynamic resultNameDVDG = dbAdapter.runStored2Object();
            string nameDVDG = resultNameDVDG.DonVi.ToString().ToUpperInvariant();

            // Gọi stored procedure để lấy danh sách đối tượng áp dụng
            dbAdapter.createStoredProceder("sp_GetDoiTuongApDungByDanhMucPIId");
            dbAdapter.sqlCommand.Parameters.Add("@IdDanhMucPI", SqlDbType.UniqueIdentifier).Value = id;
            List<dynamic> doiTuongApDungResult = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            using var package = new ExcelPackage(new FileInfo(templatePath));
            var worksheet = package.Workbook.Worksheets["Danh mục PI"]; // Giả sử dữ liệu sẽ điền vào sheet đầu tiên

            var cell = worksheet.Cells["B2"];
            string originalText = cell.Text;  // Giả sử nội dung ô ban đầu là "Núi Thành, ngày … tháng ... năm ..."
            string updatedText = originalText.Replace("...", nameDVDG);
            cell.Value = updatedText;

            cell = worksheet.Cells["A3"];
            originalText = cell.Text;
            updatedText = originalText.Replace("...", nameDVDG);
            cell.Value = updatedText;
            string apDungDen = resultNameDVDG.apDungDen.ToString("MM/yyyy");
            cell = worksheet.Cells["G3"];
            originalText = cell.Text;
            updatedText = originalText.Replace(",,,", apDungDen);
            cell.Value = updatedText;
            int rowIndexFullName = 15;
            int rowIndexCapDuyet = rowIndexFullName - 3; // Hàng để ghi item.capDuyet
                                                         // Hàng để ghi item.fullName (sau 2 hàng trống)
            int currentColumn = 2; // Bắt đầu ghi vào cột đầu tiên
            foreach (var item in cauHinhDuyetResult)
            {
                // Ghi item.capDuyet vào hàng đầu tiên của cột hiện tại
                worksheet.Cells[rowIndexCapDuyet, currentColumn].Value = item.capDuyet.ToString().ToUpper();

                // Ghi item.fullName vào hàng thứ 4 của cùng cột
                worksheet.Cells[rowIndexFullName, currentColumn].Value = item.fullName;

                // Merge ô nếu cần (nếu bạn muốn merge thêm cột)
                worksheet.Cells[rowIndexCapDuyet, currentColumn].Merge = false;
                worksheet.Cells[rowIndexFullName, currentColumn].Merge = false;

                // Căn giữa dữ liệu
                worksheet.Cells[rowIndexCapDuyet, currentColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndexFullName, currentColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Thiết lập kiểu chữ in đậm
                worksheet.Cells[rowIndexCapDuyet, currentColumn].Style.Font.Bold = true;
                worksheet.Cells[rowIndexFullName, currentColumn].Style.Font.Bold = true;

                // Tăng currentColumn để chuyển sang cột tiếp theo cho vòng lặp kế tiếp
                currentColumn += 4; // Cộng thêm 3 cột (1 cột cho dữ liệu, 2 cột trống)
            }
            cell = worksheet.Cells["I11"];
            originalText = cell.Text;  // Giả sử nội dung ô ban đầu là "Núi Thành, ngày … tháng ... năm ..."

            // Lấy ngày tháng hiện tại
            DateTime now = DateTime.Now;
            string currentDate = now.Day.ToString();
            string currentMonth = now.Month.ToString();
            string currentYear = now.Year.ToString();

            // Thay thế chuỗi để điền ngày tháng hiện tại vào ô
            updatedText = originalText.Replace("...", currentDate)
                                                .Replace("..", currentMonth)
                                                .Replace(",,,,", currentYear);
            cell.Value = updatedText;
            
            int rowToInsert = 9;
            // Lưu định dạng và viền từ dòng mẫu (dòng 9)
            var rangeToCopy = worksheet.Cells[9, 1, 9, worksheet.Dimension.End.Column];
            // Chèn các dòng mới
            var mergedCells = new List<string>();
            foreach (var mergedCell in worksheet.MergedCells)
            {
                var sourceRange = worksheet.Cells[mergedCell];
                if (sourceRange.Start.Row == 9)  // Chỉ lưu các ô gộp trong dòng mẫu
                {
                    mergedCells.Add(mergedCell); // Lưu địa chỉ ô gộp vào danh sách
                }
            }
            worksheet.InsertRow(rowToInsert, jsonObject.Count);

            for (int i = 0; i < jsonObject.Count; i++)
            {
                // Sao chép định dạng từ dòng mẫu đã lưu trước đó
                rangeToCopy.Copy(worksheet.Cells[rowToInsert + i, 1, rowToInsert + i, worksheet.Dimension.End.Column]);

                // Sao chép viền cho các ô trong dòng mới
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var sourceCell = worksheet.Cells[9 + jsonObject.Count, col]; // Ô nguồn từ dòng mẫu
                    var targetCell = worksheet.Cells[rowToInsert + i, col]; // Ô đích

                    // Sao chép kiểu viền từ ô mẫu sang ô đích
                    targetCell.Style.Border.Top.Style = sourceCell.Style.Border.Top.Style;
                    targetCell.Style.Border.Bottom.Style = sourceCell.Style.Border.Bottom.Style;
                    targetCell.Style.Border.Left.Style = sourceCell.Style.Border.Left.Style;
                    targetCell.Style.Border.Right.Style = sourceCell.Style.Border.Right.Style;
                    // Nếu bạn cần sao chép màu viền, bạn có thể thêm đoạn mã sau:
                }

                // Sao chép các ô gộp từ dòng 9 sang dòng mới
                foreach (var mergedCell in mergedCells)
                {
                    if (mergedCell == null) // Kiểm tra mergedCell có null hay không
    {
        continue; // Bỏ qua các ô null
    }
                    var sourceRange = worksheet.Cells[mergedCell];
                    string newMergedCellAddress = worksheet.Cells[rowToInsert + i, sourceRange.Start.Column,
                        rowToInsert + i, sourceRange.End.Column].Address;
                    worksheet.Cells[newMergedCellAddress].Merge = true; // Gộp ô cho dòng mới
                }
            }
            
            worksheet.DeleteRow(9 + jsonObject.Count);
            rowToInsert = 5;
            // Lưu định dạng và viền từ dòng mẫu (dòng 9)
            rangeToCopy = worksheet.Cells[5, 1, 5, 6];
            // Chèn các dòng mới
            worksheet.InsertRow(rowToInsert, doiTuongApDungResult.Count);
            for (int i = 0; i < doiTuongApDungResult.Count; i++)
            {
                // Sao chép định dạng từ dòng mẫu đã lưu trước đó
                rangeToCopy.Copy(worksheet.Cells[rowToInsert + i, 1, rowToInsert + i, 6]);

                // Sao chép viền cho các ô trong dòng mới
                for (int col = 1; col <= 6; col++)
                {
                    var sourceCell = worksheet.Cells[5 + doiTuongApDungResult.Count, col]; // Ô nguồn từ dòng mẫu
                    var targetCell = worksheet.Cells[rowToInsert + i, col]; // Ô đích

                    // Sao chép kiểu viền từ ô mẫu sang ô đích
                    targetCell.Style.Border.Top.Style = sourceCell.Style.Border.Top.Style;
                    targetCell.Style.Border.Bottom.Style = sourceCell.Style.Border.Bottom.Style;
                    targetCell.Style.Border.Left.Style = sourceCell.Style.Border.Left.Style;
                    targetCell.Style.Border.Right.Style = sourceCell.Style.Border.Right.Style;
                    // Nếu bạn cần sao chép màu viền, bạn có thể thêm đoạn mã sau:
                }

                // Sao chép các ô gộp từ dòng 9 sang dòng mới
                //foreach (var mergedCell in worksheet.MergedCells)
                //{
                //    var sourceRange = worksheet.Cells[mergedCell];

                //    // Kiểm tra xem dòng mẫu có phải là dòng chứa ô gộp không
                //    if (sourceRange.Start.Row == 5)
                //    {
                //        string newMergedCellAddress = worksheet.Cells[rowToInsert + i, sourceRange.Start.Column,
                //            rowToInsert + i, sourceRange.End.Column].Address;
                //        worksheet.Cells[newMergedCellAddress].Merge = true;
                //    }
                //}
            }
            int rowDtStart = 5;
            int stt = 0;
            foreach (var doituong in doiTuongApDungResult)
            {
                string tenChucDanh = (string)doituong.TenChucDanh;
                string abbreviation = new(
                        tenChucDanh
                            .Split(' ')                          // Tách các từ dựa trên khoảng trắng
                            .Where(word => !string.IsNullOrEmpty(word))  // Loại bỏ từ rỗng
                            .Select(word => word[0])             // Lấy ký tự đầu tiên của mỗi từ
                            .Select(c => char.ToUpper(c))        // Chuyển ký tự đầu thành chữ hoa
                            .ToArray()                           // Chuyển về mảng char để tạo chuỗi
                    );
                worksheet.Cells[rowDtStart, 1].Value = (stt + 1).ToString();
                worksheet.Cells[rowDtStart, 2].Value = doituong.TenChucDanh;
                worksheet.Cells[rowDtStart, 3].Value = abbreviation;
                int nhomChucDanh = (int)doituong.NhomChucDanh;
                switch (nhomChucDanh)
                {
                    case 2:
                        worksheet.Cells[rowDtStart, 4].Value = "x";
                        worksheet.Cells[rowDtStart, 5].Value = "x";
                        break;
                    case 1:
                        worksheet.Cells[rowDtStart, 4].Value = "x";
                        worksheet.Cells[rowDtStart, 5].Value = "-";
                        break;
                    case 0:
                        worksheet.Cells[rowDtStart, 4].Value = "-";
                        worksheet.Cells[rowDtStart, 5].Value = "x";
                        break;
                }
                rowDtStart++;
                stt++;
            }
            worksheet.DeleteRow(5 + doiTuongApDungResult.Count);

            int rowIndex = 8 + doiTuongApDungResult.Count;
            if (resultDM_KetQuaDanhGia.Count > 5)
            {
                int columnsToInsert = resultDM_KetQuaDanhGia.Count - 5;
                worksheet.InsertColumn(9, columnsToInsert);

                for (int i = 0; i < columnsToInsert; i++)
                {
                    int newColumnIndex = 9 + i;

                    // Sao chép định dạng của toàn bộ cột 8 sang cột mới
                    for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                    {
                        // Sao chép toàn bộ style từ ô mẫu (dòng 8, cột 8) sang ô mới (dòng row, cột mới)
                        worksheet.Cells[row, newColumnIndex].StyleID = worksheet.Cells[row, 8].StyleID;

                        // Sao chép viền từ cột mẫu
                        worksheet.Cells[row, newColumnIndex].Style.Border.Top.Style = worksheet.Cells[row, 8].Style.Border.Top.Style;
                        worksheet.Cells[row, newColumnIndex].Style.Border.Bottom.Style = worksheet.Cells[row, 8].Style.Border.Bottom.Style;
                        worksheet.Cells[row, newColumnIndex].Style.Border.Left.Style = worksheet.Cells[row, 8].Style.Border.Left.Style;
                        worksheet.Cells[row, newColumnIndex].Style.Border.Right.Style = worksheet.Cells[row, 8].Style.Border.Right.Style;
                    }
                    worksheet.Column(newColumnIndex).Width = worksheet.Column(8).Width;
                }
            }
            else
            {
                if(resultDM_KetQuaDanhGia.Count < 5)
                {
                    for(int i = 1; i < 5 - resultDM_KetQuaDanhGia.Count; i++)
                        worksheet.DeleteColumn(11 - i);
                }
            }
            int columStart = 7;
            foreach (var ketqua in resultDM_KetQuaDanhGia)
            {
                worksheet.Cells[rowIndex - 1, columStart].Value = ketqua.ketQuaDanhGia;
                columStart++;
            }
            foreach (var item in jsonObject)
            {
                worksheet.Cells[rowIndex, 1].Value = item.maSo.ToString(); // STT
                worksheet.Cells[rowIndex, 2].Value = item.chiSoDanhGia.ToString(); // Mã nhân viên
                var doiTuongApDungList = JsonConvert.DeserializeObject<List<dynamic>>(item.doiTuongApDungJson.ToString());
                StringBuilder vPDH = new();
                StringBuilder tHCTY = new();
                foreach (var chucdanh in doiTuongApDungList)
                {
                    string tenChucDanh = (string)chucdanh.tenChucDanh;
                    string abbreviation = new(
                            tenChucDanh
                                .Split(' ')                          // Tách các từ dựa trên khoảng trắng
                                .Where(word => !string.IsNullOrEmpty(word))  // Loại bỏ từ rỗng
                                .Select(word => word[0])             // Lấy ký tự đầu tiên của mỗi từ
                                .Select(c => char.ToUpper(c))        // Chuyển ký tự đầu thành chữ hoa
                                .ToArray()                           // Chuyển về mảng char để tạo chuỗi
                        );
                    if (chucdanh.nhomChucDanh == 0 || chucdanh.nhomChucDanh == 2)
                        tHCTY.Append(tHCTY.Length == 0 ? abbreviation : ", " + abbreviation);
                    if (chucdanh.nhomChucDanh == 1 || chucdanh.nhomChucDanh == 2)
                        vPDH.Append(vPDH.Length == 0 ? abbreviation : ", " + abbreviation);
                }
                worksheet.Cells[rowIndex, 4].Value = vPDH?.ToString(); // Họ và tên
                worksheet.Cells[rowIndex, 5].Value = tHCTY?.ToString(); // Chu kỳ đánh giá


                worksheet.Cells[rowIndex, 6].Value = item.chiTietChiSoDanhGia.ToString(); // Thời điểm đánh giá
                var ketQuaDanhGia = JsonConvert.DeserializeObject<List<dynamic>>(item.ketQuaDanhGiaJson.ToString());
                int countKQ = resultDM_KetQuaDanhGia.Count;
                columStart = 7;
                foreach (var ketqua in ketQuaDanhGia)
                {
                    if(ketqua.thuTu == countKQ)
                        worksheet.Cells[rowIndex, columStart].Value = ketqua.khoangGiaTriK.ToString();
                    countKQ--;
                    columStart++;
                }
                rowIndex++;
            }
            int col1 = 2; // Cột đầu tiên có dữ liệu nhiều
            int col2 = 6; // Cột thứ hai có dữ liệu nhiều
            for (int row = rowIndex - jsonObject.Count; row <= worksheet.Dimension.End.Row + jsonObject.Count; row++)
            {
                double maxHeight = 0;

                // Lấy chiều cao tối đa từ hai cột
                for (int col = col1; col <= col2; col++)
                {
                    var cellValue = worksheet.Cells[row, col].Text;
                    double height = Math.Ceiling(cellValue.Length / 45.0) * (15 + 5);
                    // Cập nhật chiều cao tối đa
                    if (height > maxHeight)
                        maxHeight = height;
                }

                // Đặt chiều cao hàng là chiều cao tối đa đã tính toán
                worksheet.Row(row).Height = maxHeight + 5; // Thêm khoảng trống nếu cần
            }
            worksheet.Row(11 + doiTuongApDungResult.Count + jsonObject.Count - 2).Height = 20;
            dbAdapter.deConnect();
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;  // Reset stream position for reading
            //stream.Position = 0;

            //// Return as file download
            //string fileName = "FilledTemplate.xlsx";
            //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            // Step 2: Use Spire.XLS to convert the Excel to PDF
            Workbook workbook = new();
            workbook.LoadFromStream(stream); // Load the stream containing Excel data

            // Create a memory stream for PDF output
            var pdfStream = new MemoryStream();
            workbook.SaveToStream(pdfStream, Spire.Xls.FileFormat.PDF); // Convert and save as PDF

            // Step 3: Return the PDF file to the user
            pdfStream.Position = 0; // Reset stream position for reading
            var pdfContent = pdfStream.ToArray(); // Convert to byte array

            return File(pdfContent, "application/pdf", "Export_Danh_muc_PI.pdf"); // Return PDF file
        }

        [HttpPut("Duyet-PI")]
        public IActionResult DuyetPI(Guid IdDanhMucPI)
        {
            var duyetPI = _uow.duyetPIs.GetSingle(d => d.DanhMucPIId == IdDanhMucPI);
            if (duyetPI == null || duyetPI.IsComplete || duyetPI.IsRefuse)
                return BadRequest("Danh mục này đã duyệt hoặc đã bị từ chối");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DuyetDanhMucPI");
            dbAdapter.sqlCommand.Parameters.Add("@IdDanhMucPI", SqlDbType.UniqueIdentifier).Value = IdDanhMucPI;
            dbAdapter.sqlCommand.Parameters.Add("@Updatedby", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Duyệt thành công");
            return BadRequest("Người dùng không có quyền duyệt PI này");
        }
        [HttpPut("Refuse-PI")]
        public IActionResult RefusePI(Guid IdDanhMucPI, string LyDoTuChoi)
        {
            var duyetPI = _uow.duyetPIs.GetSingle(d => d.DanhMucPIId == IdDanhMucPI);
            if (duyetPI == null || duyetPI.IsComplete || duyetPI.IsRefuse)
                return BadRequest("Danh mục này đã duyệt hoặc đã bị từ chối");
            if (LyDoTuChoi == null)
                return BadRequest("Lý do từ chối là bắt buộc");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_RefuseDanhMucPI");
            dbAdapter.sqlCommand.Parameters.Add("@IdDanhMucPI", SqlDbType.UniqueIdentifier).Value = IdDanhMucPI;
            dbAdapter.sqlCommand.Parameters.Add("@Updatedby", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@WhyForRefuse", LyDoTuChoi);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Từ chối thành công");
            return BadRequest("Người dùng không có quyền từ chối PI này");
        }


        [HttpPost("xuatExcel-DuyetPI")]
        public IActionResult XuatExcel()
        {
            // Call the GetAllByKeyword function to get the list of data
            //int page = 0;
            //int pageSize = 0;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetDanhMucDuyetPI");
            //dbAdapter.sqlCommand.Parameters.Add("@DonVi", SqlDbType.UniqueIdentifier).Value = null;
            //dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            //dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2JSON();
            var resultJson = JsonConvert.DeserializeObject<dynamic>(result.ToString());
            if (resultJson == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(resultJson.dataList.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachNhanVien");

                // Thiết lập tiêu đề bảng
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Đơn Vị";
                worksheet.Cells[1, 3].Value = "Phiên Bản";
                worksheet.Cells[1, 4].Value = "Thơi gian tạo";
                worksheet.Cells[1, 5].Value = "Áp dụng đến";
                worksheet.Cells[1, 6].Value = "Trạng thái";

                // Tạo viền cho header
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 102, 204)); // Màu xanh header
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White); // Màu chữ trắng
                }

                worksheet.Column(1).Width = 10; // Cột STT
                worksheet.Column(2).Width = 25; // Cột Mã nhân viên
                worksheet.Column(3).Width = 35; // Cột Họ và tên
                worksheet.Column(4).Width = 20; // Cột Chu kỳ đánh giá
                worksheet.Column(5).Width = 20; // Cột Thời điểm đánh giá
                worksheet.Column(6).Width = 25;
                // Điền dữ liệu vào các hàng bắt đầu từ hàng 2
                int rowIndex = 2;
                for (int i = 0; i < jsonObject.Count; i++)
                {
                    // Use dynamic access since the list is of type object
                    var record = jsonObject[i];

                    worksheet.Cells[rowIndex, 1].Value = (i + 1).ToString(); // STT
                    worksheet.Cells[rowIndex, 2].Value = record.donVi != null ? record.donVi.ToString() : ""; // Đơn vị
                    worksheet.Cells[rowIndex, 3].Value = record.phienBan != null ? record.phienBan.ToString() : ""; // Phiên bản
                    worksheet.Cells[rowIndex, 4].Value = record.createdDate != null ? record.createdDate.ToString() : ""; // Thời gian tạo
                    worksheet.Cells[rowIndex, 5].Value = record.apDungDen != null ? record.apDungDen.ToString() : ""; // Áp dụng đến
                    worksheet.Cells[rowIndex, 6].Value = record.trangThai != null ? record.trangThai.ToString() : ""; // Trạng thái

                    using (var range = worksheet.Cells[rowIndex, 1, rowIndex, 6])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    rowIndex++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Trả về file Excel
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhMucDuyetPI.xlsx");
            }
        }
        [HttpGet("get-Danh-Muc-PI-Chi-tiet-Version-Newest")]
        public IActionResult GetVersionNewest()
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetAllPIDetailNewest");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            if (result == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(result);
        }
        [HttpGet("get-List-PI-phu-thuoc-by-ID-DMPI-Detail")]
        public IActionResult GetPIPhuThuoc(Guid PIChiTietId)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetPIPhuThuoc");
            dbAdapter.sqlCommand.Parameters.Add("@PIChiTietId", SqlDbType.UniqueIdentifier).Value = PIChiTietId;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
    }
}
