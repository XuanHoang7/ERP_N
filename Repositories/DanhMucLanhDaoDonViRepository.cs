using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucLanhDaoDonViRepository : IRepository<DM_LanhDaoDonVi>
    {

    }
    public class DanhMucLanhDaoDonViRepository : Repository<DM_LanhDaoDonVi>, IDanhMucLanhDaoDonViRepository
    {
        public DanhMucLanhDaoDonViRepository(MyDbContext _db) : base(_db)
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
