using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IEmailPhongCongNgheThongTinRepository : IRepository<EmailPhongCongNgheThongTin>
    {

    }
    public class EmailPhongCongNgheThongTinRepository : Repository<EmailPhongCongNgheThongTin>, IEmailPhongCongNgheThongTinRepository
    {
        public EmailPhongCongNgheThongTinRepository(MyDbContext _db) : base(_db)
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
