using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Diagnostics.CodeAnalysis;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DanhMucNhomPI : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string MaDanhMucNhomPI { get; set; }

        [MaxLength(255)]
        public string TenDanhMucNhomPI { get; set; }

        [Required]
        public bool TrangThai { get; set; } = true;

        [StringLength(500)]
        [AllowNull]
        public string GhiChu { get; set; }
    }
}
