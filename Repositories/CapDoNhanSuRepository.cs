using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface ICapDoNhanSuRepository : IRepository<CapDoNhanSu>
    {

    }
    public class CapDoNhanSuRepository : Repository<CapDoNhanSu>, ICapDoNhanSuRepository
    {
        public CapDoNhanSuRepository(MyDbContext _db) : base(_db)
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
