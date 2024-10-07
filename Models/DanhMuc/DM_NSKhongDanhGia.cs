using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Models.Default;
using static ERP.Data.MyDbContext;

namespace ERP.Models.DanhMuc
{
    public class DM_NSKhongDanhGia : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }
        public string ChuKyDanhGia { get; set; }
        public string ThoiDiemDanhGia { get; set; }
        public Guid DonViID { get; set; }
        public vptq_kpi_DonViKPI DonVi { get; set; }
    }
}
