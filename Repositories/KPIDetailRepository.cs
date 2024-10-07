using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.ChiTieuKPI;

namespace ERP.Repositories
{
    public interface IKPIDetailRepository : IRepository<KPIDetail>
    {
    }
    public class KPIDetailRepository : Repository<KPIDetail>, IKPIDetailRepository
    {
        public KPIDetailRepository(MyDbContext _db) : base(_db)
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
