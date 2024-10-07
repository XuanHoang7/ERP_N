using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDM_KetQuaDanhGiaRepository : IRepository<DM_KetQuaDanhGia>
    {
    }
    public class DM_KetQuaDanhGiaRepository : Repository<DM_KetQuaDanhGia>, IDM_KetQuaDanhGiaRepository
    {
        public DM_KetQuaDanhGiaRepository(MyDbContext _db) : base(_db)
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
