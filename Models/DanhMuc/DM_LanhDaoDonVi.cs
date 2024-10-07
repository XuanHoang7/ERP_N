using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using static ERP.Data.MyDbContext;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DM_LanhDaoDonVi : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid LanhDaoId { get; set; }
        [ForeignKey("LanhDaoId")]
        public virtual ApplicationUser User { get; set; }
        public Guid DonViID { get; set; }
        [ForeignKey("DonViID")]
        public vptq_kpi_DonViKPI DonVi { get; set; }
    }
}
