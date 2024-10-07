using ERP.Models.DanhMuc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.ChiTieuKPI
{
    public class KPIDetailChild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid KPIDetailId { get; set; }
        [ForeignKey("KPIDetailId")]
        public virtual KPIDetail KPIDetail { get; set; }
        public Guid PIPhuThuocId { get; set; }
        [ForeignKey("PIPhuThuocId")]
        public virtual PIPhuThuoc PIPhuThuoc { get; set; }
        public byte Tytrong { get; set; }
        public float ChiTieuCanDat { get; set; }
        public string DienGiai { get; set; }
    }
}
