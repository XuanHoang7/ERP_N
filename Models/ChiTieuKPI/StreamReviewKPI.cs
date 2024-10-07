using static ERP.Data.MyDbContext;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using ERP.Models.ChiTieuKPI;
using System.Diagnostics.CodeAnalysis;

namespace ERP.Models
{
    public class StreamReviewKPI
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [AllowNull]
        public Guid? KPIId { get; set; }
        [ForeignKey("KPIId")]
        [AllowNull]
        public virtual KPI KPI { get; set; } = null;

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [AllowNull]
        public DateTime? DateDuyet { get; set; }
        public byte SerialNumber { get; set; }
    }
}
