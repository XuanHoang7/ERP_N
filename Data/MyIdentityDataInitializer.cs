using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using static ERP.Data.MyDbContext;
using ERP.Infrastructure;
using ERP.Models;

namespace ERP.Data
{

    public class MyIdentityDataInitializer
    {
        public static IUnitofWork uow;
        public MyIdentityDataInitializer(IUnitofWork _uow)
        {
            uow = _uow;
        }
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Administrator", Description = "Quản trị", CreatedDate = DateTime.Now });
            }
            if (await userManager.FindByIdAsync("c662783d-03c0-4404-9473-1034f1ac1caa") == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.Id = Guid.Parse("c662783d-03c0-4404-9473-1034f1ac1caa");
                user.UserName = "2303032";
                user.Email = "levinhdu@thaco.com.vn";
                user.FullName = "Lê Vinh Dự";
                user.MaNhanVien = "2303032";
                user.IsActive = true;
                user.CreatedDate = DateTime.Now;
                user.PasswordHash = Commons.HashPassword("2303032");
                IdentityResult result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
            if (await userManager.FindByIdAsync("E2567647-49BF-4C25-8F3F-6C26C7BA2D91") == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.Id = Guid.Parse("E2567647-49BF-4C25-8F3F-6C26C7BA2D91");
                user.UserName = "2305994";
                user.Email = "dinhngocphuc@thaco.com.vn";
                user.FullName = "Đinh Ngọc Phúc";
                user.MaNhanVien = "2305994";
                user.IsActive = true;
                user.CreatedDate = DateTime.Now;
                user.PasswordHash = Commons.HashPassword("2305994");
                IdentityResult result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
            if (await userManager.FindByIdAsync("55F8A570-40F4-49CF-B05F-30544D64CB2A") == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.Id = Guid.Parse("55F8A570-40F4-49CF-B05F-30544D64CB2A");
                user.UserName = "2307223";
                user.Email = "ngothanhha@thaco.com.vn";
                user.FullName = "Ngô Thanh Hà";
                user.MaNhanVien = "2307223";
                user.IsActive = true;
                user.CreatedDate = DateTime.Now;
                user.PasswordHash = Commons.HashPassword("2307223");
                IdentityResult result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }
    }
}