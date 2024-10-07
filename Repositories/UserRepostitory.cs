using ERP.Data;
using ERP.Infrastructure;
using ERP.Models;
using Microsoft.AspNetCore.Identity;
using static ERP.Data.MyDbContext;

namespace ERP.Repositories
{
    public interface IUserRepostitory : IRepository<ApplicationUser>
    {
    }
    public class UserRepostitory : Repository<ApplicationUser>, IUserRepostitory
    {
        public UserRepostitory(MyDbContext _db) : base(_db)
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
