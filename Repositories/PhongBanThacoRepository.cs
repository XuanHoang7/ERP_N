using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;
namespace ERP.Repositories
{
    public interface IPhongBanThacoRepository : IRepository<PhongBanThaco>
	{
	}
	public class PhongBanThacoRepository : Repository<PhongBanThaco>, IPhongBanThacoRepository
	{
		public PhongBanThacoRepository(MyDbContext _db) : base(_db)
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

