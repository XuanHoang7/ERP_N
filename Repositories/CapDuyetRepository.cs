using ERP.Data;
using ERP.Infrastructure;
using ERP.Models;
using ERP.Models.ChiTieuKPI;

namespace ERP.Repositories
{
    public interface ICapDuyetRepository : IRepository<CapDuyet>
    {

    }
    public class CapDuyetRepository : Repository<CapDuyet>, ICapDuyetRepository
    {
        public CapDuyetRepository(MyDbContext _db) : base(_db)
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
