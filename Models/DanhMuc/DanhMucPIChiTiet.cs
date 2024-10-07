using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ERP.Models.Default;
using static ERP.Data.MyDbContext;

namespace ERP.Models.DanhMuc
{
    public class DanhMucPIChiTiet : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid IdDanhMucPI { get; set; }
        [ForeignKey("IdDanhMucPI")]
        public virtual DanhMucPI DanhMucPI { get; set; }
        public Guid IdNhomPI { get; set; }
        [ForeignKey("IdNhomPI")]
        public virtual DanhMucNhomPI DanhMucNhomPI { get; set; }
        [MaxLength(50)]
        public string MaSo { get; set; }

        public Guid IdDMTrongYeu { get; set; }
        [ForeignKey("IdDMTrongYeu")]
        public virtual DanhMucTrongYeu DanhMucTrongYeu { get; set; }
        [MaxLength(500)]
        public string ChiSoDanhGia { get; set; }
        [MaxLength(500)]
        [AllowNull]
        public string DuLieuThamDinh { get; set; }

        public Guid? IdNguoiThamDinh { get; set; }
        [ForeignKey("IdNguoiThamDinh")]
        [AllowNull]
        public virtual ApplicationUser NguoiThamDinh { get; set; }
        public ChuKy ChuKy { get; set; }

        [MaxLength(500)]
        public string ChiTietChiSoDanhGia { get; set; }
        public bool TrangThaiSuDung { get; set; } = true;
        public KieuDanhGia KieuDanhGia { get; set; }
        [AllowNull]
        public ChieuHuongTot? ChieuHuongTot { get; set; } = null;
        [AllowNull]
        public HeSoHoanThanhK? HeSoHoanThanhK { get; set; } = null;
        [AllowNull]
        public Guid? IdDonViDo { get; set; } = null;
        [ForeignKey("IdDonViDo")]
        [AllowNull]
        public virtual DonViDo DonViDo { get; set; } = null;
        public virtual ICollection<KetQuaDanhGia> KetQuaDanhGias { get; set; }
        [AllowNull]
        public virtual ICollection<DoiTuongApDung> DoiTuongApDungs { get; set; } = null;
        [AllowNull]
        public virtual ICollection<PIPhuThuoc> PIPhuThuocs { get; set; } = null;
    }
}
