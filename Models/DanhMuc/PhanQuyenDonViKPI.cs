using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Models.Default;
using static ERP.Data.MyDbContext;

namespace ERP.Models.DanhMuc
{
    public class PhanQuyenDonViKPI : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public Guid DonViKPIId { get; set; }
        [ForeignKey("DonViKPIId")]
        public virtual vptq_kpi_DonViKPI DonViKPI { get; set; }
    }
}
