using ERP.Data;
using ERP.Infrastructure;
using static ERP.Data.MyDbContext;

namespace ERP.Repositories
{
    public interface IRoleUserRepository  : IRepository<ApplicationUserRole>
    {
    }
    public class RoleUserRepository : Repository<ApplicationUserRole>, IRoleUserRepository
    {
        public RoleUserRepository(MyDbContext _db) : base(_db)
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
