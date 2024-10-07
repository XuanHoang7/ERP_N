using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Diagnostics.CodeAnalysis;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DanhMucTrongYeu : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string MaDanhMucTrongYeu { get; set; }

        [MaxLength(255)]
        public string TenDanhMucTrongYeu { get; set; }

        [StringLength(500)]
        [AllowNull]
        public string DienGiai { get; set; }

        public bool TrangThai { get; set; } = true;

        [MaxLength(500)]
        [AllowNull]
        public string GhiChu { get; set; }

    }
}
