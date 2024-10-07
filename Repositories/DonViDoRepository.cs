using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDonViDoRepository : IRepository<DonViDo>
    {
    }
    public class DonViDoRepository : Repository<DonViDo>, IDonViDoRepository
    {
        public DonViDoRepository(MyDbContext _db) : base(_db)
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
