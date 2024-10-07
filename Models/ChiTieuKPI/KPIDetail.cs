using ERP.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using static ERP.Data.MyDbContext;

namespace ERP.Models.ChiTieuKPI
{
    public class KPIDetail : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid KPIId { get; set; }
        [ForeignKey("KPIId")]
        public virtual KPI KPI { get; set; }
        public Guid DanhMucPIChiTietId { get; set; }
        [ForeignKey("DanhMucPIChiTietId")]
        public virtual DanhMucPIChiTiet DanhMucPIChiTiet { get; set; }
        public byte Tytrong { get; set; }
        public float ChiTieuCanDat { get; set; }
        public string DienGiai { get; set; }
        public bool IsAddChiTieuNaw { get; set; } = false;
        public bool WillApproved { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public bool IsRefuseForApproved { get; set; } = false;
        [AllowNull]
        public DateTime? DateRefuseForApproved { get; set; } = null;
        [AllowNull]
        public string NoteForApprove { get; set; }
        public virtual ICollection<KPIDetailChild> KPIDetailChildren { get; set; }
    }
}
