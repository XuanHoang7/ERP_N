using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;
namespace ERP.Repositories
{
    public interface Ivptq_kpi_DonViKPIRepository : IRepository<vptq_kpi_DonViKPI>
	{
	}
	public class vptq_kpi_DonViKPIRepository : Repository<vptq_kpi_DonViKPI>, Ivptq_kpi_DonViKPIRepository
	{
		public vptq_kpi_DonViKPIRepository(MyDbContext _db) : base(_db)
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

