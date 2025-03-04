﻿using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.Default;

namespace ERP.Repositories
{
    public interface IPhongbanRepository : IRepository<Phongban>
    {

    }
    public class PhongbanRepository : Repository<Phongban>, IPhongbanRepository
    {
        public PhongbanRepository(MyDbContext _db) : base(_db)
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
