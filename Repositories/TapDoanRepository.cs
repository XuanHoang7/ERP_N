using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface ITapDoanRepository : IRepository<TapDoan>
    {

    }
    public class TapDoanRepository : Repository<TapDoan>, ITapDoanRepository
    {
        public TapDoanRepository(MyDbContext _db) : base(_db)
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
