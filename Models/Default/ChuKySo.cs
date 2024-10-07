using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.Default
{
    public class ChuKySo : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string HinhAnhChuKySo { get; set; }
        public Guid User_Id { get; set; }
    }
}
