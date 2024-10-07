using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucNSKhongDanhGiaRepository : IRepository<DM_NSKhongDanhGia>
    {

    }
    public class DanhMucNSKhongDanhGiaRepository : Repository<DM_NSKhongDanhGia>, IDanhMucNSKhongDanhGiaRepository
    {
        public DanhMucNSKhongDanhGiaRepository(MyDbContext _db) : base(_db)
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
