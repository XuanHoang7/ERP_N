using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IChuKySoRepository : IRepository<ChuKySo>
    {

    }
    public class ChuKySoRepository : Repository<ChuKySo>, IChuKySoRepository
    {
        public ChuKySoRepository(MyDbContext _db) : base(_db)
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
