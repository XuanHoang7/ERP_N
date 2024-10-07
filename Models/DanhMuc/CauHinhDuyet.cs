using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using static ERP.Data.MyDbContext;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class CauHinhDuyet : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Mã bắt buộc")]
        public Guid NhanVienId { get; set; }
        [ForeignKey("NhanVienId")]
        public virtual ApplicationUser User { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "Cấp duỵệt là bắt buộc")]
        public string CapDuyet { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ThuTuDuyet phải lớn hơn 0.")]
        public int ThuTuDuyet { get; set; }
    }
}
