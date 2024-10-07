using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class KetQuaDanhGia : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid IdDanhMucPIChiTiet { get; set; }
        [ForeignKey("IdDanhMucPIChiTiet")]
        public virtual DanhMucPIChiTiet DanhMucPIChiTiet { get; set; }
        public Guid IdDMKetQuaDanhGia { get; set; }
        [ForeignKey("IdDMKetQuaDanhGia")]
        public virtual DM_KetQuaDanhGia DM_KetQuaDanhGia { get; set; }
        public string KhoangGiaTriK { get; set; }
    }
}
