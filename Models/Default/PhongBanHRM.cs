using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.Default
{
    public class PhongBanHRM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(30)]
        public string MaPhongBanHRM { get; set; }
        [ForeignKey("DonViHRM")]
        public Guid DonViHRM_Id { get; set; }
        public DonViHRM DonViHRM { get; set; }
        [ForeignKey("DonVi")]
        public Guid DonVi_Id { get; set; }
        public DonVi DonVi { get; set; }
        public Guid CapDoPhongBanBoPhanLevel1_Id { get; set; }
        public Guid? CapDoPhongBanBoPhanLevel2_Id { get; set; }
        public Guid? CapDoPhongBanBoPhanLevel3_Id { get; set; }
        public Guid? CapDoPhongBanBoPhanLevel4_Id { get; set; }
        public Guid? CapDoPhongBanBoPhanLevel5_Id { get; set; }
        public Guid? CapDoPhongBanBoPhanLevel6_Id { get; set; }
        public Guid? CapDoPhongBanBoPhanLevel7_Id { get; set; }
        public Guid? CapDoPhongBanBoPhanLevel8_Id { get; set; }
        [StringLength(120)]
        public string TenPhongBan { get; set; }
    }
}