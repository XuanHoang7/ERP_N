using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IStreamReviewKPIRepository : IRepository<StreamReviewKPIRepository>
    {

    }
    public class StreamReviewKPIRepository : Repository<StreamReviewKPIRepository>, IStreamReviewKPIRepository
    {
        public StreamReviewKPIRepository(MyDbContext _db) : base(_db)
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
