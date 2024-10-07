using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using static ERP.Data.MyDbContext;

namespace ERP.Models.DanhMuc
{
    public class DuyetPIStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid DuyetPIId { get; set; }
        [ForeignKey("DuyetPIId")]
        public virtual DuyetPI DuyetPI { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [AllowNull]
        public DateTime? DateDuyet { get; set; }
        public int Serial { get; set; }
    }
}
