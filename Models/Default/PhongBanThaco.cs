using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace ERP.Models.Default
{
    public class PhongBanThaco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } //orgStructureID
        [StringLength(25)]
        public string MaPhongBan { get; set; }
        [StringLength(120)]
        public string TenPhongBan { get; set; }
        public Guid? Parent_Id { get; set; }
        public int CapDo { get; set; }
        public int ThuTuCap1 { get; set; } = 0;
        public int ThuTuCap2 { get; set; } = 0;
        public int ThuTuCap3 { get; set; } = 0;
        public int ThuTuCap4 { get; set; } = 0;
        public int ThuTuCap5 { get; set; } = 0;
        public int ThuTuCap6 { get; set; } = 0;
        public int ThuTuCap7 { get; set; } = 0;
        public int ThuTuCap8 { get; set; } = 0;//Cái nào không có thì ThuTu = 0
        [ForeignKey("DonVi")]
        public Guid? DonVi_Id { get; set; }
        public DonVi DonVi { get; set; }
        [StringLength(120)]
        public string TenCap1 { get; set; }
        [StringLength(120)]
        public string TenCap2 { get; set; }
        [StringLength(120)]
        public string TenCap3 { get; set; }
        [StringLength(120)]
        public string TenCap4 { get; set; }
        [StringLength(120)]
        public string TenCap5 { get; set; }
        [StringLength(120)]
        public string TenCap6 { get; set; }
        [StringLength(120)]
        public string TenCap7 { get; set; }
        [StringLength(120)]
        public string TenCap8 { get; set; }
        [ForeignKey("vptq_kpi_DonViKPI")]
        public Guid? vptq_kpi_DonViKPI_Id { get; set; }
        public vptq_kpi_DonViKPI vptq_kpi_DonViKPI { get; set; }
    }
}
