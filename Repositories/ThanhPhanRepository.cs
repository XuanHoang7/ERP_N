using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IThanhPhanRepository : IRepository<ThanhPhan>
    {

    }
    public class ThanhPhanRepository : Repository<ThanhPhan>, IThanhPhanRepository
    {
        public ThanhPhanRepository(MyDbContext _db) : base(_db)
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