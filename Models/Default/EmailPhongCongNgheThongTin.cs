using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ERP.Models.Default
{
    public class EmailPhongCongNgheThongTin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}