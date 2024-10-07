using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IPhanMemRepository : IRepository<PhanMem>
    {

    }
    public class PhanMemRepository : Repository<PhanMem>, IPhanMemRepository
    {
        public PhanMemRepository(MyDbContext _db) : base(_db)
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
