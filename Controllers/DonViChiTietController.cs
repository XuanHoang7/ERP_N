using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ERP.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using ERP.Models.Default;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DonViChiTietController : ControllerBase
    {
        private readonly IUnitofWork uow;
        public static IWebHostEnvironment environment;
        public DonViChiTietController(IUnitofWork _uow, IWebHostEnvironment _environment)
        {
            uow = _uow;
            environment = _environment;
        }

        [HttpGet]
        public ActionResult Get(Guid phanMem_Id, Guid donVi_Id)
        {
            var donVi = uow.DonVis.GetById(donVi_Id);
            if (donVi == null)
            {
                return NotFound();
            }
            var donViChiTiet = uow.DonViChiTiets.FirstOrDefault(x => !x.IsDeleted && x.DonVi_Id == donVi_Id && x.PhanMem_Id == phanMem_Id);
            return Ok(new
            {
                DonVi_Id = donVi.Id,
                donVi.MaDonVi,
                donVi.TenDonVi,
                donViChiTiet?.PhanMem_Id,
                donViChiTiet?.SDT,
                donViChiTiet?.Email,
                donViChiTiet?.Fax,
                donViChiTiet?.DiaChi,
                donViChiTiet?.NguoiLienHe,
                donViChiTiet?.SDTNguoiLienHe,
                donViChiTiet?.MaSoThue,
            });
        }
        public class Class_DonViChiTiet
        {
            [Required]
            public Guid DonVi_Id { get; set; }
            [Required]
            public Guid PhanMem_Id { get; set; }
            private string _sDT;
            [StringLength(30)]
            public string SDT
            {
                get { return _sDT; }
                set { _sDT = CleanUp(value); }
            }
            private string _email;
            [StringLength(50)]
            public string Email
            {
                get { return _email; }
                set { _email = CleanUp(value); }
            }
            private string _fax;
            [StringLength(30)]
            public string Fax
            {
                get { return _fax; }
                set { _fax = CleanUp(value); }
            }
            private string _diaChi;
            [StringLength(250)]
            public string DiaChi
            {
                get { return _diaChi; }
                set { _diaChi = CleanUp(value); }
            }
            private string _nguoiLienHe;

            [StringLength(100)]
            public string NguoiLienHe
            {
                get { return _nguoiLienHe; }
                set { _nguoiLienHe = CleanUp(value); }
            }
            private string _sDTNguoiLienHe;
            [StringLength(30)]
            public string SDTNguoiLienHe
            {
                get { return _sDTNguoiLienHe; }
                set { _sDTNguoiLienHe = CleanUp(value); }
            }
            private string _maSoThue;
            [StringLength(30)]
            public string MaSoThue
            {
                get { return _maSoThue; }
                set { _maSoThue = CleanUp(value); }
            }
            public static string CleanUp(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return null;
                return input.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "");
            }
        }
        [HttpPut("{id}")]
        public ActionResult Put(Class_DonViChiTiet data)
        {
            string testEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!string.IsNullOrWhiteSpace(data.Email) && !Regex.IsMatch(data.Email, testEmail))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Email không đúng định dạng");
            }
            DonViChiTiet donViChiTiet = uow.DonViChiTiets.FirstOrDefault(x => !x.IsDeleted && x.DonVi_Id == data.DonVi_Id && x.PhanMem_Id == data.PhanMem_Id);
            if (donViChiTiet == null)
            {
                DonViChiTiet donViChiTietNew = new()
                {
                    Id = Guid.NewGuid(),
                    DonVi_Id = data.DonVi_Id,
                    PhanMem_Id = data.PhanMem_Id,
                    SDT = data.SDT,
                    Email = data.Email,
                    Fax = data.Fax,
                    DiaChi = data.DiaChi,
                    NguoiLienHe = data.NguoiLienHe,
                    SDTNguoiLienHe = data.SDTNguoiLienHe,
                    MaSoThue = data.MaSoThue,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Guid.Parse(User.Identity.Name),
                };
                uow.DonViChiTiets.Add(donViChiTietNew);
                uow.Complete();
            }
            else
            {
                donViChiTiet.SDT = data.SDT;
                donViChiTiet.Email = data.Email;
                donViChiTiet.Fax = data.Fax;
                donViChiTiet.DiaChi = data.DiaChi;
                donViChiTiet.NguoiLienHe = data.NguoiLienHe;
                donViChiTiet.SDTNguoiLienHe = data.SDTNguoiLienHe;
                donViChiTiet.MaSoThue = data.MaSoThue;
                donViChiTiet.IsDeleted = false;
                donViChiTiet.UpdatedDate = DateTime.Now;
                donViChiTiet.UpdatedBy = Guid.Parse(User.Identity.Name);
                uow.DonViChiTiets.Update(donViChiTiet);
                uow.Complete();
            }
            return Ok();
        }
    }
}