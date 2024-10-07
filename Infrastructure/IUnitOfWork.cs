using System;
using ERP.Models;
using ERP.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ERP.Infrastructure
{
    public interface IUnitofWork : IDisposable
    {
        IMenuRepository Menus { get; }
        IDonViTinhRepository DonViTinhs { get; }
        IPhongbanRepository phongbans { get; }
        IDonViRepository DonVis { get; }
        IMenu_RoleRepository Menu_Roles { get; }
        ILogRepository Logs { get; }
        IBoPhanRepository BoPhans { get; }
        ITapDoanRepository tapDoans { get; }
        IChucVuRepository chucVus { get; }
        IChucDanhRepository ChucDanhs { get; }
        ICapDoNhanSuRepository CapDoNhanSus { get; }
        IDonViTraLuongRepository DonViTraLuongs { get; }
        IDonViHRMRepository DonViHRMs { get; }
        ICapDoPhongBanBoPhanRepository CapDoPhongBanBoPhans { get; }
        IThanhPhanRepository ThanhPhans { get; }
        IPhongBanHRMRepository PhongBanHRMs { get; }
        IEmailPhongCongNgheThongTinRepository EmailPhongCongNgheThongTins { get; }
        IDonViChiTietRepository DonViChiTiets { get; }
        IConfigRepository Configs { get; }
        IRoleByDonViRepository roleByDonVis { get; }
        IRole_DV_PBRepository role_DV_PBs { get; }
        IPhanMemRepository phanMems { get; }
        IPhanMemDonViURLRepository PhanMemDonViURLs { get; }
        IPhienBanRepository phienBans { get; }
        IChuKySoRepository ChuKySos { get; }
        IThongBaoHeThongRepository ThongBaoHeThongs { get; }
        IPhongBanThacoRepository PhongBanThacos { get; }
		Ivptq_kpi_DonViKPIRepository vptq_kpi_DonViKPIs { get; }
		IUserRepostitory Users { get; }
        IDanhMucTrongYeuRepository DanhMucTrongYeus { get; }
        IDanhMucLanhDaoDonViRepository DanhMucLanhDaoDonVis { get; }
        IDanhMucNSKhongDanhGiaRepository DanhMucNSKhongDanhGias { get; }
        IDanhMucUyQuyenRepository DanhMucUyQuyens { get; }
        IDuocUyQuyenRepository DuocUyQuyens { get; }
        IDanhMucTyTrongRepository DanhMucTyTrongs  { get; }
        IChucDanhTyTrongRepository ChucDanhTyTrongs { get; }
        IDM_DonViDanhGiaRepository DonViDanhGias { get; }
        IDM_KetQuaDanhGiaRepository KetQuaDanhGias { get; }
        IDanhMucPIRepository DanhMucPIs { get; }
        IDonViDoRepository DonViDos { get; }
        IDanhMucNhomPIRepository DanhMucNhomPIs { get; }
        IPhanQuyenDonViKPIRepository PhanQuyenDonViKPIs { get; }
        IDuyetPIRepository duyetPIs { get; }
        ICapDuyetRepository CapDuyets { get; }
        IDanhMucDuyetRepository DanhMucDuyets { get; }
        IDanhMucPIChiTietRepository DanhMucPIChiTiets { get; }
        IDMGiaoChiTieuRepository DMGiaoChiTieus { get; }
        IDuocGiaoChiTieuRepository DuocGiaoChiTieus { get; }
        IKPIRepository KPIs { get; }
        IKPIDetailRepository KPIDetails { get; }
        IStreamReviewKPIRepository Streams { get; }
        IRoleUserRepository RoleUsers { get; }
        IPiPhuThuocRepository PiPhuThuocs { get; }
        int Complete();
    }
}