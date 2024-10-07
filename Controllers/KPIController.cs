using ERP.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ERP.Data.MyDbContext;
using ThacoLibs;
using ERP.Models;
using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2010.Excel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Collections.Generic;
using ERP.Repositories;
using System.Linq.Expressions;
using ERP.Models.ChiTieuKPI;
using NuGet.Protocol.Core.Types;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Drawing;
using ERP.Models.DanhMuc;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KPIController : ControllerBase
    {
        private readonly IUnitofWork _uow;
        private readonly DbAdapter dbAdapter;
        private readonly UserManager<ApplicationUser> _userManager;

        public KPIController(IConfiguration _configuration,
            UserManager<ApplicationUser> userManager,
            IUnitofWork unitof)
        {
            _userManager = userManager;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
            _uow = unitof;
        }

        private IActionResult ValidateDTO(KPIDTO model,bool isUpdate = false)
        {
            var exists = false;
            if (!isUpdate)
            {
                if (model.IsCaNhan)
                {
                    foreach (var item in model.User_Ids)
                    {
                        exists = _uow.KPIs
                    .Any(kpi => kpi.ThoiDiemDanhGia == model.ThoiDiemDanhGia && !kpi.IsDeleted && kpi.UserId == item);
                        if (exists)
                            break;
                    }
                }
                else
                    exists = _uow.KPIs
                   .Any(kpi => kpi.ThoiDiemDanhGia == model.ThoiDiemDanhGia && !kpi.IsDeleted && kpi.DM_DonViDanhGiaId == model.DMDonViDanhGiaId);
            }
            else
            {
                if (model.IsCaNhan)
                {
                    foreach (var item in model.User_Ids)
                    {
                        exists = _uow.KPIs
                    .Any(kpi => kpi.ThoiDiemDanhGia == model.ThoiDiemDanhGia && !kpi.IsDeleted && kpi.UserId == item && kpi.Id != model.Id);
                        if (exists)
                            break;
                    }
                }
                else
                    exists = _uow.KPIs
                   .Any(kpi => kpi.ThoiDiemDanhGia == model.ThoiDiemDanhGia && !kpi.IsDeleted && kpi.DM_DonViDanhGiaId == model.DMDonViDanhGiaId && kpi.Id != model.Id);
            }
            if (exists)
                return BadRequest("Thời điểm đánh giá đã tồn t trong cơ sở dữ liệu.");
            if (model == null)
                return BadRequest("Dữ liệu không hợp lệ.");
            if (!model.IsGiaoHangLoat && model.User_Ids.Count > 1)
                return BadRequest("Không phải màn hình giao hàng loạt chỉ giao 1 user duy nhất");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (model.IsCaNhan && (model.User_Ids == null || model.User_Ids.Count == 0))
                return BadRequest("Phải có ít nhất một cá nhân được giao KPI");
            if (!model.IsCaNhan && (model.DMDonViDanhGiaId == null || model.DMDonViDanhGiaId == Guid.Empty))
                return BadRequest("Phải có danh mục đơn vị đánh giá được giao KPI");
            switch (model.ChuKy)
            {
                case ChuKy.THANG:
                    string[] parts = model.ThoiDiemDanhGia.Split('/');
                    int month;
                    int year;
                    if (model.ThoiDiemDanhGia == null || !int.TryParse(parts[0], out month) || !int.TryParse(parts[1], out year))
                        return BadRequest("Thời điểmm đánh giá không hợp lệ (phải theo định dạng MM/yyyy);");
                    else if (model.ThoiDiemDanhGia == null || month < 1 || month > 12 || year < 1900)
                        return BadRequest("Thời điểm đánh giá tháng không hợp lệ (phải từ 1 đến 12) và year > 1900;");
                    break;
                case ChuKy.NAM:
                    if (model.ThoiDiemDanhGia == null || !int.TryParse(model.ThoiDiemDanhGia, out year) || year < 1900)
                        return BadRequest("Thời điểm đánh giá có format không hợp lệ (phải có định dạng năm và > 1900");
                    break;
                default:
                    return BadRequest("Chu Kỳ đánh giá có định dạng không hợp lệ phải là O(Tháng) hoặc 1(Năm);");
            }
            foreach (var item in model.KPIChiTietDTOs)
            {
                var danhMucPICT = _uow.DanhMucPIChiTiets.GetSingle(dm => dm.Id == item.DMPIChiTietId && !dm.IsDeleted);
                if (item.DMPIChiTietId == Guid.Empty)
                    return BadRequest("ID Danh Mục PI chi tiết phải hợp lệ");
                if (danhMucPICT == null)
                    return BadRequest("ID Danh Mục PI chi tiết không tồn tại");
                else
                {
                    if (danhMucPICT.KieuDanhGia == KieuDanhGia.SO && item.ChiTieuCanDat == null)
                        return BadRequest("PI này là kiểu đánh giá số, vui lòng nhập chỉ tiêu cần đạt");
                    if (danhMucPICT.KieuDanhGia == KieuDanhGia.NOIDUNG && item.DienGiai == null)
                        return BadRequest("PI này là kiểu đánh giá nội dung, vui lòng nhập diễn giải");
                    if (item.KPIChiTietChildDTOs != null || item.KPIChiTietChildDTOs.Count > 0)
                    {
                        foreach (var itemChild in item.KPIChiTietChildDTOs)
                        {
                            if (!_uow.PiPhuThuocs.Exists(p => p.Id == itemChild.PIPhuThuocId))
                                return BadRequest("Pi Phụ thuộc này không tồn tại trong database");
                            if (danhMucPICT.KieuDanhGia == KieuDanhGia.SO && itemChild.ChiTieuCanDat == null)
                                return BadRequest("PI này là kiểu đánh giá số, vui lòng nhập chỉ tiêu cần đạt");
                            if (danhMucPICT.KieuDanhGia == KieuDanhGia.NOIDUNG && itemChild.DienGiai == null)
                                return BadRequest("PI này là kiểu đánh giá nội dung, vui lòng nhập diễn giải");
                        }
                    }
                }
                if (item.TyTrong < 5 || item.TyTrong > 100 || item.TyTrong % 5 != 0)
                    return BadRequest("Tỷ trong phải chia hết cho 5 và không bé hơn 5 và lớn hơn 100");
                string validateTyTrong = ValidateTotalTyTrong(model);
                if (validateTyTrong != null)
                    return BadRequest(validateTyTrong);

            }
            if (model.IsCaNhan)
            {
                foreach (var item in model.User_Ids)
                {
                    if (!_uow.Users.Exists(u => u.Id == item))
                        return BadRequest("Id User này không tồn tại");
                    var existingKPI = _uow.DanhMucDuyets.GetSingle(
                            x => x.NhanVienId == item,
                    new string[] { "CapDuyets" });
                    if(existingKPI != null)
                    {
                        if(existingKPI.CapDuyets.Count == 3)
                        {
                            if (model.KPIChiTietDTOs.Count > 12)
                                return BadRequest("Số lượng chỉ tiêu KPI không được quá 12 chỉ tiêu");
                        }
                        else
                        {
                            if (model.KPIChiTietDTOs.Count > 8)
                                return BadRequest("Số lượng chỉ tiêu KPI không được quá 8 chỉ tiêu");
                        }

                    }
                }
            }
            else
            {
                if (!_uow.DanhMucLanhDaoDonVis.Exists(dm => dm.LanhDaoId == Guid.Parse(User.Identity.Name)))
                    return BadRequest("Id User này không phải là lãnh đạo đơn vị");
                if (model.KPIChiTietDTOs.Count > 12)
                    return BadRequest("Số lượng chỉ tiêu KPI không được quá 12 chỉ tiêu");
            }
            return null;
        }

        [HttpPost]
        public IActionResult Add([FromBody] KPIDTO model)
        {
            var isValidate = ValidateDTO(model);
            if (isValidate != null)
                return isValidate;
            if (model.User_Ids.Count == 1 || model.IsCaNhan == false)
            {
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_AddKPI");
                dbAdapter.sqlCommand.Parameters.AddWithValue("@IsIndividual", model.IsCaNhan);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiemDanhGia", model.ThoiDiemDanhGia);
                if (model.IsCaNhan)
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", model.User_Ids.First());
                else
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDMDonViDanhGia", model.DMDonViDanhGiaId);
                dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy", Guid.Parse(User.Identity.Name));
                // Lấy giá trị trả về từ stored procedure
                var result = dbAdapter.sqlCommand.ExecuteScalar();
                dbAdapter.deConnect();
                if (result != null && Guid.TryParse(result.ToString(), out Guid newKPIId))
                {
                    foreach (var item in model.KPIChiTietDTOs)
                    {
                        if (!AddKPIChiTiet(item, newKPIId))
                            return BadRequest("Thêm lỗi với item này: " + item.DMPIChiTietId);
                    }
                }
                else
                    return BadRequest("Thêm thất bại hoặc giá trị trả về không hợp lệ.");

                if(model.IsCaNhan == false && _uow.DanhMucLanhDaoDonVis
                .GetAll(dm => dm.LanhDaoId == Guid.Parse(User.Identity.Name) && !dm.IsDeleted) // Chỉ lấy những bản ghi không bị xóa
                .Select(dm => dm.DonViID) // Chọn DonViId
                .Distinct() // Lấy các DonViId khác nhau
                .Count() == 1)
                {
                    dbAdapter.connect();
                    dbAdapter.createStoredProceder("sp_AddKPI");
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@IsIndividual", model.IsCaNhan);
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiemDanhGia", model.ThoiDiemDanhGia);
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy", Guid.Parse(User.Identity.Name));
                    // Lấy giá trị trả về từ stored procedure
                    result = dbAdapter.sqlCommand.ExecuteScalar();
                    dbAdapter.deConnect();
                    if (result != null && Guid.TryParse(result.ToString(), out newKPIId))
                    {
                        foreach (var item in model.KPIChiTietDTOs)
                        {
                            if (!AddKPIChiTiet(item, newKPIId))
                                return BadRequest("Thêm lỗi với item này: " + item.DMPIChiTietId);
                        }
                    }
                    else
                        return BadRequest("Thêm thất bại hoặc giá trị trả về không hợp lệ.");
                }

            }
            if (model.IsGiaoHangLoat && model.User_Ids.Count > 1)
            {
                foreach(var userId in model.User_Ids)
                {
                    dbAdapter.connect();
                    dbAdapter.createStoredProceder("sp_AddKPI");
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@IsIndividual", model.IsCaNhan);
                    if (model.IsCaNhan)
                    {
                        dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", userId);
                        dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy",userId);
                    }
                    else
                    {
                        dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDMDonViDanhGia", model.DMDonViDanhGiaId);
                        dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy", Guid.Parse(User.Identity.Name));
                    }
                    dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiemDanhGia", model.ThoiDiemDanhGia);
                    // Lấy giá trị trả về từ stored procedure
                    var result = dbAdapter.sqlCommand.ExecuteScalar();
                    dbAdapter.deConnect();
                    if (result != null && Guid.TryParse(result.ToString(), out Guid newKPIId))
                    {
                        foreach (var item in model.KPIChiTietDTOs)
                        {
                            if (!AddKPIChiTiet(item, newKPIId))
                                return BadRequest("Thêm lỗi với item này: " + item.DMPIChiTietId);
                        }
                    }
                    else
                        return BadRequest("Thêm thất bại hoặc giá trị trả về không hợp lệ.");
                }
            }
            return Ok("Thêm thành công.");

        }
        private bool AddKPIChiTiet(KPIChiTietDTO model, Guid newKPIId)
        {
            // Convert DoiTuongApDung list to DataTable
            var KPIChiTietChildTable = new DataTable();
            KPIChiTietChildTable.Columns.Add("PIPhuThuocId", typeof(Guid));
            KPIChiTietChildTable.Columns.Add("TyTrong", typeof(byte));
            KPIChiTietChildTable.Columns.Add("ChiTieuCanDat", typeof(float));
            KPIChiTietChildTable.Columns.Add("DienGiai", typeof(string));

            foreach (var item in model.KPIChiTietChildDTOs)
            {
                KPIChiTietChildTable.Rows.Add(item.PIPhuThuocId, (byte)item.TyTrong, (float)item.ChiTieuCanDat, item.DienGiai);
            }

            // Convert DanhMucPIChiTietChild list to DataTable
            SqlParameter kPIChiTietChildParam = new()
            {
                ParameterName = "@KPIChiTietChildList",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.KPIChiTietChildType", // Đặt đúng tên của TVP trong database
                Value = KPIChiTietChildTable
            };
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_AddKPIChiTiet");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdKPI", newKPIId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DanhMucPiChiTietId", model.DMPIChiTietId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@TyTrong", model.TyTrong);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChiTieuCanDat", model.ChiTieuCanDat);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DienGiai", model.DienGiai);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsAddChiTieuNam", model.IsAddChiTieuNam);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@CreatedBy", Guid.Parse(User.Identity.Name));
            dbAdapter.sqlCommand.Parameters.Add(kPIChiTietChildParam);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return true;
            return false;
        }
        private string ValidateTotalTyTrong(KPIDTO kpiDto)
        {
            float totalTyTrong = 0;
            foreach (var kpiChiTiet in kpiDto.KPIChiTietDTOs)
            {
                if (kpiChiTiet != null)
                {
                    totalTyTrong += kpiChiTiet.TyTrong;
                    if (kpiChiTiet.KPIChiTietChildDTOs != null)
                    {
                        foreach (var kpiChild in kpiChiTiet.KPIChiTietChildDTOs)
                        {
                            if (kpiChild.TyTrong < 5 || kpiChild.TyTrong > 100 || kpiChild.TyTrong % 5 != 0)
                                return "Tỷ trọng phải chia hết cho 5 và không bé hơn 5 và lớn hơn 100";
                            totalTyTrong += kpiChild.TyTrong;
                        }
                    }
                }
            }
            if(totalTyTrong != 100)
                return "Tổng tỷ trọng phải là 100";
            return null;
        }

        [HttpPut("Infor/Get-Status-CaNhan-Duyet")]
        public IActionResult GetStatusKPICaNhan(bool IsCaNhan = true)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPT_CANHAN_STATUS");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpGet("Infor/get-KPI-CaNhans")]
        public IActionResult GetAllKPICaNhans(int? chuky = null, string? ThoiDiemDanhGia = null, Guid? DonViKPIId = null, string keyword = "",  int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPICaNhans");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViDanhGiaId", DonViKPIId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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

        [HttpGet("Infor/get-KPI-Details")]
        public IActionResult Get(Guid Id)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPIChiTiet");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Id", Id);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<KPICTDTO>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpGet("Infor/get-Chi-Tieu-Nam")]
        public IActionResult GetNam(bool IsCaNhan = true, Guid? DMDonViDanhGiaId = null)
        {
            if (!IsCaNhan && (DMDonViDanhGiaId == null || DMDonViDanhGiaId == Guid.Empty))
                return BadRequest("Nếu đơn vị thì bắt buộc phải có DMDonViDanhGiaId");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPIChiTietYear");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsCaNhan", IsCaNhan);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdUser", Guid.Parse(User.Identity.Name));
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdDMDonViDanhGia", DMDonViDanhGiaId);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<KPICTDTO>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpPut]
        public IActionResult Update(KPIDTO kPIDTO)
        {
            
            string validateTyTrong = ValidateTotalTyTrong(kPIDTO);
            if (validateTyTrong != null)
                return BadRequest(validateTyTrong);
            if (!_uow.KPIs.Exists(k => k.Id == kPIDTO.Id))
                return BadRequest("Id không tồn tại trong database");

            var isValidate = ValidateDTO(kPIDTO);
            if (isValidate != null)
                return isValidate;
            var existingKPI = _uow.KPIs.GetSingle(
                x => x.Id == kPIDTO.Id,
                new string[] { "KPIDetails", "KPIDetails.KPIDetailChildren" }
            );
            //var existingKPI = _uow.KPIs.GetSingle(
            //    x => x.Id == kPIDTO.Id,
            //    new string[] { "KPIDetails", "KPIDetails.KPIDetailChildren" }
            //);
            if (existingKPI.ThoiDiemDanhGia != kPIDTO.ThoiDiemDanhGia)
                return BadRequest("Chỉ được thay đổi tỷ trọng của KPI chi tiết cũ và thêm các KPI chi tiết mới");
            foreach (var newDetail in kPIDTO.KPIChiTietDTOs)
            {
                var existingDetail = existingKPI.KPIDetails.FirstOrDefault(d => d.Id == newDetail.Id);
                if (existingDetail != null)
                {
                    if (existingDetail.ChiTieuCanDat != newDetail.ChiTieuCanDat ||
                        existingDetail.DienGiai != newDetail.DienGiai ||
                        existingDetail.IsAddChiTieuNaw != newDetail.IsAddChiTieuNam)
                        return BadRequest("Chỉ được thay đổi tỷ trọng của KPI chi tiết cũ và thêm các KPI chi tiết mới");
                }
            }
            var dtoDetailIds = kPIDTO.KPIChiTietDTOs
            .Where(d => d.Id != null) // Chỉ lấy các DTO có Id
            .Select(d => d.Id)
            .ToList();
            // Kiểm tra xem có bất kỳ KPIChiTiet nào trong database mà không có trong DTO
            var missingDetails = existingKPI.KPIDetails
                .Where(d => !dtoDetailIds.Contains(d.Id)) // So sánh với danh sách Id từ DTO
                .ToList();

            // Nếu có chi tiết nào trong cơ sở dữ liệu mà không có trong DTO, trả về lỗi
            if (missingDetails.Any())
            {
                return BadRequest("Một hoặc nhiều KPI chi tiết hiện có trong hệ thống không được cung cấp trong dữ liệu DTO.");
            }
            existingKPI.IsReviewed = false;
            existingKPI.IsRefuse = false;
            existingKPI.ReasonForRefuse = null;
            existingKPI.SerialNumberNow = 0;
            //existingKPI.ReasonForRefuse = 

            _uow.KPIs.Update(existingKPI);
            foreach (var newDetail in kPIDTO.KPIChiTietDTOs)
            {
                var existingDetail = existingKPI.KPIDetails.FirstOrDefault(d => d.Id == newDetail.Id);
                if (existingDetail != null)
                {
                    if (existingDetail.Tytrong != newDetail.TyTrong)
                    {
                        existingDetail.Tytrong = newDetail.TyTrong;
                        existingDetail.UpdatedDate = DateTime.Now;
                        existingDetail.UpdatedBy = Guid.Parse(User.Identity.Name);
                        _uow.KPIDetails.Update(existingDetail);
                    }
                }
            }
            _uow.Complete();
            foreach (var item in kPIDTO.KPIChiTietDTOs)
            {
                if (item.Id == null || item.Id == Guid.Empty)
                    AddKPIChiTiet(item, kPIDTO.Id ?? Guid.Empty);
            }
            return Ok("Update thành công");
        }
        [HttpPut("Infor/Get-Status-DonVi-Duyet")]
        public IActionResult GetStatusKPIDonVi(bool IsCaNhan = true)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPT_DONVI_STATUS");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpGet("Infor/get-KPI-DonVis")]
        public IActionResult GetAllKPIDonVis(int? chuky = null, string? ThoiDiemDanhGia = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPIDonVis");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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
        [HttpGet("Infor/Lich-su-cap-nhat")]
        public IActionResult GetLichSu(Guid IdKPI)
        {
            if (!_uow.KPIs.Exists(k => k.Id == IdKPI))
                return BadRequest("Không tồn tại Id.");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetUpdateHistories");
            dbAdapter.sqlCommand.Parameters.Add("@IdKPI", SqlDbType.UniqueIdentifier).Value = IdKPI;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpDelete]
        public ActionResult Delete(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest("Id is null or empty");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DeleteKPI");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DeletedBy", Guid.Parse(User.Identity.Name));
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Xóa thành công.");
            else
                return BadRequest("Id không tồn tại");
        }
        [HttpPut("Duyet/Get-Status")]
        public IActionResult GetStatusKPI(bool IsCaNhan = true)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPI_Status");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            dbAdapter.sqlCommand.Parameters.Add("@IsCaNhan", SqlDbType.Bit).Value = IsCaNhan;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpPut("Duyet/Duyet")]
        public IActionResult Duyet(Guid Id)
        {
            var duyetPI = _uow.KPIs.GetSingle(d => d.Id == Id);
            if (duyetPI == null || duyetPI.IsReviewed || duyetPI.IsRefuse)
                return BadRequest("Danh mục này đã duyệt hoặc đã bị từ chối");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_DuyetKPI");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Id;
            dbAdapter.sqlCommand.Parameters.Add("@Updatedby", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Duyệt thành công");
            return BadRequest("Người dùng không có quyền duyệt KPI này");
        }
        [HttpPut("Duyet/Refuse-Duyet")]
        public IActionResult Refuse(Guid IdKPI, string LyDoTuChoi)
        {
            var duyetPI = _uow.KPIs.GetSingle(d => d.Id == IdKPI);
            if (duyetPI == null || duyetPI.IsRefuse || duyetPI.IsRefuse)
                return BadRequest("Danh mục này đã duyệt hoặc đã bị từ chối");
            if(LyDoTuChoi == null)
                return BadRequest("Lý do từ chối là bắt buộc");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_RefuseDuyetKPI");
            dbAdapter.sqlCommand.Parameters.Add("@IdDanhMucPI", SqlDbType.UniqueIdentifier).Value = IdKPI;
            dbAdapter.sqlCommand.Parameters.Add("@Updatedby", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@WhyForRefuse", LyDoTuChoi);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Từ chối thành công");
            return BadRequest("Người dùng không có quyền từ chối KPI này");
        }
        [HttpGet("Duyet/get-Ly-Do-TuCHoi")]
        public IActionResult GetDoTuChoiDuyet(Guid IdKPI)
        {
            var kPI = _uow.KPIs.GetSingle(dm => dm.Id == IdKPI && !dm.IsDeleted);
            if (kPI == null)
                return BadRequest("Id không tồn tại trong database");
            if (kPI.IsRefuse == false)
                return BadRequest("KPI này chưa bị từ chối duyệt");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetRefuseDuyetKPI");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdKPI", IdKPI);
            var result = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            return Ok(result);
        }

        [HttpGet("Duyet/Tien-trinh-Duyet")]
        public IActionResult GetTienTrinhDuyet(Guid IdKPI)
        {
            if (!_uow.KPIs.Exists(k => k.Id == IdKPI))
                return BadRequest("Không tồn tại Id.");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetTienTrinhDuyetKPI");
            dbAdapter.sqlCommand.Parameters.Add("@IdKPI", SqlDbType.UniqueIdentifier).Value = IdKPI;
            var result = dbAdapter.runStored2ObjectList();
            dbAdapter.deConnect();
            return Ok(result);
        }
        [HttpGet("Duyet/get-KPI-CaNhans")]
        public IActionResult GetAllKPICaNhanDuyet(int? chuky = null, string? ThoiDiemDanhGia = null, Guid? DonViKPIId = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPICaNhanDuyet");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViDanhGiaId", DonViKPIId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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
        [HttpGet("Duyet/get-KPI-DonVis")]
        public IActionResult GetAllKPIDonViDuyet(int? chuky = null, string? ThoiDiemDanhGia = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPIDonViDuyet");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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

        [HttpGet("ThamDinh/Get-Status")]
        public IActionResult GetStatusKPIThamDinh(bool IsCaNhan = true)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPT_STATUS_THAMDINH");
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            dbAdapter.sqlCommand.Parameters.Add("@IsCaNhan", SqlDbType.Bit).Value = IsCaNhan;
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }
        [HttpGet("ThamDinh/Get-list-ca-nhan-tham-dinh")]
        public IActionResult GetAllKPICaNhanThamDinhs(int? chuky = null, string? ThoiDiemDanhGia = null, Guid? DonViKPIId = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPICaNhanThamDinh");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViDanhGiaId", DonViKPIId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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
            });
        }

        [HttpGet("ThamDinh/Get-list-don-vi-tham-dinh")]
        public IActionResult GetAllKPIDonViThamDinhs(int? chuky = null, string? ThoiDiemDanhGia = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPIDonVisThamDinh");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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
            });
        }

        [HttpGet("ThamDinh/List-KPI-detail-tham-dinh")]
        public IActionResult GetKPIDetailTD(Guid IdKPI)
        {
            if (!_uow.KPIs.Exists(k => k.Id == IdKPI))
                return BadRequest("Không tồn tại Id.");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetKPIChiTietThamDinh");
            dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = IdKPI;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<KPICTDTO>(result.ToString());
            if (jsonObject == null)
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(jsonObject);
        }

        [HttpPut("ThamDinh/Tham-dinh")]
        public IActionResult ThamDinh(Guid IdKPIDetail, string? note = null)
        {
            var duyetPI = _uow.KPIDetails.GetSingle(d => d.Id == IdKPIDetail);
            if (duyetPI == null || duyetPI.IsApproved || duyetPI.IsRefuseForApproved)
                return BadRequest("KPI chi tiết này đã thẩm định hoặc bị từ chối");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("UpdateKPIDetailReview");
            dbAdapter.sqlCommand.Parameters.Add("@KPIDetailId", SqlDbType.UniqueIdentifier).Value = IdKPIDetail;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsApproved", 1);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@NoteForApprove", note);
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Thẩm định thành công");
            return BadRequest("Người dùng không có quyền thẩm định KPI này");
        }

        [HttpPut("ThamDinh/Tu-choi-tham-dinh")]
        public IActionResult TuChoiThamDinh(Guid IdKPIDetail, string? note = null)
        {
            var duyetPI = _uow.KPIDetails.GetSingle(d => d.Id == IdKPIDetail);
            if (duyetPI == null || duyetPI.IsApproved || duyetPI.IsRefuseForApproved)
                return BadRequest("KPI chi tiết này đã thẩm định hoặc bị từ chối");
            if(note == null)
            {
                return BadRequest("Từ chối với nội dung từ chối là bắt buộc");
            }
            dbAdapter.connect();
            dbAdapter.createStoredProceder("UpdateKPIIsRefused");
            dbAdapter.sqlCommand.Parameters.Add("@KPIDetailId", SqlDbType.UniqueIdentifier).Value = IdKPIDetail;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IsRefuseForApproved", 1);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@NoteForApprove", note);
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Duyệt thành công");
            return BadRequest("Người dùng không có quyền thẩm định KPI này");
        }
        [HttpPut("ThamDinh/Hoan-tat-tham-dinh")]
        public IActionResult HoanTatThamDinh(Guid IdKPI)
        {
            var duyetPI = _uow.KPIs.GetSingle(d => d.Id == IdKPI);
            if (duyetPI == null || duyetPI.IsApproved || duyetPI.IsRefuseForApproved)
                return BadRequest("KPI chi tiết này không tồn tại hoặc đã thẩm định, bị từ chối");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("CompleteApproved");
            dbAdapter.sqlCommand.Parameters.Add("@KPIId", SqlDbType.UniqueIdentifier).Value = IdKPI;
            dbAdapter.sqlCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = Guid.Parse(User.Identity.Name);
            var result = dbAdapter.runStoredNoneQuery();
            dbAdapter.deConnect();
            if (result > 0)
                return Ok("Hoàn tất thẩm định thành công");
            return BadRequest("Người dùng không có quyền hoàn tất thẩm định kpi này");
        }
        [HttpGet("ThamDinh/get-Ly-Do-TuCHoi")]
        public IActionResult GetDoTuChoiThamDinh(Guid IdKPIDetail)
        {
            var kPI = _uow.KPIDetails.GetSingle(dm => dm.Id == IdKPIDetail && !dm.IsDeleted);
            if (kPI == null)
                return BadRequest("Id không tồn tại trong database");
            if (kPI.IsRefuseForApproved == false)
                return BadRequest("KPI này thẩm định không phải là không đạt");
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetRefuseThamDinhKPI");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@IdKPIDetail", IdKPIDetail);
            var result = dbAdapter.runStored2Object();
            dbAdapter.deConnect();
            return Ok(result);
        }

        [HttpGet("PhongKPI/get-Sum-KPI-CaNhans")]
        public IActionResult GetAllSumKPICaNhans(int? chuky = null, string? ThoiDiemDanhGia = null, Guid? DonViKPIId = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetSumKPICaNhans");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViDanhGiaId", DonViKPIId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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
            });
        }
        [HttpGet("PhongKPI/get-Sum-KPI-DonVis")]
        public IActionResult GetAllSumKPIDonVis(int? chuky = null, string? ThoiDiemDanhGia = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetSumKPIDonVis");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
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
            });
        }

        [HttpGet("PhongKPI/get-Sum-Block-KPI-CaNhan")]
        public IActionResult GetBlockCaNhan(int? chuky = null, string? ThoiDiemDanhGia = null, Guid? DonViKPIId = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetSumKPIBlockCaNhans");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@DonViDanhGiaId", DonViKPIId);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());
            if (jsonObject == null || !jsonObject.Any())
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            var kpiList = new List<KPICTDTOBlock>();
            foreach (var item in jsonObject)
            {
                var kpi = new KPICTDTOBlock
                {
                    id = item.id,
                    maNhanVien = item.maNhanVien,
                    fullName = item.fullName,
                    tenChucDanh = item.tenChucDanh,
                    chuKy = item.chuKy,
                    thoiDiemDanhGia = item.thoiDiemDanhGia,
                    nhomPIs = JsonConvert.DeserializeObject<List<NhomPIDTO>>(item.nhomPIs.ToString())
                };
                kpiList.Add(kpi);
            }
            return Ok(kpiList);
        }
        [HttpGet("PhongKPI/get-Sum-Block-KPI-DonVis")]
        public IActionResult GetBlockKPIDonVis(int? chuky = null, string? ThoiDiemDanhGia = null, string keyword = "", int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            dbAdapter.connect();
            dbAdapter.createStoredProceder("sp_GetSumKPIBlockDonVis");
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ChuKy", chuky);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@ThoiDiem", ThoiDiemDanhGia);
            dbAdapter.sqlCommand.Parameters.AddWithValue("@Keyword", keyword);
            dbAdapter.sqlCommand.Parameters.Add("@PageNumber", SqlDbType.Int).Value = page;
            dbAdapter.sqlCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            dbAdapter.sqlCommand.Parameters.AddWithValue("@UserId", Guid.Parse(User.Identity.Name));
            var result = dbAdapter.runStored2JSON();
            dbAdapter.deConnect();
            var jsonObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());
            if (jsonObject == null || !jsonObject.Any())
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            var kpiList = new List<KPICTDTOBlockDV>();
            foreach (var item in jsonObject)
            {
                var kpi = new KPICTDTOBlockDV
                {
                    id = item.id,
                    tenDonViKPI = item.tenDonViKPI,
                    chuKy = item.chuKy,
                    thoiDiemDanhGia = item.thoiDiemDanhGia,
                    nhomPIs = JsonConvert.DeserializeObject<List<NhomPIDTO>>(item.nhomPIs.ToString())
                };
                kpiList.Add(kpi);
            }
            return Ok(kpiList);
        }

    }
}
