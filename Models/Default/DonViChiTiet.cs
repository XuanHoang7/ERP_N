using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ERP.Models.Default
{
    public class DonViChiTiet : Auditable
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
        [StringLength(30)]
        public string SDT { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(30)]
        public string Fax { get; set; }
        [StringLength(250)]
        public string DiaChi { get; set; }
        [StringLength(100)]
        public string NguoiLienHe { get; set; }
        [StringLength(30)]
        public string SDTNguoiLienHe { get; set; }
        [StringLength(30)]
        public string MaSoThue { get; set; }
    }
}