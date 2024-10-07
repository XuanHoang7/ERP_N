using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using static ERP.Data.MyDbContext;

namespace ERP.Models.ChiTieuKPI
{
    public class CapDuyet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid DanhMucDuyetId { get; set; }
        [ForeignKey("DanhMucDuyetId")]
        public virtual DanhMucDuyet DanhMucDuyet { get; set; } = null;

        public CacCapDuyet CacCapDuyet { get; set; }
        public Guid LanhDaoDuyetId { get; set; }
        [ForeignKey("LanhDaoDuyetId")]
        public virtual ApplicationUser LanhDaoDuyet { get; set; } = null;
    }
}
