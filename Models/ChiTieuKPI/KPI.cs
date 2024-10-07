using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ERP.Models.DanhMuc;
using ERP.Models.Default;
using static ERP.Data.MyDbContext;

namespace ERP.Models.ChiTieuKPI
{
    public class KPI : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public bool IsIndividual { get; set; } = true;
        [AllowNull]
        public Guid? UserId { get; set; } = null;
        [ForeignKey("UserId")]
        [AllowNull]
        public virtual ApplicationUser User { get; set; } = null;
        public string ThoiDiemDanhGia { get; set; }
        [AllowNull]
        public Guid? DM_DonViDanhGiaId { get; set; } = null;
        [ForeignKey("DM_DonViDanhGiaId")]
        [AllowNull]
        public virtual DM_DonViDanhGia DM_DonViDanhGia { get; set; } = null;
        public bool IsReviewed { get; set; } = false;
        public bool IsRefuse { get; set; } = false;
        [AllowNull]
        public string ReasonForRefuse { get; set; }
        [AllowNull]
        public DateTime? DateRefuse { get; set; }
        public byte SerialNumberNow { get; set; } = 0;
        public bool WillApproved { get; set; } = false;
        public bool IsCompleteApproved { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public bool IsRefuseForApproved { get; set; } = false;
        public bool IsBlock { get; set; } = false;
        public virtual ICollection<StreamReviewKPI> StreamReviewKPIs { get; set; }
        public virtual ICollection<KPIDetail> KPIDetails { get; set; }
        public virtual ICollection<UpdateHistory> UpdateHistorys { get; set; }
    }
}
