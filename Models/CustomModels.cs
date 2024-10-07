using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using static ERP.Commons;
using static ERP.Data.MyDbContext;

namespace ERP.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Mật khẩu {0} ngắn nhất phải {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string Password { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Mật khẩu {0} ngắn nhất phải {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [CustomPasswordValidation(ErrorMessage = "Mật khẩu phải chứa ít nhất một ký tự in hoa, một ký tự đặc biệt và một số.")]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu xác nhận")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu mới không đúng.")]
        public string ConfirmNewPassword { get; set; }
    }
    public class FileModel
    {
        public Guid? Id { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public bool IsRemoved { get; set; }
    }
    public class PhanQuyenDonVi
    {
        public List<DonViViewModel> lst_DonVis { get; set; }
        public Guid User_Id { get; set; }
    }
    public class KPIDTO
    {
        public Guid? Id { get; set; }
        public bool IsCaNhan { get; set; } = true;
        public bool IsGiaoHangLoat { get; set; } = false;
        public ICollection<Guid> User_Ids { get; set; } = null;
        public Guid? DMDonViDanhGiaId { get; set; } = null;
        public ChuKy ChuKy { get; set; }
        [Required(ErrorMessage = "Thời điểm đánh giá không được để trống.")]
        public string ThoiDiemDanhGia { get; set; }
        public List<KPIChiTietDTO> KPIChiTietDTOs { get; set; }
    }
    public class KPIChiTietDTO
    {
        public Guid? Id { get; set; }
        public Guid DMPIChiTietId { get; set; }
        public byte TyTrong { get; set; }
        [AllowNull]
        public float? ChiTieuCanDat { get; set; } = null;
        [AllowNull]
        public string DienGiai { get; set; } = null;
        public bool IsAddChiTieuNam { get; set; } = false;
        public List<KPIChiTietChildDTO> KPIChiTietChildDTOs { get; set; }
    }
    public class KPIChiTietChildDTO
    {
        public Guid PIPhuThuocId { get; set; }
        public byte TyTrong { get; set; }
        [AllowNull]
        public float? ChiTieuCanDat { get; set; } = null;
        [AllowNull]
        public string DienGiai { get; set; } = null;
    }
    public class DanhMucPIDTO
    {
        public Guid? Id { get; set; }
        public DateTime ApDungDen { get; set; }
        public Guid? IdDonViDanhGia { get; set; } = null;
        public ICollection<DanhMucPIChiTietDTO> DanhMucPIChiTiets { get; set; }
    }
    public class KetQuaDanhGiaDTO
    {
        public Guid IdDMKetQuaDanhGia { get; set; }
        [Required(ErrorMessage = "KHoảng giá trị K không được để trống.")]
        public string KhoangGiaTriK { get; set; }
    }
    public class DanhMucPIChiTietDTO
    {
        //public Guid? Id { get; set; }
        //public Guid IdDanhMucPI { get; set; }
        //[ForeignKey("IdDanhMucPI")]
        //public virtual DanhMucPI DanhMucPI { get; set; }
        public Guid IdNhomPI { get; set; }
        public string MaSo { get; set; }
        public Guid IdDMTrongYeu { get; set; }
        [Required(ErrorMessage = "Chỉ số đánh giá không được để trống.")]
        [MaxLength(500)]
        public string ChiSoDanhGia { get; set; }
        [MaxLength(500)]
        [AllowNull]
        public string DuLieuThamDinh { get; set; }

        public Guid? IdNguoiThamDinh { get; set; }
        [ForeignKey("IdNguoiThamDinh")]
        [Range(0, 1, ErrorMessage = "Giá trị của ChuKy phải là 0: Tháng hoặc 1: Năm.")]
        public ChuKy ChuKy { get; set; }

        [MaxLength(500)]
        [Required(ErrorMessage = "Chi tiết chỉ số đánh giá không được để trống.")]
        public string ChiTietChiSoDanhGia { get; set; }
        public bool TrangThaiSuDung { get; set; } = true;
        [Range(0, 1, ErrorMessage = "Giá trị của TrangThaiSuDung phải là 0: Số hoặc 1: Nội dung.")]
        public KieuDanhGia KieuDanhGia { get; set; }
        [AllowNull]
        public ICollection<DoiTuongApDungDTO> DoiTuongApDungs { get; set; } = null;
        [AllowNull]
        public ICollection<PIPhuThuocDTO> PIPhuThuocs { get; set; } = null;
        [AllowNull]
        public ChieuHuongTot? ChieuHuongTot { get; set; }
        [AllowNull]
        public HeSoHoanThanhK? HeSoHoanThanhK { get; set; }
        [AllowNull]
        public Guid? IdDonViDo { get; set; } = null;
        public ICollection<KetQuaDanhGiaDTO> KetQuaDanhGiaDTOs { get; set; } = null;
    }
    public class PIPhuThuocDTO
    {
        [MaxLength(50)]
        [Required(ErrorMessage = "Mã số không được để trống.")]
        public string MaSo { get; set; }
        [Required(ErrorMessage = "Chỉ số đánh giá không được để trống.")]
        [MaxLength(500)]
        public string ChiSoDanhGia { get; set; }

        [AllowNull]
        public string ChiTietChiSoDanhGia { get; set; }
    }
    public class DoiTuongApDungDTO
    {
        [Range(0, 2, ErrorMessage = "Giá trị của NhomChucDanh phải là 0: CTy hoặc 1: BPNV hoặc 2: CTY & BPNV.")]
        public NhomChucDanh NhomChucDanh { get; set; }
        public Guid ChucDanhId { get; set; }
    }
    public class User_PhanQuyen
    {
        public Guid DonVi_Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsFull { get; set; }
    }
    public class MenuInfo
    {
        public Guid Id { get; set; }
        public string STT { get; set; }
        public string TenMenu { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int ThuTu { get; set; }
        public Guid? Parent_Id { get; set; }
        public List<MenuInfo> children { set; get; }
        public bool IsUsed { get; set; }
        public bool IsRemove { get; set; }
        public Guid? PhanMem_Id { get; set; }
        public Guid? DonVi_Id { get; set; }
        public Guid? TapDoan_Id { get; set; }
        public Guid? PhongBan_Id { get; set; }
    }
    public class Permission
    {
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Del { get; set; }
        public bool Print { get; set; }
        public bool Cof { get; set; }
    }
    public class MenuView
    {
        public Guid Id { get; set; }
        public string STT { get; set; }
        public string TenMenu { get; set; }
        public string Url { get; set; }
        public Guid? Parent_Id { get; set; }
        public int ThuTu { get; set; }
        public string Icon { get; set; }
        public List<MenuView> children { set; get; }
        public Permission permission { set; get; }
        public Guid? PhanMem_Id { get; set; }
        public Guid? DonVi_Id { get; set; }
        public Guid? TapDoan_Id { get; set; }
        public Guid? PhongBan_Id { get; set; }
    }
    public class DonViViewModel
    {
        public Guid? Id { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string STT { get; set; }
        public bool IsUsed { set; get; }
        public bool IsRemove { set; get; }
        public Guid? Parent_Id { set; get; }
        public int ThuTu { set; get; }
        public bool Checked { set; get; }
        public bool HasChild { set; get; }
        public bool IsFull { get; set; }
        public bool IsLeaf { get; set; }
        public int Level { get; set; }
        public List<DonViViewModel> children { set; get; }
        public List<User_PhanQuyen> lst_user_phanquyen { set; get; }
    }
    public class ClassPhongBan
    {
        public Guid Id { get; set; }
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public Guid DonVi_Id { get; set; }
        public Guid? PhongBan_Id { get; set; }
    }
    public class ClassDonVi
    {
        public Guid Id { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public Guid? TapDoan_Id { get; set; }
        public Guid? DonVi_Id { get; set; }
    }
    public class ClassDonViDo
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Mã không được để trống.")]
        [MinLength(1, ErrorMessage = "Mã không được chứa chỉ khoảng trắng.")]
        public string MaDonViDo { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Tên không được để trống.")]
        [MinLength(1, ErrorMessage = "Tên không được chứa chỉ khoảng trắng.")]
        public string TenDonViDo { get; set; }
    }
    public class ClassCauHinhDuyet
    {
        public Guid? Id { get; set; }
        public Guid NhanVienId { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "Cấp duỵệt là bắt buộc")]
        public string CapDuyet { get; set; }
    }
    public class ClassDM_KetQuaDanhGia
    {
        public Guid? Id { get; set; }
        [StringLength(250)]
        [Required(ErrorMessage = "kết quả đánh giá là bắt buộc")]
        public string KetQuaDanhGia { get; set; }
    }
    public class NSKhongDanhGiaDTO
    {
        public Guid? IdDonViDanhGia { get; set; } = null;
        public Guid? IdPhongBan { get; set; } = null;
        public Guid? IdDonVi { get; set; } = null;
        [AllowNull]
        public string ChuKyDanhGia { get; set; } = null;
        public int? ThoiDiemDanhGia { get; set; } = null;
        [AllowNull]

        public string Keyword { get; set; } = null;
    }
    public class ClassNSKhongDanhGia
    {
        [AllowNull]
        public string MaNhanVien { get; set; }
        [AllowNull]
        public string ChuKyDanhGia { get; set; } = "Tháng";

        public string ThoiDiemDanhGia { get; set; }

    }
    public class ChiTieuTyTrongDTO
    {
        public Guid DanhMucNhomPiId { get; set; }
        [Range(0, 100, ErrorMessage = "Chỉ tiêu phải không bé hơn 0 và tổng tất cả các chỉ tiêu tỷ trọng phải là 100")]
        public float ChiTieu { get; set; } = 0;
        public string ToanTu { get; set; } = "=";
    }
    public class ChucDanhDTO
    {
        [Required(ErrorMessage = "Chức Danh Không được để trống")]
        public Guid ChucDanhId { get; set; }
    }
    public class ClassDanhMucTyTrongDTO
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "Nhóm chức danh bắt buộc chọn")]
        public string NhomChucDanh { get; set; }
        public string ChuKyDanhGia { get; set; } = "Tháng";
        public bool IsKhong { get; set; } = true;
        public bool BatBuocDung { get; set; } = true;
        public List<ChiTieuTyTrongDTO> ChiTieuTyTrongList { get; set; }
        public List<ChucDanhDTO> ChucDanhList { get; set; }
    }
    public class KPICTDTO
    {
        public Guid id { get; set; }
        public string chuKy { get; set; }
        public string thoiDiemDanhGia { get; set; }
        public List<NhomPIDTO> nhomPIs { get; set; }
    }
    public class KPICTDTOBlock
    {
        public Guid id { get; set; }
        public string maNhanVien { get; set; }
        public string fullName { get; set; }
        public string tenChucDanh { get; set; }
        public string chuKy { get; set; }
        public string thoiDiemDanhGia { get; set; }
        public List<NhomPIDTO> nhomPIs { get; set; }
    }
    public class KPICTDTOBlockDV
    {
        public Guid id { get; set; }
        public string tenDonViKPI { get; set; }
        public string chuKy { get; set; }
        public string thoiDiemDanhGia { get; set; }
        public List<NhomPIDTO> nhomPIs { get; set; }
    }
    public class NhomPIDTO
    {
        public Guid id { get; set; }
        public string tenDanhMucNhomPI { get; set; }
        public List<KPIDetailDTO> kPIDetails { get; set; }
    }

    public class KPIDetailDTO
    {
        public Guid id { get; set; }
        public string maSo { get; set; }
        public string chiSoDanhGia { get; set; }
        public decimal tyTrong { get; set; }
        public decimal chiTieuCanDat { get; set; }
        public string dienGiai { get; set; }
        public bool isAddChiTieuNaw { get; set; }
        public List<KPIDetailChildDTO> kPIDetailChilds { get; set; }
    }

    public class KPIDetailChildDTO
    {
        public Guid id { get; set; }
        public string maSo { get; set; }
        public decimal tyTrong { get; set; }
        public decimal chiTieuCanDat { get; set; }
        public string dienGiai { get; set; }
    }
    public class ClassDonViDanhGia
    {
        public Guid? Id { get; set; }
        public Guid? IdDonViKPI { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Tiền tố là buộc nhập")]
        public string TienTo { get; set; }

        public bool DanhGia { get; set; } = true;

    }
    public class DM_DonViDanhGiaImport
    {
        public Guid? IdDonViKPI { get; set; }
        [AllowNull]
        public string TienTo { get; set; }
        public bool? DanhGia { get; set; } = true;

    }
    
    public class ClassDanhMucTrongYeu
    {
        public Guid Id { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Mã bắt buộc nhập")]
        public string MaDanhMucTrongYeu { get; set; }

        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        [MaxLength(255)]
        public string TenDanhMucTrongYeu { get; set; }

        [StringLength(500)]
        [AllowNull]
        public string DienGiai { get; set; }

        public bool TrangThai { get; set; } = true;

        [MaxLength(500)]
        [AllowNull]
        public string? GhiChu { get; set; }
    }
    public class ClassDanhMucTrongYeuImport
    {
        [AllowNull]
        public string MaDanhMucTrongYeu { get; set; }
        [AllowNull]
        public string TenDanhMucTrongYeu { get; set; }

        [StringLength(500)]
        [AllowNull]
        public string DienGiai { get; set; }

        public bool TrangThai { get; set; } = true;

        [MaxLength(500)]
        [AllowNull]
        public string GhiChu { get; set; }
    }
    public class ClassDanhMucLanhDaoDonViImport
    {
        [AllowNull]
        public string MaDonVi { get; set; }
        [AllowNull]
        public string MaLanhDao { get; set; }
    }
    public class AddAndUpdateDanhMucUyQuyenDTO
    {
        public Guid? Id { get; set; }
        public Guid LanhDaoUyQuyenId { get; set; }

        public List<AddAndUpdateDuocUyQuyenDTO> DuocUyQuyenDTOs { get; set; }
    }
    public class AddAndUpdateDuocUyQuyenDTO
    {
        public Guid LanhDaoDuocUyQuyenId { get; set; }
    }
    public class DMGiaoChiTieuDTO
    {
        public Guid? Id { get; set; }
        public Guid DonViKPIId { get; set; }

        public List<DuocGiaoChiTieuDTO> DuocGiaoChiTieuDTOs { get; set; }
    }
    public class DuocGiaoChiTieuDTO
    {
        public Guid UserId { get; set; }
    }
    public class AddAndUpdatePhanQuyenDVKPIDTO
    {
        public Guid UserId { get; set; }

        public List<AddAndUpdateDonViKPIDTO> DonViKPIDTOs { get; set; }
    }

    public class DanhMucDuyetAddAndUpdateDTO
    {
        public Guid? Id { get; set; }
        public bool IsCaNhan { get; set; } = true;
        public Guid? NhanVienId { get; set; }
        public Guid? DanhMucDonViId { get; set; }
        public  List<CapDuyetDTO> CapDuyetDTOs { get; set; }
    }

    public class CapDuyetDTO
    {
        public CacCapDuyet CapDuyet { get; set; }
        public Guid LanhDaoDuyetId { get; set; }
    }

    public class DanhMUcDuyetImportDTO
    {
        [AllowNull]
        public string MaNhanVien { get; set; }
        [AllowNull]
        public string MaDonViKPI { get; set; }
        [AllowNull]
        public List<CapDuyetImportDTO> CapDuyetImportDTOs { get; set; }
    }

    public class CapDuyetImportDTO
    {
        public CacCapDuyet CapDuyet { get; set; }
        [AllowNull]
        public string MaLanhDaoDuyet { get; set; }
    }
    public class AddAndUpdateDonViKPIDTO
    {
        public Guid DonViKPIId { get; set; }
    }

    public class ImportPhanQuyenDonViKPIDTO
    {
        [AllowNull]
        public string MaNhanVIen { get; set; }

        [AllowNull]
        public string MaDonViKPI { get; set; }
    }
    public class ImportDanhMucUyQuyenDTO
    {
        [AllowNull]
        public string MaLanhDaoUyQuyen { get; set; }

        [AllowNull]
        public string MaLanhDaoDuocUyQuyen { get; set; }
    }

    public class ImportDMGiaoChiTieuDTO
    {
        [AllowNull]
        public string MaDonViKPI { get; set; }

        [AllowNull]
        public string MaNhanVien { get; set; }
    }
    public class DanhMucTyTrongDTO
    {
        public Guid Id { get; set; }
        public string ChuKyDanhGia { get; set; }
        public double NhomChiTieuSXKD { get; set; }
        public double NhomChiTieuCTQT { get; set; }
        public bool BatBuocDung { get; set; }
        public bool IsKhong { get; set; }
        public List<NhomChucDanhDTO> NhomChucDanhs { get; set; } = new List<NhomChucDanhDTO>();
    }

    public class NhomChucDanhDTO
    {
        public Guid NhomChucDanhId { get; set; }
        public string TenNhomChucDanh { get; set; }
        public List<ChucDanhDTO> ChucDanhs { get; set; } = new List<ChucDanhDTO>();
    }


    public class DanhMucTyTrongByIdDTO
    {
        public Guid Id { get; set; } // DanhMucTyTrongId
        public string ChuKyDanhGia { get; set; }
        public double NhomChiTieuSXKD { get; set; }
        public double NhomChiTieuCTQT { get; set; }
        public bool BatBuocDung { get; set; }
        public bool IsKhong { get; set; }

        // Danh sách các chức danh trong danh mục tỷ trọng
        public List<ChucDanhByIdDTO> ChucDanhs { get; set; } = new List<ChucDanhByIdDTO>();
    }

    public class ChucDanhByIdDTO
    {
        public Guid ChucDanhId { get; set; } // ChucDanhId
        public string TenChucDanh { get; set; }

        // Danh sách các nhóm chức danh thuộc chức danh
        public List<NhomChucDanhByIdDTO> NhomChucDanhs { get; set; } = new List<NhomChucDanhByIdDTO>();
    }

    public class NhomChucDanhByIdDTO
    {
        public Guid NhomChucDanhId { get; set; } // NhomChucDanhId
        public string TenNhomChucDanh { get; set; }
    }


    public class ChucDanh_NhomChucDanhDTO
    {
        public Guid ChucDanhId { get; set; }
        public Guid NhomChucDanhId { get; set; }
    }
    public class DanhMucTyTrongDto
    {
        public Guid? Id { get; set; }
        public string ChuKyDanhGia { get; set; }
        public double NhomChiTieuSXKD { get; set; }
        public double NhomChiTieuCTQT { get; set; }
        public bool BatBuocDung { get; set; } = true;
        public bool IsKhong { get; set; } = true;

        public List<ChucDanh_NhomChucDanhDTO> ChucDanhNhomChucDanhs { get; set; }
    }
    public class DanhMucUyQuyenDTO
    {
        public Guid DanhMucUyQuyenId { get; set; }
        public string MaLanhDaoUyQuyen { get; set; }
        public string TenLanhDaoUyQuyen { get; set; }
        public List<DuocUyQuyenDTO> LanhDaoDuocUyQuyens { get; set; }
    }
    public class UpdateDanhMucUyQuyenDTO
    {
        public Guid DuocUyQuyenId { get; set; }
        public Guid LanhDaoDuocUyQuyenId { get; set; }
        public Guid DanhMucUyQuyenId { get; set; }
    }
    public class DuocUyQuyenDTO
    {
        public Guid DuocUyQuyenId { get; set; }
        public string MaLanhDaoDuocUyQuyen { get; set; }
        public string TenLanhDaoDuocUyQuyen { get; set; }
    }
    public class DM_LanhDaoDonViDTO
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "LanhDaoId is required.")]
        public Guid LanhDaoId { get; set; }


        [Required(ErrorMessage = "DonViID is required.")]
        public Guid DonViID { get; set; }
    }
    public class ClassDanhMucNhomPI
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Mã bắt buộc nhập")]
        [MaxLength(50)]
        public string MaDanhMucNhomPI { get; set; }

        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        [MaxLength(255)]
        public string TenDanhMucNhomPI { get; set; }
        public bool TrangThai { get; set; } = true;

        [StringLength(500)]
        [AllowNull]
        public string GhiChu { get; set; }
    }
    public class ClassTapDoan
    {
        public Guid Id { get; set; }
        public string MaTapDoan { get; set; }
        public string TenTapDoan { get; set; }
        public Guid? TapDoan_Id { get; set; }
    }
    //for admin
    public class ClassAdminActive
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public Guid Role_Id { get; set; }
        public Guid? Roleold_Id { get; set; }
    }
    public class DanhMucSave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}