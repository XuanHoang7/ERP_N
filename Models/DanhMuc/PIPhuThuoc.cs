using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class PIPhuThuoc : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DanhMucPIChiTietId { get; set; }
        [ForeignKey("DanhMucPIChiTietId")]
        public virtual DanhMucPIChiTiet DanhMucPIChiTiet { get; set; }
        [MaxLength(50)]
        public string MaSo { get; set; }
        [MaxLength(500)]
        public string ChiSoDanhGia { get; set; }

        [AllowNull]
        public string ChiTietChiSoDanhGia { get; set; }
    }
}
