using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucPIChiTietRepository : IRepository<DanhMucPIChiTiet>
    {

    }
    public class DanhMucPIChiTietRepository : Repository<DanhMucPIChiTiet>, IDanhMucPIChiTietRepository
    {
        public DanhMucPIChiTietRepository(MyDbContext _db) : base(_db)
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
