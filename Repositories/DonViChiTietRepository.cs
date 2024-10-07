using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IDonViChiTietRepository : IRepository<DonViChiTiet>
    {

    }
    public class DonViChiTietRepository : Repository<DonViChiTiet>, IDonViChiTietRepository
    {
        public DonViChiTietRepository(MyDbContext _db) : base(_db)
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
