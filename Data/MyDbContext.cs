using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static ERP.Data.MyDbContext;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Models.DanhMuc;
using ERP.Models.ChiTieuKPI;
using ERP.Models.Default;

namespace ERP.Data
{
    public class MyDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>,
    ApplicationUserRole, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public class ApplicationUser : IdentityUser<Guid>
        {
            [StringLength(100)]
            public string FullName { get; set; }
            [StringLength(10)]
            public string MaNhanVien { get; set; }
            [StringLength(200)]
            public string HinhAnhUrl { get; set; }
            [ForeignKey("ChucDanh")]
            public Guid? ChucDanh_Id { get; set; }
            public ChucDanh ChucDanh { get; set; }
            [ForeignKey("ChucVu")]
            public Guid? ChucVu_Id { get; set; }
            public ChucVu ChucVu { get; set; }
            public Guid? ThanhPhan_Id { get; set; }
            public Guid? DonVi_Id { get; set; } //Null
            public Guid? DonViTraLuong_Id { get; set; } //Null
            public Guid? CapDoNhanSu_Id { get; set; } //Null
            [StringLength(30)]
            public string MaPhongBanHRM { get; set; } //Null
            [StringLength(40)]
            public string TrinhDoChuyenMon { get; set; }
            [StringLength(150)]
            public string Truong { get; set; }
            [StringLength(100)]
            public string ChuyenNganh { get; set; }
            [StringLength(100)]
            public string EmailThongBao { get; set; }
            public DateTime? NgaySinh { get; set; }
            public DateTime? NgayVaoLam { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public bool HasUyQuyen { get; set; } = false;
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public DateTime? DeletedDate { get; set; }
            public ICollection<ApplicationUserRole> UserRoles { get; set; }
            public bool NghiViec { get; set; }
            public DateTime? NgayNghiViec { get; set; }
            [ForeignKey("PhongBanThaco")]
            public Guid? PhongBanThaco_Id { get; set; }
            public PhongBanThaco PhongBanThaco { get; set; }
            [StringLength(250)]
            public string GhiChu { get; set; }
            public bool GioiTinh { get; set; } = false; //Nam: true - 1, Nu: false - 0
            [StringLength(250)]
            public string NoiLamViec { get; set; }
            [StringLength(250)]
            public string NoiOHienTai { get; set; }
            [ForeignKey("vptq_kpi_DonViKPI")]
            public Guid? vptq_kpi_DonViKPI_Id { get; set; }
            public vptq_kpi_DonViKPI vptq_kpi_DonViKPI { get; set; }
        }
        public class ApplicationRole : IdentityRole<Guid>
        {
            public string Description { get; set; }
            public bool IsDeleted { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public DateTime? DeletedDate { get; set; }
            public Guid? PhanMem_Id { get; set; }
            public Guid? TapDoan_Id { get; set; }
            public Guid? DonVi_Id { get; set; }
            public Guid? PhongBan_Id { get; set; }
            public ICollection<ApplicationUserRole> UserRoles { get; set; }
            public ICollection<Menu_Role> Menu_Roles { get; set; }
        }
        public class ApplicationUserRole : IdentityUserRole<Guid>
        {
            public virtual ApplicationUser User { get; set; }
            public virtual ApplicationRole Role { get; set; }
            public bool Default { get; set; } = false;
            public bool IsActive_Role { get; set; } = true;
        }
        public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Loại bỏ quan hệ vòng
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
                userRole.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
                userRole.HasOne(ur => ur.User)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });
            builder.Entity<Menu_Role>(pq =>
            {
                pq.HasKey(ur => new { ur.Menu_Id, ur.Role_Id });
                pq.HasOne(ur => ur.Menu)
                .WithMany(r => r.Menu_Roles)
                .HasForeignKey(ur => ur.Menu_Id)
                .IsRequired();
                pq.HasOne(ur => ur.Role)
                .WithMany(r => r.Menu_Roles)
                .HasForeignKey(ur => ur.Role_Id)
                .IsRequired();
            });
        }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<ChucDanh> ChucDanhs { get; set; }
        public DbSet<CapDoNhanSu> CapDoNhanSus { get; set; }
        public DbSet<DonViTraLuong> DonViTraLuongs { get; set; }
        public DbSet<DonViHRM> DonViHRMs { get; set; }
        public DbSet<CapDoPhongBanBoPhan> CapDoPhongBanBoPhans { get; set; }
        public DbSet<ThanhPhan> ThanhPhans { get; set; }
        public DbSet<PhongBanHRM> PhongBanHRMs { get; set; }
        public DbSet<EmailPhongCongNgheThongTin> EmailPhongCongNgheThongTins { get; set; }
        public DbSet<DonViChiTiet> DonViChiTiets { get; set; }
        public DbSet<Phongban> phongbans { get; set; }
        public DbSet<DonViTinh> DonViTinhs { get; set; }
        public DbSet<DonVi> DonVis { get; set; }
        public DbSet<Menu_Role> Menu_Roles { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<BoPhan> BoPhans { get; set; }
        public DbSet<TapDoan> TapDoans { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ChuKySo> ChuKySos { get; set; }
        public DbSet<RoleByDonVi> RoleByDonVis { get; set; }
        public DbSet<Role_DV_PB> Role_DV_PBs { get; set; }
        public DbSet<PhanMem> PhanMems { get; set; }
        public DbSet<PhanMemDonViURL> PhanMemDonViURLs { get; set; }
        public DbSet<PhienBan> phienBans { get; set; }
        public DbSet<ThongBaoHeThong> ThongBaoHeThongs { get; set; }
        public DbSet<PhongBanThaco> PhongBanThacos { get; set; }
		public DbSet<vptq_kpi_DonViKPI> vptq_kpi_DonViKPIs { get; set; }
        /*CreateMyDBContext*/
        public DbSet<DanhMucNhomPI> DanhMucNhomPIs { get; set; }
        public DbSet<DanhMucTrongYeu> DanhMucTrongYeus { get; set; }
        public DbSet<DanhMucTyTrong> DanhMucTyTrongs { get; set; }
        public DbSet<DanhMucUyQuyen> DanhMucUyQuyens { get; set; }
        public DbSet<DM_LanhDaoDonVi> DM_LanhDaoDonVis { get; set; }
        public DbSet<DM_NSKhongDanhGia> NSKhongDanhGias { get; set; }
        public DbSet<DM_DonViDanhGia> DM_DonViDanhGias { get; set; }
        public DbSet<CauHinhDuyet> CauHinhDuyets { get; set; }
        public DbSet<DM_KetQuaDanhGia> DM_KetQuaDanhGias { get; set; }
        public DbSet<DonViDo> DonViDos { get; set; }
        public DbSet<DanhMucPI> DanhMucPIs { get; set; }
        public DbSet<PhanQuyenDonViKPI> PhanQuyenDonViKPIs { get; set; }
        public DbSet<DuyetPI> DuyetPIs { get; set; }
        public DbSet<DanhMucDuyet> DanhMucDuyets { get; set; }
        public DbSet<DMGiaoChiTieu> DMGiaoChiTieus { get; set; }
        public DbSet<KPI> KPIs { get; set; }
    }
}
