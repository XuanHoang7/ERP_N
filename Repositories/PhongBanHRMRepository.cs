
using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IPhongBanHRMRepository : IRepository<PhongBanHRM>
    {

    }
    public class PhongBanHRMRepository : Repository<PhongBanHRM>, IPhongBanHRMRepository
    {
        public PhongBanHRMRepository(MyDbContext _db) : base(_db)
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