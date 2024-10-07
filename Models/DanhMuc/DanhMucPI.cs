using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DanhMucPI : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid IdDMDonViDanhGia { get; set; }
        [ForeignKey("IdDMDonViDanhGia")]
        public virtual DM_DonViDanhGia DM_DonViDanhGia { get; set; }
        public int PhienBan { get; set; }
        public DateTime ApDungDen { get; set; }
        public int SoLuongPI { get; set; }
        [AllowNull]
        public Guid? IdDMLanhDaoDonVi { get; set; }
        [ForeignKey("IdDMLanhDaoDonVi")]
        [AllowNull]
        public virtual DM_LanhDaoDonVi DM_LanhDaoDonVi { get; set; }

        //public virtual ICollection<DanhMucPICauHinhDuyet> DanhMucPICauHinhDuyets { get; set; }
        public virtual ICollection<DanhMucPIChiTiet> DanhMucPIChiTiets { get; set; }
    }
}
