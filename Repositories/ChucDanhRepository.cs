using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IChucDanhRepository : IRepository<ChucDanh>
    {

    }
    public class ChucDanhRepository : Repository<ChucDanh>, IChucDanhRepository
    {
        public ChucDanhRepository(MyDbContext _db) : base(_db)
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
