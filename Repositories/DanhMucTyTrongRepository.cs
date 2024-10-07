using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucTyTrongRepository : IRepository<DanhMucTyTrong>
    {

    }
    public class DanhMucTyTrongRepository : Repository<DanhMucTyTrong>, IDanhMucTyTrongRepository
    {
        public DanhMucTyTrongRepository(MyDbContext _db) : base(_db)
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
