using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.ChiTieuKPI;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucDuyetRepository : IRepository<DanhMucDuyet>
    {

    }
    public class DanhMucDuyetRepository : Repository<DanhMucDuyet>, IDanhMucDuyetRepository
    {
        public DanhMucDuyetRepository(MyDbContext _db) : base(_db)
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
