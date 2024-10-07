using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucNhomPIRepository : IRepository<DanhMucNhomPI>
    {

    }
    public class DanhMucNhomPIRepository : Repository<DanhMucNhomPI>, IDanhMucNhomPIRepository
    {
        public DanhMucNhomPIRepository(MyDbContext _db) : base(_db)
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
