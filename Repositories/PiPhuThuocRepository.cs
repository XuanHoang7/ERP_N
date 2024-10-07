using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IPiPhuThuocRepository : IRepository<PIPhuThuoc>
    {
    }
    public class PiPhuThuocRepository : Repository<PIPhuThuoc>, IPiPhuThuocRepository
    {
        public PiPhuThuocRepository(MyDbContext _db) : base(_db)
        {
        }
        public MyDbContext MyDbContext
        {
            get
            {
                return _db as MyDbContext;
            }
        }
    }
}
