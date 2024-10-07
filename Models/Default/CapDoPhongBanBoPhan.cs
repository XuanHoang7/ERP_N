using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ERP.Models.Default
{
    public class CapDoPhongBanBoPhan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(120)]
        [Required]
        public string TenCapDoPhongBanBoPhan { get; set; }
        public int Level { get; set; }
    }
}