using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ERP.Models.DanhMuc
{
    public class DuyetPI
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DanhMucPIId { get; set; }
        [ForeignKey("DanhMucPIId")]
        public virtual DanhMucPI DanhMucPI { get; set; }
        public bool IsComplete { get; set; } = false;
        public int ThuTuNow { get; set; } = -1;
        public bool IsRefuse { get; set; } = false;

        [AllowNull] 
        public DateTime? DateRefuse { get; set; }

        [AllowNull]
        public string ReasonForRefuse { get; set; }
        public virtual ICollection<DuyetPIStatus> DuyetPIStatuses { get; set; }
    }
}
