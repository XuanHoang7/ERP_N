using System;
using System.ComponentModel.DataAnnotations.Schema;
using static ERP.Data.MyDbContext;

namespace ERP.Models
{
    public class Auditable
    {
        public DateTime CreatedDate { get; set; }
        [ForeignKey("UserCreated")]
        public Guid CreatedBy { get; set; }
        public ApplicationUser UserCreated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}