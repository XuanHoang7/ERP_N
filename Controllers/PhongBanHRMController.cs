using ERP.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThacoLibs;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PhongBanHRMController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly DbAdapter dbAdapter;
        public PhongBanHRMController(IConfiguration _configuration, IUnitofWork _uow)
        {
            uow = _uow;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            dbAdapter = new DbAdapter(connectionString);
        }
        public class Class_PhongBanHRMPost
        {
            public Guid Id { get; set; }
            [StringLength(30)]
            public string MaPhongBanHRM { get; set; }
            public Guid DonViHRM_Id { get; set; }
            public string TenDonViHRM { get; set; }
            public Guid DonVi_Id { get; set; }
            public string MaDonVi { get; set; }
            public string TenCapDoPhongBanBoPhanLevel1 { get; set; }
            public string TenCapDoPhongBanBoPhanLevel2 { get; set; }
            public string TenCapDoPhongBanBoPhanLevel3 { get; set; }
            public string TenCapDoPhongBanBoPhanLevel4 { get; set; }
            public string TenCapDoPhongBanBoPhanLevel5 { get; set; }
            public string TenCapDoPhongBanBoPhanLevel6 { get; set; }
            public string TenCapDoPhongBanBoPhanLevel7 { get; set; }
            public string TenCapDoPhongBanBoPhanLevel8 { get; set; }
            public Guid CapDoPhongBanBoPhanLevel1_Id { get; set; }
            public Guid? CapDoPhongBanBoPhanLevel2_Id { get; set; }
            public Guid? CapDoPhongBanBoPhanLevel3_Id { get; set; }
            public Guid? CapDoPhongBanBoPhanLevel4_Id { get; set; }
            public Guid? CapDoPhongBanBoPhanLevel5_Id { get; set; }
            public Guid? CapDoPhongBanBoPhanLevel6_Id { get; set; }
            public Guid? CapDoPhongBanBoPhanLevel7_Id { get; set; }
            public Guid? CapDoPhongBanBoPhanLevel8_Id { get; set; }
            [StringLength(120)]
            public string TenPhongBan { get; set; }
            public string GhiChuImport { get; set; }
        }

        public class Class_CapDoPhongBanBoPhan
        {
            public Guid Id { get; set; }
            [StringLength(120)]
            [Required]
            public string TenCapDoPhongBanBoPhan { get; set; }
            public int Level { get; set; }
        }

        public class Class_DonViHRM
        {
            public Guid Id { get; set; }
            [StringLength(120)]
            [Required]
            public string TenDonViHRM { get; set; }
        }

        [HttpPost]
        public ActionResult ImportExcel(List<Class_PhongBanHRMPost> data)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var donviHRMs = uow.DonViHRMs.GetAll(x => true);
                var donvis = uow.DonVis.GetAll(x => !x.IsDeleted);
                var capdos = uow.CapDoPhongBanBoPhans.GetAll(x => true);
                List<Class_PhongBanHRMPost> list_Errors = new();
                List<Class_PhongBanHRMPost> list_phongBanHRM = new();
                List<Class_CapDoPhongBanBoPhan> list_CapDo = new();
                List<Class_DonViHRM> list_DonViHRM = new();
                foreach (var item in data)
                {
                    if (string.IsNullOrWhiteSpace(item.MaPhongBanHRM))
                    {
                        item.GhiChuImport = $"Mã phòng ban trực thuộc không được để trống!";
                        list_Errors.Add(item);
                        continue;
                    }
                    if (item.MaPhongBanHRM.Length <= 3)
                    {
                        item.GhiChuImport = $"Mã phòng ban trực thuộc không hợp lệ!";
                        list_Errors.Add(item);
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.TenDonViHRM))
                    {
                        item.GhiChuImport = "Tên đơn vị HRM không được để trống!";
                        list_Errors.Add(item);
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.MaDonVi))
                    {
                        item.GhiChuImport = "Mã đơn vị không được để trống!";
                        list_Errors.Add(item);
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenDonViHRM))
                    {
                        item.TenDonViHRM = item.TenDonViHRM.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var donvihrm = donviHRMs.FirstOrDefault(x => x.TenDonViHRM == item.TenDonViHRM.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim());
                        if (donvihrm == null)
                        {
                            if (list_DonViHRM.Exists(x => x.TenDonViHRM == item.TenDonViHRM))
                            {
                                item.DonViHRM_Id = list_DonViHRM.FirstOrDefault(x => x.TenDonViHRM == item.TenDonViHRM).Id;
                            }
                            else
                            {
                                Class_DonViHRM newdonvihrm = new();
                                Guid iddonvihrm = Guid.NewGuid();
                                newdonvihrm.Id = iddonvihrm;
                                newdonvihrm.TenDonViHRM = item.TenDonViHRM;
                                list_DonViHRM.Add(newdonvihrm);
                                item.DonViHRM_Id = iddonvihrm;
                            }
                        }
                        else
                        {
                            item.DonViHRM_Id = donvihrm.Id;
                        }
                    }
                    item.MaDonVi = item.MaDonVi.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim().ToUpper();
                    if (!string.IsNullOrEmpty(item.MaDonVi))
                    {
                        var donvi = donvis.FirstOrDefault(x => x.MaDonVi.ToUpper() == item.MaDonVi);
                        if (donvi == null)
                        {
                            item.GhiChuImport = $"Mã đơn vị {item.MaDonVi} không tồn tại!";
                            list_Errors.Add(item);
                            continue;
                        }
                        item.DonVi_Id = donvi.Id;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel1))
                    {
                        item.TenCapDoPhongBanBoPhanLevel1 = item.TenCapDoPhongBanBoPhanLevel1.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel1 && x.Level == 1);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel1 && x.Level == 1))
                            {
                                item.CapDoPhongBanBoPhanLevel1_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel1 && x.Level == 1).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel1;
                                newcapdo.Level = 1;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel1_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel1_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel1;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel2))
                    {
                        item.TenCapDoPhongBanBoPhanLevel2 = item.TenCapDoPhongBanBoPhanLevel2.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel2 && x.Level == 2);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel2 && x.Level == 2))
                            {
                                item.CapDoPhongBanBoPhanLevel2_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel2 && x.Level == 2).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel2;
                                newcapdo.Level = 2;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel2_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel2_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel2;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel3))
                    {
                        item.TenCapDoPhongBanBoPhanLevel3 = item.TenCapDoPhongBanBoPhanLevel3.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel3 && x.Level == 3);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel3 && x.Level == 3))
                            {
                                item.CapDoPhongBanBoPhanLevel3_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel3 && x.Level == 3).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel3;
                                newcapdo.Level = 3;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel3_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel3_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel3;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel4))
                    {
                        item.TenCapDoPhongBanBoPhanLevel4 = item.TenCapDoPhongBanBoPhanLevel4.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel4 && x.Level == 4);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel4 && x.Level == 4))
                            {
                                item.CapDoPhongBanBoPhanLevel4_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel4 && x.Level == 4).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel4;
                                newcapdo.Level = 4;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel4_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel4_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel4;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel5))
                    {
                        item.TenCapDoPhongBanBoPhanLevel5 = item.TenCapDoPhongBanBoPhanLevel5.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel5 && x.Level == 5);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel5 && x.Level == 5))
                            {
                                item.CapDoPhongBanBoPhanLevel5_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel5 && x.Level == 5).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel5;
                                newcapdo.Level = 5;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel5_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel5_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel5;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel6))
                    {
                        item.TenCapDoPhongBanBoPhanLevel6 = item.TenCapDoPhongBanBoPhanLevel6.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel6 && x.Level == 6);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel6 && x.Level == 6))
                            {
                                item.CapDoPhongBanBoPhanLevel6_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel6 && x.Level == 6).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel6;
                                newcapdo.Level = 6;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel6_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel6_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel6;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel7))
                    {
                        item.TenCapDoPhongBanBoPhanLevel7 = item.TenCapDoPhongBanBoPhanLevel7.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel7 && x.Level == 7);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel7 && x.Level == 7))
                            {
                                item.CapDoPhongBanBoPhanLevel7_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel7 && x.Level == 7).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel7;
                                newcapdo.Level = 7;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel7_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel7_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel7;
                    }
                    if (!string.IsNullOrWhiteSpace(item.TenCapDoPhongBanBoPhanLevel8))
                    {
                        item.TenCapDoPhongBanBoPhanLevel8 = item.TenCapDoPhongBanBoPhanLevel8.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                        var capdo = capdos.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel8 && x.Level == 8);
                        if (capdo == null)
                        {
                            if (list_CapDo.Exists(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel8 && x.Level == 8))
                            {
                                item.CapDoPhongBanBoPhanLevel8_Id = list_CapDo.FirstOrDefault(x => x.TenCapDoPhongBanBoPhan == item.TenCapDoPhongBanBoPhanLevel8 && x.Level == 8).Id;
                            }
                            else
                            {
                                Class_CapDoPhongBanBoPhan newcapdo = new();
                                Guid idcapdo = Guid.NewGuid();
                                newcapdo.Id = idcapdo;
                                newcapdo.TenCapDoPhongBanBoPhan = item.TenCapDoPhongBanBoPhanLevel8;
                                newcapdo.Level = 8;
                                list_CapDo.Add(newcapdo);
                                item.CapDoPhongBanBoPhanLevel8_Id = idcapdo;
                            }
                        }
                        else
                        {
                            item.CapDoPhongBanBoPhanLevel8_Id = capdo.Id;
                        }
                        item.TenPhongBan = item.TenCapDoPhongBanBoPhanLevel8;
                    }
                    Class_PhongBanHRMPost newphongbanhrm = new();
                    Guid idphongbanhrm = Guid.NewGuid();
                    newphongbanhrm.Id = idphongbanhrm;
                    newphongbanhrm.MaPhongBanHRM = item.MaPhongBanHRM.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
                    newphongbanhrm.DonViHRM_Id = item.DonViHRM_Id;
                    newphongbanhrm.DonVi_Id = item.DonVi_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel1_Id = item.CapDoPhongBanBoPhanLevel1_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel2_Id = item.CapDoPhongBanBoPhanLevel2_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel3_Id = item.CapDoPhongBanBoPhanLevel3_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel4_Id = item.CapDoPhongBanBoPhanLevel4_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel5_Id = item.CapDoPhongBanBoPhanLevel5_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel6_Id = item.CapDoPhongBanBoPhanLevel6_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel7_Id = item.CapDoPhongBanBoPhanLevel7_Id;
                    newphongbanhrm.CapDoPhongBanBoPhanLevel8_Id = item.CapDoPhongBanBoPhanLevel8_Id;
                    newphongbanhrm.TenPhongBan = item.TenPhongBan;
                    list_phongBanHRM.Add(newphongbanhrm);
                }
                if (list_Errors.Any())
                    return StatusCode(StatusCodes.Status409Conflict, list_Errors);
                var result = 0;
                dbAdapter.connect();
                dbAdapter.createStoredProceder("sp_Delete_PhongBanHRM");
                result += dbAdapter.runStoredNoneQuery();
                foreach (var item in list_DonViHRM)
                {
                    dbAdapter.createStoredProceder("sp_Post_DonViHRM");
                    dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@TenDonViHRM", SqlDbType.NVarChar).Value = item.TenDonViHRM;
                    result += dbAdapter.runStoredNoneQuery();
                }
                foreach (var item in list_CapDo)
                {
                    dbAdapter.createStoredProceder("sp_Post_CapDoPhongBanBoPhan");
                    dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@TenCapDoPhongBanBoPhan", SqlDbType.NVarChar).Value = item.TenCapDoPhongBanBoPhan;
                    dbAdapter.sqlCommand.Parameters.Add("@Level", SqlDbType.Int).Value = item.Level;
                    result += dbAdapter.runStoredNoneQuery();
                }
                foreach (var item in list_phongBanHRM)
                {
                    dbAdapter.createStoredProceder("sp_Post_PhongBanHRM");
                    dbAdapter.sqlCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    dbAdapter.sqlCommand.Parameters.Add("@MaPhongBanHRM", SqlDbType.NVarChar).Value = item.MaPhongBanHRM;
                    dbAdapter.sqlCommand.Parameters.Add("@DonViHRM_Id", SqlDbType.UniqueIdentifier).Value = item.DonViHRM_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@DonVi_Id", SqlDbType.UniqueIdentifier).Value = item.DonVi_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel1_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel1_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel2_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel2_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel3_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel3_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel4_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel4_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel5_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel5_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel6_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel6_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel7_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel7_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@CapDoPhongBanBoPhanLevel8_Id", SqlDbType.UniqueIdentifier).Value = item.CapDoPhongBanBoPhanLevel8_Id;
                    dbAdapter.sqlCommand.Parameters.Add("@TenPhongBan", SqlDbType.NVarChar).Value = item.TenPhongBan;
                    result += dbAdapter.runStoredNoneQuery();
                }
                dbAdapter.deConnect();
                return Ok();
            }
        }

        [HttpGet]
        public ActionResult Get(string maPhongBanHRM, Guid? donViHRM_Id, Guid? donVi_Id)
        {
            var capdo = uow.CapDoPhongBanBoPhans.GetAll(x => true).ToDictionary(x => x.Id, x => x.TenCapDoPhongBanBoPhan);
            if (maPhongBanHRM == null) maPhongBanHRM = "";
            var result = uow.PhongBanHRMs
                        .GetAll(x => (
                                maPhongBanHRM == null || x.MaPhongBanHRM.Contains(maPhongBanHRM)) &&
                                (donViHRM_Id == null || x.DonViHRM_Id == donViHRM_Id) &&
                                (donVi_Id == null || x.DonVi_Id == donVi_Id)
                                , null, new string[] { "DonViHRM", "DonVi" }).Select(x => new
                                {
                                    x.Id,
                                    x.MaPhongBanHRM,
                                    x.DonViHRM_Id,
                                    x.DonVi_Id,
                                    x.DonVi.TenDonVi,
                                    x.DonVi.MaDonVi,
                                    x.DonViHRM.TenDonViHRM,
                                    x.CapDoPhongBanBoPhanLevel1_Id,
                                    x.CapDoPhongBanBoPhanLevel2_Id,
                                    x.CapDoPhongBanBoPhanLevel3_Id,
                                    x.CapDoPhongBanBoPhanLevel4_Id,
                                    x.CapDoPhongBanBoPhanLevel5_Id,
                                    x.CapDoPhongBanBoPhanLevel6_Id,
                                    x.CapDoPhongBanBoPhanLevel7_Id,
                                    x.CapDoPhongBanBoPhanLevel8_Id,
                                    TenCapDoPhongBanBoPhanLevel1 = capdo[x.CapDoPhongBanBoPhanLevel1_Id],
                                    TenCapDoPhongBanBoPhanLevel2 = x.CapDoPhongBanBoPhanLevel2_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel2_Id.Value] : "",
                                    TenCapDoPhongBanBoPhanLevel3 = x.CapDoPhongBanBoPhanLevel3_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel3_Id.Value] : "",
                                    TenCapDoPhongBanBoPhanLevel4 = x.CapDoPhongBanBoPhanLevel4_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel4_Id.Value] : "",
                                    TenCapDoPhongBanBoPhanLevel5 = x.CapDoPhongBanBoPhanLevel5_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel5_Id.Value] : "",
                                    TenCapDoPhongBanBoPhanLevel6 = x.CapDoPhongBanBoPhanLevel6_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel6_Id.Value] : "",
                                    TenCapDoPhongBanBoPhanLevel7 = x.CapDoPhongBanBoPhanLevel7_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel7_Id.Value] : "",
                                    TenCapDoPhongBanBoPhanLevel8 = x.CapDoPhongBanBoPhanLevel8_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel8_Id.Value] : "",
                                    x.TenPhongBan
                                }).OrderBy(x => x.MaPhongBanHRM);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult GetById(Guid id)
        {
            var capdo = uow.CapDoPhongBanBoPhans.GetAll(x => true).ToDictionary(x => x.Id, x => x.TenCapDoPhongBanBoPhan);
            var result = uow.PhongBanHRMs
            .GetAll(x => x.Id == id
                    , null, new string[] { "DonViHRM", "DonVi" }).Select(x => new
                    {
                        x.Id,
                        x.MaPhongBanHRM,
                        x.DonViHRM_Id,
                        x.DonVi_Id,
                        x.DonVi.TenDonVi,
                        x.DonVi.MaDonVi,
                        x.DonViHRM.TenDonViHRM,
                        x.CapDoPhongBanBoPhanLevel1_Id,
                        x.CapDoPhongBanBoPhanLevel2_Id,
                        x.CapDoPhongBanBoPhanLevel3_Id,
                        x.CapDoPhongBanBoPhanLevel4_Id,
                        x.CapDoPhongBanBoPhanLevel5_Id,
                        x.CapDoPhongBanBoPhanLevel6_Id,
                        x.CapDoPhongBanBoPhanLevel7_Id,
                        x.CapDoPhongBanBoPhanLevel8_Id,
                        TenCapDoPhongBanBoPhanLevel1 = capdo[x.CapDoPhongBanBoPhanLevel1_Id],
                        TenCapDoPhongBanBoPhanLevel2 = x.CapDoPhongBanBoPhanLevel2_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel2_Id.Value] : "",
                        TenCapDoPhongBanBoPhanLevel3 = x.CapDoPhongBanBoPhanLevel3_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel3_Id.Value] : "",
                        TenCapDoPhongBanBoPhanLevel4 = x.CapDoPhongBanBoPhanLevel4_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel4_Id.Value] : "",
                        TenCapDoPhongBanBoPhanLevel5 = x.CapDoPhongBanBoPhanLevel5_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel5_Id.Value] : "",
                        TenCapDoPhongBanBoPhanLevel6 = x.CapDoPhongBanBoPhanLevel6_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel6_Id.Value] : "",
                        TenCapDoPhongBanBoPhanLevel7 = x.CapDoPhongBanBoPhanLevel7_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel7_Id.Value] : "",
                        TenCapDoPhongBanBoPhanLevel8 = x.CapDoPhongBanBoPhanLevel8_Id.HasValue ? capdo[x.CapDoPhongBanBoPhanLevel8_Id.Value] : "",
                        x.TenPhongBan
                    }).FirstOrDefault();
            return Ok(result);
        }

        [HttpPost("file-mau-import")]
        public ActionResult ImportFileExcel()
        {
            lock (Commons.LockObjectState)
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo("Uploads/Templates/DMPhongBanHRM.xlsx")))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Đơn vị"];
                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Đơn vị");
                    }
                    int indexrow = 3;
                    int stt = 1;
                    var donvi = uow.DonVis.GetAll(x => !x.IsDeleted).OrderBy(x => x.MaDonVi);
                    foreach (var item in donvi)
                    {
                        worksheet.InsertRow(indexrow, 1, 2);
                        worksheet.Row(indexrow).Height = 25;
                        worksheet.Cells["A" + indexrow].Value = stt;
                        worksheet.Cells["B" + indexrow].Value = item.TenDonVi;
                        worksheet.Cells["C" + indexrow].Value = item.MaDonVi;
                        ++indexrow;
                        ++stt;
                    }
                    worksheet.DeleteRow(2, 1);

                    ExcelWorksheet worksheet1 = package.Workbook.Worksheets["DM Daotao"];
                    if (worksheet1 == null)
                    {
                        worksheet1 = package.Workbook.Worksheets.Add("DM Daotao");
                    }
                    indexrow = 3;
                    int stt1 = 1;
                    var phongban = uow.PhongBanHRMs
                        .GetAll(x => true, null, new string[] { "DonViHRM", "DonVi" })
                        .OrderBy(x => x.MaPhongBanHRM).GroupBy(x => new
                        {
                            x.DonViHRM.TenDonViHRM,
                            x.DonVi.TenDonVi,
                            x.DonVi.MaDonVi
                        }).Select(x => new
                        {
                            x.Key.TenDonViHRM,
                            x.Key.TenDonVi,
                            x.Key.MaDonVi
                        });
                    foreach (var item in phongban)
                    {
                        worksheet1.InsertRow(indexrow, 1, 2);
                        worksheet1.Row(indexrow).Height = 25;
                        worksheet1.Cells["A" + indexrow].Value = item.TenDonVi;
                        worksheet1.Cells["B" + indexrow].Value = item.TenDonViHRM;
                        worksheet1.Cells["C" + indexrow].Value = item.MaDonVi;
                        ++indexrow;
                        ++stt1;
                    }
                    worksheet1.DeleteRow(2, 1);
                    return Ok(new { dataexcel = package.GetAsByteArray() });
                }
            }
        }


        [HttpGet("list-don-vi-hrm")]
        public ActionResult GetListDonViHRM()
        {
            var result = uow.DonViHRMs.GetAll(x => true).OrderBy(x => x.TenDonViHRM);
            return Ok(result);
        }

    }
}
