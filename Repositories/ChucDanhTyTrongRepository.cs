using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IChucDanhTyTrongRepository : IRepository<ChucDanhTyTrong>
    {

    }
    public class ChucDanhTyTrongRepository : Repository<ChucDanhTyTrong>, IChucDanhTyTrongRepository
    {
        public ChucDanhTyTrongRepository(MyDbContext _db) : base(_db)
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
