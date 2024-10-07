using static ERP.Data.MyDbContext;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ERP.Models.DanhMuc;

namespace ERP.Models.ChiTieuKPI
{
    public class DanhMucDuyet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public bool IsCaNhan { get; set; } = true;
        [AllowNull]
        public Guid? NhanVienId { get; set; }
        [ForeignKey("NhanVienId")]
        [AllowNull]
        public virtual ApplicationUser NhanVien { get; set; } = null;

        [AllowNull]
        public Guid? DM_DonViDanhGiaId { get; set; } = null;
        [ForeignKey("DM_DonViDanhGiaId")]
        [AllowNull]
        public virtual DM_DonViDanhGia DM_DonViDanhGia { get; set; } = null;
        public virtual ICollection<CapDuyet> CapDuyets { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
