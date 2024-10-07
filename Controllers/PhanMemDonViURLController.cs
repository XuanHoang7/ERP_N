using ERP.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static ERP.Data.MyDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using ThacoLibs;
using ERP.Models.Default;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PhanMemDonViURLController : ControllerBase
    {
        private readonly IUnitofWork uow;
        private readonly RoleManager<ApplicationRole> roleManager;
        public PhanMemDonViURLController(IUnitofWork _uow, RoleManager<ApplicationRole> _roleManager)
        {
            uow = _uow;
            roleManager = _roleManager;
        }
        public class ClassPhanMemDonViURLPut
        {
            [Required]
            public Guid? DonVi_Id { get; set; }
            [Required]
            public Guid? PhanMem_Id { get; set; }
            [StringLength(250)]
            [Required]
            public string DuongDan { get; set; }
            public bool IsERP { get; set; } = false;
        }
        [HttpPut]
        public ActionResult Put(ClassPhanMemDonViURLPut data)
        {
            lock (Commons.LockObjectState)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var exit = uow.PhanMemDonViURLs.FirstOrDefault(x => x.DonVi_Id == data.DonVi_Id && x.PhanMem_Id == data.PhanMem_Id);
                if (exit == null)
                {
                    PhanMemDonViURL phanMemDonViURL = new();
                    phanMemDonViURL.Id = Guid.NewGuid();
                    phanMemDonViURL.DonVi_Id = data.DonVi_Id.Value;
                    phanMemDonViURL.PhanMem_Id = data.PhanMem_Id.Value;
                    phanMemDonViURL.DuongDan = data.DuongDan;
                    phanMemDonViURL.IsERP = data.IsERP;
                    uow.PhanMemDonViURLs.Add(phanMemDonViURL);
                }
                else
                {
                    exit.DonVi_Id = data.DonVi_Id.Value;
                    exit.PhanMem_Id = data.PhanMem_Id.Value;
                    exit.DuongDan = data.DuongDan;
                    exit.IsERP = data.IsERP;
                    uow.PhanMemDonViURLs.Update(exit);
                }
                uow.Complete();
                return Ok();
            }
        }
        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            lock (Commons.LockObjectState)
            {
                var exit = uow.PhanMemDonViURLs.GetById(id);
                if (exit == null)
                {
                    return NotFound();
                }
                var role = roleManager.Roles.FirstOrDefault(x => !x.IsDeleted && x.PhanMem_Id == exit.PhanMem_Id && x.DonVi_Id == exit.DonVi_Id);
                if (role != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Phần mềm theo đơn vị đã tạo quyền không thể xóa!");
                }
                uow.PhanMemDonViURLs.Delete(id);
                uow.Complete();
                return Ok();
            }
        }
        [HttpGet]
        public ActionResult Get(Guid? donVi_Id, Guid? phanMem_Id)
        {
            var data = uow.PhanMemDonViURLs.GetAll(x => (phanMem_Id == null || x.PhanMem_Id == phanMem_Id) && (donVi_Id == null || x.DonVi_Id == donVi_Id), null, new string[] { "DonVi", "PhanMem" })
                .Select(x => new
                {
                    x.Id,
                    x.PhanMem_Id,
                    x.PhanMem.TenPhanMem,
                    x.PhanMem.MaPhanMem,
                    x.DonVi_Id,
                    x.DonVi.TenDonVi,
                    x.DonVi.MaDonVi,
                    x.DuongDan,
                    x.IsERP,
                });
            return Ok(data);
        }
        [HttpGet("{id}")]
        public ActionResult GetById(Guid id)
        {
            var x = uow.PhanMemDonViURLs.FirstOrDefault(x => x.Id == id, null, new string[] { "DonVi", "PhanMem" });
            return Ok(new
            {
                x.Id,
                x.PhanMem_Id,
                x.PhanMem.TenPhanMem,
                x.PhanMem.MaPhanMem,
                x.DonVi_Id,
                x.DonVi.TenDonVi,
                x.DonVi.MaDonVi,
                x.DuongDan,
                x.IsERP,
            });
        }

        public class Class_PhanMemDonViURLImport
        {
            [Required]
            public string MaPhanMem { get; set; }
            [Required]
            public string MaDonVi { get; set; }
            [Required]
            public string DuongDan { get; set; }
            public bool IsERP { get; set; }
            public string GhiChuImport { get; set; }
        }
        [HttpPost("import-excel")]
        public ActionResult ImportExcel(List<Class_PhanMemDonViURLImport> data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var phanMem = uow.phanMems.GetAll(x => !x.IsDeleted).ToDictionary(x => x.MaPhanMem, x => x.Id);
            var donVi = uow.DonVis.GetAll(x => !x.IsDeleted).ToDictionary(x => x.MaDonVi, x => x.Id);
            var pmdonvis = uow.PhanMemDonViURLs.GetAll(x => true);
            List<Class_PhanMemDonViURLImport> errors = new();
            //Bắt trùng mã sản phẩm
            var trungs = data.Select(x => new { MaDonVi = x.MaDonVi.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", ""), MaPhanMem = x.MaPhanMem.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "") })
                    .GroupBy(x => new { x.MaPhanMem,x.MaDonVi })
                    .Select(x => new { x.Key.MaPhanMem,x.Key.MaDonVi, SoLuong = x.Count() })
                    .Where(x => x.SoLuong > 1)
                    .Select(x => new {x.MaDonVi,x.MaPhanMem});
            if (trungs.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, data.Where(x => trungs.Select(u => u.MaPhanMem.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "")).Contains(x.MaPhanMem.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "")) && trungs.Select(u => u.MaDonVi.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "")).Contains(x.MaDonVi.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", ""))).Select(x => new Class_PhanMemDonViURLImport()
                {
                    MaPhanMem = x.MaPhanMem,
                    MaDonVi = x.MaDonVi,
                    GhiChuImport = "Mã phần mềm và mã đơn vị tồn tại trùng nhau trong danh sách",
                }));
            }
            foreach (var item in data)
            {
                PhanMemDonViURL pmdv = new()
                {
                    Id = Guid.NewGuid()
                };
                item.MaDonVi = item.MaDonVi.Trim()
                                           .Replace("\n", "")
                                           .Replace("\r", "")
                                           .Replace("\t", "")
                                           .ToUpper();
                if (string.IsNullOrWhiteSpace(item.MaDonVi))
                {
                    item.GhiChuImport = "Mã đơn vị bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                item.MaPhanMem = item.MaPhanMem.Trim()
                                               .Replace("\n", "")
                                               .Replace("\r", "")
                                               .Replace("\t", "")
                                               .ToUpper();
                if (string.IsNullOrWhiteSpace(item.MaPhanMem))
                {
                    item.GhiChuImport = "Mã phần mềm bắt buộc nhập!";
                    errors.Add(item);
                    continue;
                }
                pmdv.PhanMem_Id = phanMem[item.MaPhanMem];
                pmdv.DonVi_Id = donVi[item.MaDonVi];
                pmdv.DuongDan = item.DuongDan;
                pmdv.IsERP = item.IsERP;
                var exit = pmdonvis.FirstOrDefault(x => x.DonVi_Id == pmdv.DonVi_Id && x.PhanMem_Id == pmdv.PhanMem_Id);
                if (exit == null)
                {
                    uow.PhanMemDonViURLs.Add(pmdv);
                }
                else
                {
                    exit.DuongDan = pmdv.DuongDan;
                    exit.IsERP = pmdv.IsERP;
                    uow.PhanMemDonViURLs.Update(exit);
                }
            }
            if (errors.Any())
            {
                return StatusCode(StatusCodes.Status409Conflict, errors);
            }
            uow.Complete();
            return Ok();
        }
        [HttpGet("export-file-excel")]
        public ActionResult ExportFileExcel(bool isUpdate)
        {
            using ExcelPackage package = new(new FileInfo("Uploads/Templates/FileMauImport_PhanMemDonVi.xlsx"));
            if (package.Workbook.Worksheets.Count == 0)
            {
                package.Workbook.Worksheets.Add("Đơn vị");
            }
            ExcelWorksheet worksheet = package.Workbook.Worksheets["Đơn vị"];
            worksheet ??= package.Workbook.Worksheets.Add("Đơn vị");
            int indexrow = 7;
            int stt = 1;
            var donvi = uow.DonVis.GetAll(x => !x.IsDeleted).OrderBy(x => x.MaDonVi);
            foreach (var item in donvi)
            {
                worksheet.InsertRow(indexrow, 1, 6);
                worksheet.Row(indexrow).Height = 25;
                worksheet.Cells["A" + indexrow].Value = stt;
                worksheet.Cells["B" + indexrow].Value = item.MaDonVi;
                worksheet.Cells["C" + indexrow].Value = item.TenDonVi;
                ++indexrow;
                ++stt;
            }
            worksheet.DeleteRow(6, 1);
            worksheet.Cells.AutoFitColumns();
            worksheet = package.Workbook.Worksheets["Phần mềm"];
            worksheet ??= package.Workbook.Worksheets.Add("Phần mềm");
            indexrow = 7;
            stt = 1;
            var phamem = uow.phanMems.GetAll(x => !x.IsDeleted).OrderBy(x => x.MaPhanMem);
            foreach (var item in phamem)
            {
                worksheet.InsertRow(indexrow, 1, 6);
                worksheet.Row(indexrow).Height = 25;
                worksheet.Cells["A" + indexrow].Value = stt;
                worksheet.Cells["B" + indexrow].Value = item.MaPhanMem;
                worksheet.Cells["C" + indexrow].Value = item.TenPhanMem;
                ++indexrow;
                ++stt;
            }
            worksheet.DeleteRow(6, 1);
            worksheet.Cells.AutoFitColumns();
            if(isUpdate)
            {
                worksheet = package.Workbook.Worksheets["import"];
                worksheet ??= package.Workbook.Worksheets.Add("import");
                indexrow = 7;
                stt = 1;
                var pmdv = uow.PhanMemDonViURLs.GetAll(x => true,null,new string[] { "PhanMem", "DonVi" }).OrderBy(x=>x.PhanMem.MaPhanMem).ThenBy(x=>x.DonVi.MaDonVi);
                foreach (var item in pmdv)
                {
                    worksheet.InsertRow(indexrow, 1, 6);
                    worksheet.Row(indexrow).Height = 25;
                    worksheet.Cells["A" + indexrow].Value = stt;
                    worksheet.Cells["B" + indexrow].Value = item.PhanMem.MaPhanMem;
                    worksheet.Cells["C" + indexrow].Value = item.DonVi.MaDonVi;
                    worksheet.Cells["D" + indexrow].Value = item.DuongDan;
                    worksheet.Cells["E" + indexrow].Value = item.IsERP ? "X" : "";
                    ++indexrow;
                    ++stt;
                }
                worksheet.DeleteRow(6, 1);
                worksheet.Cells.AutoFitColumns();
            }
            return Ok(new { dataexcel = package.GetAsByteArray() });
        }

    }
}
