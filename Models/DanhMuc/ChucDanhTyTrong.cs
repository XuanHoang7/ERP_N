using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class ChucDanhTyTrong : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ChucDanhId { get; set; }
        [ForeignKey("ChucDanhId")]
        public virtual ChucDanh ChucDanh { get; set; }

        public Guid DanhMucTyTrongId { get; set; }
        [ForeignKey("DanhMucTyTrongId")]
        public virtual DanhMucTyTrong DanhMucTyTrong { get; set; }

    }
}
