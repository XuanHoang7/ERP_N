using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDanhMucUyQuyenRepository : IRepository<DanhMucUyQuyen>
    {

    }
    public class DanhMucUyQuyenRepository : Repository<DanhMucUyQuyen>, IDanhMucUyQuyenRepository
    {
        public DanhMucUyQuyenRepository(MyDbContext _db) : base(_db)
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
