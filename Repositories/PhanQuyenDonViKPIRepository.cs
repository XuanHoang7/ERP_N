using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IPhanQuyenDonViKPIRepository : IRepository<PhanQuyenDonViKPI>
    {

    }
    public class PhanQuyenDonViKPIRepository : Repository<PhanQuyenDonViKPI>, IPhanQuyenDonViKPIRepository
    {
        public PhanQuyenDonViKPIRepository(MyDbContext _db) : base(_db)
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
