using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using static ERP.Data.MyDbContext;

namespace ERP.Models.Default
{
    public class ThongBaoHeThong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("DonVi")]
        public Guid? DonVi_Id { get; set; }
        public DonVi DonVi { get; set; }
        [ForeignKey("PhanMem")]
        public Guid PhanMem_Id { get; set; }
        public PhanMem PhanMem { get; set; }
        [ForeignKey("UserCreated")]
        public Guid User_Id { get; set; }
        public ApplicationUser UserCreated { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(400)]
        public string Body { get; set; }
        [StringLength(250)]
        public string DuongDan { get; set; }
        public DateTime ThoiGian { get; set; }
        [StringLength(50)]
        public string Icon { get; set; }
        public bool IsDaXem { get; set; } = false;
    }
}