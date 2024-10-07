using ERP.Models.Default;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ERP.Data.MyDbContext;

namespace ERP.Models.DanhMuc
{
    public class DuocUyQuyen : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid LanhDaoDuocUyQuyenId { get; set; }
        [ForeignKey("LanhDaoDuocUyQuyenId")]
        public virtual ApplicationUser User { get; set; }
        public Guid DanhMucUyQuyenId { get; set; }
        [ForeignKey("DanhMucUyQuyenId")]
        public virtual DanhMucUyQuyen DanhMucUyQuyen { get; set; }

    }
}
