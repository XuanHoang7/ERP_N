using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucPIRepository : IRepository<DanhMucPI>
    {

    }
    public class DanhMucPIRepository : Repository<DanhMucPI>, IDanhMucPIRepository
    {
        public DanhMucPIRepository(MyDbContext _db) : base(_db)
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
