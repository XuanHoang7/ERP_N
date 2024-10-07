using ERP.Data;
using ERP.Infrastructure;
using ERP.Models;
using ERP.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ERP.UOW
{
    public class UnitofWork : IUnitofWork
    {
        public IMenuRepository Menus { get; private set; }
        public IDonViRepository DonVis { get; private set; }
        public IDonViTinhRepository DonViTinhs { get; private set; }
        public IMenu_RoleRepository Menu_Roles { get; private set; }
        public ILogRepository Logs { get; private set; }
        public IBoPhanRepository BoPhans { get; private set; }
        public ITapDoanRepository tapDoans { get; private set; }
        public IPhongbanRepository phongbans { get; private set; }
        public IChucVuRepository chucVus { get; private set; }
        public IChucDanhRepository ChucDanhs { get; private set; }
        public ICapDoNhanSuRepository CapDoNhanSus { get; private set; }
        public IDonViTraLuongRepository DonViTraLuongs { get; private set; }
        public IDonViHRMRepository DonViHRMs { get; private set; }
        public ICapDoPhongBanBoPhanRepository CapDoPhongBanBoPhans { get; private set; }
        public IThanhPhanRepository ThanhPhans { get; private set; }
        public IPhongBanHRMRepository PhongBanHRMs { get; private set; }
        public IEmailPhongCongNgheThongTinRepository EmailPhongCongNgheThongTins { get; private set; }
        public IDonViChiTietRepository DonViChiTiets { get; private set; }
        public IConfigRepository Configs { get; private set; }
        public IRoleByDonViRepository roleByDonVis { get; private set; }
        public IRole_DV_PBRepository role_DV_PBs { get; private set; }
        public IPhanMemRepository phanMems { get; private set; }
        public IPhanMemDonViURLRepository PhanMemDonViURLs { get; private set; }
        public IPhienBanRepository phienBans { get; private set; }
        public IChuKySoRepository ChuKySos { get; private set; }
        public IThongBaoHeThongRepository ThongBaoHeThongs { get; private set; }
        public IPhongBanThacoRepository PhongBanThacos { get; private set; }
		public Ivptq_kpi_DonViKPIRepository vptq_kpi_DonViKPIs { get; private set; }

        public IDanhMucTrongYeuRepository DanhMucTrongYeus { get; private set; }
        public IDanhMucLanhDaoDonViRepository DanhMucLanhDaoDonVis { get; private set; }
        public IDanhMucNSKhongDanhGiaRepository DanhMucNSKhongDanhGias { get; private set; }
        public IDanhMucUyQuyenRepository DanhMucUyQuyens { get; private set; }
        public IDuocUyQuyenRepository DuocUyQuyens { get; private set; }
        public IDanhMucTyTrongRepository DanhMucTyTrongs { get; private set; }
        public IChucDanhTyTrongRepository ChucDanhTyTrongs { get; private set; }
        public IUserRepostitory Users {  get; private set; }
        public IDM_DonViDanhGiaRepository DonViDanhGias {  get; private set; }
        public IDM_KetQuaDanhGiaRepository KetQuaDanhGias {  get; private set; }
        public IDanhMucPIRepository DanhMucPIs {  get; private set; }
        public IDonViDoRepository DonViDos {  get; private set; }
        public IDanhMucNhomPIRepository DanhMucNhomPIs {  get; private set; }
        public IPhanQuyenDonViKPIRepository PhanQuyenDonViKPIs { get; private set; }
        public IDuyetPIRepository duyetPIs { get; private set; }
        public ICapDuyetRepository CapDuyets { get; private set; }
        public IDanhMucDuyetRepository DanhMucDuyets { get; private set; }
        public IDanhMucPIChiTietRepository DanhMucPIChiTiets { get; private set; }
        public IDMGiaoChiTieuRepository DMGiaoChiTieus { get; private set; }
        public IDuocGiaoChiTieuRepository DuocGiaoChiTieus { get; private set; }
        public IKPIRepository KPIs { get; private set; }
        public IKPIDetailRepository KPIDetails { get; private set; }
        public IStreamReviewKPIRepository Streams { get; private set; }
        public IRoleUserRepository RoleUsers { get; private set; }
        public IPiPhuThuocRepository PiPhuThuocs { get; private set; }

        private MyDbContext db;
        public UnitofWork(MyDbContext _db)
        {
            db = _db;
            Menus = new MenuRepository(db);
            DonViTinhs = new DonViTinhRepository(db);
            DonVis = new DonViRepository(db);
            Menu_Roles = new Menu_RoleRepository(db);
            Logs = new LogRepository(db);
            BoPhans = new BoPhanRepository(db);
            tapDoans = new TapDoanRepository(db);
            phongbans = new PhongbanRepository(db);
            chucVus = new ChucVuRepository(db);
            ChucDanhs = new ChucDanhRepository(db);
            CapDoNhanSus = new CapDoNhanSuRepository(db);
            DonViTraLuongs = new DonViTraLuongRepository(db);
            DonViHRMs = new DonViHRMRepository(db);
            CapDoPhongBanBoPhans = new CapDoPhongBanBoPhanRepository(db);
            ThanhPhans = new ThanhPhanRepository(db);
            PhongBanHRMs = new PhongBanHRMRepository(db);
            EmailPhongCongNgheThongTins = new EmailPhongCongNgheThongTinRepository(db);
            DonViChiTiets = new DonViChiTietRepository(db);
            Configs = new ConfigRepository(db);
            roleByDonVis = new RoleByDonViRepository(db);
            role_DV_PBs = new Role_DV_PBRepository(db);
            phanMems = new PhanMemRepository(db);
            PhanMemDonViURLs = new PhanMemDonViURLRepository(db);
            phienBans = new PhienBanRepository(db);
            ChuKySos = new ChuKySoRepository(db);
            ThongBaoHeThongs = new ThongBaoHeThongRepository(db);
            PhongBanThacos = new PhongBanThacoRepository(db);
			vptq_kpi_DonViKPIs = new vptq_kpi_DonViKPIRepository(db);
			DanhMucTrongYeus = new DanhMucTrongYeuRepository(db);
			DanhMucLanhDaoDonVis = new DanhMucLanhDaoDonViRepository(db);
            DanhMucNSKhongDanhGias = new DanhMucNSKhongDanhGiaRepository(db);
            DanhMucUyQuyens = new DanhMucUyQuyenRepository(db);
            DuocUyQuyens = new DuocUyQuyenRepository(db);
            DanhMucTyTrongs = new DanhMucTyTrongRepository(db);
            ChucDanhTyTrongs = new ChucDanhTyTrongRepository(db);
            Users = new UserRepostitory(db);
            DonViDanhGias = new DM_DonViDanhGiaRepository(db);
            KetQuaDanhGias = new DM_KetQuaDanhGiaRepository(db);
            DanhMucPIs = new DanhMucPIRepository(db);
            DonViDos = new DonViDoRepository(db);
            DanhMucNhomPIs = new DanhMucNhomPIRepository(db);
            PhanQuyenDonViKPIs = new PhanQuyenDonViKPIRepository(db);
            duyetPIs = new DuyetPIRepository(db);
            CapDuyets = new CapDuyetRepository(db);
            DanhMucDuyets = new DanhMucDuyetRepository(db);
            DanhMucPIChiTiets = new DanhMucPIChiTietRepository(db); 
            DMGiaoChiTieus = new DMGiaoChiTieuRepository(db);
            DuocGiaoChiTieus = new DuocGiaoChiTieuRepository(db);
            KPIs = new KPIRepository(db);
            KPIDetails = new KPIDetailRepository(db);
            Streams = new StreamReviewKPIRepository(db);
            RoleUsers = new RoleUserRepository(db);
            PiPhuThuocs = new PiPhuThuocRepository(db);
        }
        public void Dispose()
        {
            db.Dispose();
        }
        public int Complete()
        {
            return db.SaveChanges();
        }
    }
}
