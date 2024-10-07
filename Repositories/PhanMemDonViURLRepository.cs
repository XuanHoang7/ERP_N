using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IPhanMemDonViURLRepository : IRepository<PhanMemDonViURL>
    {

    }
    public class PhanMemDonViURLRepository : Repository<PhanMemDonViURL>, IPhanMemDonViURLRepository
    {
        public PhanMemDonViURLRepository(MyDbContext _db) : base(_db)
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
