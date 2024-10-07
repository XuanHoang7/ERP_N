using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ERP.Models.Default;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Models.DanhMuc
{
    public class DanhMucTyTrong : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string ChuKyDanhGia { get; set; } = "Tháng";
        // Sử dụng Enum cho nhóm chức danh
        public string NhomChucDanh { get; set; } = "Công Ty";
        public bool BatBuocDung { get; set; } = true;
        public bool IsKhong { get; set; } = true;
        public virtual ICollection<ChucDanhTyTrong> ChucDanhs { get; set; }
        public virtual ICollection<ChiTieuTyTrong> ChiTieuTyTrongs { get; set; }
    }
}
