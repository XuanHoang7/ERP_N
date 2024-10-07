using static ERP.Data.MyDbContext;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ERP.Models.DanhMuc
{
    public class DuocGiaoChiTieu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public Guid DMGiaoChiTieuId { get; set; }
        [ForeignKey("DMGiaoChiTieuId")]
        public virtual DMGiaoChiTieu DMGiaoChiTieu { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
