using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDuyetPIRepository : IRepository<DuyetPI>
    {
    }
    public class DuyetPIRepository : Repository<DuyetPI>, IDuyetPIRepository
    {
        public DuyetPIRepository(MyDbContext _db) : base(_db)
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
