using System.ComponentModel.DataAnnotations.Schema;
using static ERP.Data.MyDbContext;
using System.Diagnostics.CodeAnalysis;
using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ChiTieuKPI
{
    public class UpdateHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid KPIId { get; set; }
        [ForeignKey("KPIId")]
        public virtual KPI KPI { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DateTime DateUpdate { get; set; }
    }
}
