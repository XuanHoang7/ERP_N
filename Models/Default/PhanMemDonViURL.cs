using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.Default
{
    public class PhanMemDonViURL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("DonVi")]
        public Guid DonVi_Id { get; set; }
        public DonVi DonVi { get; set; }
        [ForeignKey("PhanMem")]
        public Guid PhanMem_Id { get; set; }
        public PhanMem PhanMem { get; set; }
        [StringLength(250)]
        [Required]
        public string DuongDan { get; set; }
        public bool IsERP { get; set; } = false;
    }
}
