using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDuocGiaoChiTieuRepository : IRepository<DuocGiaoChiTieu>
    {
    }
    public class DuocGiaoChiTieuRepository : Repository<DuocGiaoChiTieu>, IDuocGiaoChiTieuRepository
    {
        public DuocGiaoChiTieuRepository(MyDbContext _db) : base(_db)
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
