using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Models.Default;
using static ERP.Data.MyDbContext;

namespace ERP.Models.DanhMuc
{
    public class DanhMucUyQuyen : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid LanhDaoUpQuyenId { get; set; }
        [ForeignKey("LanhDaoUpQuyenId")]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<DuocUyQuyen> DuocUyQuyens { get; set; }
    }
}
