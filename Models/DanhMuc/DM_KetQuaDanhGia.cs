using static ERP.Data.MyDbContext;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using ERP.Models.Default;

namespace ERP.Models.DanhMuc
{
    public class DM_KetQuaDanhGia : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string KetQuaDanhGia { get; set; }
        public int ThuTu { get; set; }
    }
}
