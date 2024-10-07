using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DoiTuongApDung : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DanhMucPIChiTietId { get; set; }
        [ForeignKey("DanhMucPIChiTietId")]
        public virtual DanhMucPIChiTiet DanhMucPIChiTiet { get; set; }
        public NhomChucDanh NhomChucDanh { get; set; }

        public Guid ChucDanhId { get; set; }
        [ForeignKey("ChucDanhId")]
        public virtual ChucDanh ChucDanh { get; set; }
    }
}
