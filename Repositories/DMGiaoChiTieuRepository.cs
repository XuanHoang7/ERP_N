﻿using ERP.Data;
using ERP.Infrastructure;
using ERP.Models.DanhMuc;

namespace ERP.Repositories
{
    public interface IDMGiaoChiTieuRepository : IRepository<DMGiaoChiTieu>
    {
    }
    public class DMGiaoChiTieuRepository : Repository<DMGiaoChiTieu>, IDMGiaoChiTieuRepository
    {
        public DMGiaoChiTieuRepository(MyDbContext _db) : base(_db)
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
