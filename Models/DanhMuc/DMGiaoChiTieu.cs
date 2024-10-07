using static ERP.Data.MyDbContext;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DMGiaoChiTieu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DonViKPIId { get; set; }
        [ForeignKey("DonViKPIId")]
        public virtual vptq_kpi_DonViKPI DonViKPI { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<DuocGiaoChiTieu> DuocGiaoChiTieus { get; set; }
    }
}
