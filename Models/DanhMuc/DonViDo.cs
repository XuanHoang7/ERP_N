using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DonViDo : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Mã không được để trống.")]
        [MinLength(1, ErrorMessage = "Mã không được chứa chỉ khoảng trắng.")]
        public string MaDonViDo { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Tên không được để trống.")]
        [MinLength(1, ErrorMessage = "Tên không được chứa chỉ khoảng trắng.")]
        public string TenDonViDo { get; set; }
    }
}
