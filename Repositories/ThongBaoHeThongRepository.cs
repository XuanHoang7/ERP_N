using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IThongBaoHeThongRepository : IRepository<ThongBaoHeThong>
    {

    }
    public class ThongBaoHeThongRepository : Repository<ThongBaoHeThong>, IThongBaoHeThongRepository
    {
        public ThongBaoHeThongRepository(MyDbContext _db) : base(_db)
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
