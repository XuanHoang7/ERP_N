using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DM_DonViDanhGia : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(50)]
        public Guid? IdDonViKPI { get; set; }
        [AllowNull]
        [ForeignKey("IdDonViKPI")]
        public vptq_kpi_DonViKPI DonViKPI { get; set; }

        public string TienTo { get; set; }

        public bool DanhGia { get; set; } = true;

    }
}
