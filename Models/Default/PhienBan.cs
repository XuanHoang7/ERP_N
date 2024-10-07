using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ERP.Models.Default
{
    public class PhienBan : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(50)]
        [Required]
        public string MaPhienBan { get; set; }
        [StringLength(250)]
        [Required]
        public string MoTa { get; set; }
        [StringLength(250)]
        public string FileName { get; set; }
        [StringLength(500)]
        public string FileUrl { get; set; }
        public bool IsSuDung { get; set; }
        [ForeignKey("PhanMem")]
        public Guid PhanMem_Id { get; set; }
        public PhanMem PhanMem { get; set; }
        [ForeignKey("DonVi")]
        public Guid DonVi_Id { get; set; }
        public DonVi DonVi { get; set; }
    }
}
