using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.Default
{
    public class PhanMem : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string MaPhanMem { get; set; }
        [StringLength(250)]
        public string TenPhanMem { get; set; }
        [StringLength(50)]
        public string Icon { get; set; }
        [StringLength(250)]
        public string HinhAnh { get; set; }
        public Guid? NguoiQuanLy_Id { get; set; }
        public bool IsDungChung { get; set; } = false;
        public bool IsSuDungNgoai { get; set; } = false;
        [StringLength(250)]
        public string UrlPhamMemNgoai { get; set; }
    }
}
