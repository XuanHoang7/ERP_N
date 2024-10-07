using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IPhienBanRepository : IRepository<PhienBan>
    {

    }
    public class PhienBanRepository : Repository<PhienBan>, IPhienBanRepository
    {
        public PhienBanRepository(MyDbContext _db) : base(_db)
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
