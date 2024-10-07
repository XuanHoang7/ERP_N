using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDuocUyQuyenRepository : IRepository<DuocUyQuyen>
    {

    }
    public class DuocUyQuyenRepository : Repository<DuocUyQuyen>, IDuocUyQuyenRepository
    {
        public DuocUyQuyenRepository(MyDbContext _db) : base(_db)
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
