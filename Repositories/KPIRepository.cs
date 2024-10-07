using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.ChiTieuKPI;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IKPIRepository : IRepository<KPI>
    {
    }
    public class KPIRepository : Repository<KPI>, IKPIRepository
    {
        public KPIRepository(MyDbContext _db) : base(_db)
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
