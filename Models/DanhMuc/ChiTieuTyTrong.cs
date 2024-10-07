using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class ChiTieuTyTrong : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DanhMucNhomPiId { get; set; }
        [ForeignKey("DanhMucNhomPiId")]
        public virtual DanhMucNhomPI DanhMucNhomPI { get; set; }

        public Guid DanhMucTyTrongId { get; set; }
        [ForeignKey("DanhMucTyTrongId")]
        public virtual DanhMucTyTrong DanhMucTyTrong { get; set; }

        [Range(0, 100, ErrorMessage = "ThuTuDuyet phải lớn hơn 0.")]
        public float ChiTieu { get; set; }

        public string ToanTu { get; set; } = "=";
    }
}
