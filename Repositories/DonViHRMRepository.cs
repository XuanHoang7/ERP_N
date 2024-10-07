using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IDonViHRMRepository : IRepository<DonViHRM>
    {

    }
    public class DonViHRMRepository : Repository<DonViHRM>, IDonViHRMRepository
    {
        public DonViHRMRepository(MyDbContext _db) : base(_db)
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