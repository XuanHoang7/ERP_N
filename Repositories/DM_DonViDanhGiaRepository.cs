using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDM_DonViDanhGiaRepository : IRepository<DM_DonViDanhGia>
    {
    }
    public class DM_DonViDanhGiaRepository : Repository<DM_DonViDanhGia>, IDM_DonViDanhGiaRepository
    {
        public DM_DonViDanhGiaRepository(MyDbContext _db) : base(_db)
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
