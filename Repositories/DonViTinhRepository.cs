using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IDonViTinhRepository : IRepository<DonViTinh>
    {

    }
    public class DonViTinhRepository : Repository<DonViTinh>, IDonViTinhRepository
    {
        public DonViTinhRepository(MyDbContext _db) : base(_db)
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