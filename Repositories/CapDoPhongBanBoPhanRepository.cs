using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface ICapDoPhongBanBoPhanRepository : IRepository<CapDoPhongBanBoPhan>
    {

    }
    public class CapDoPhongBanBoPhanRepository : Repository<CapDoPhongBanBoPhan>, ICapDoPhongBanBoPhanRepository
    {
        public CapDoPhongBanBoPhanRepository(MyDbContext _db) : base(_db)
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