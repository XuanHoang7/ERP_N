using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ERP.Infrastructure;
using ERP.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using static ERP.Commons;
using SixLabors.ImageSharp.Formats.Webp;

namespace ERP.Controllers
{
    [EnableCors("CorsApi")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UploadController : ControllerBase
    {
        private readonly IUnitofWork uow;
        public UploadController(IUnitofWork _uow)
        {
            uow = _uow;
        }
        [HttpPost]
        public ActionResult Upload(IFormFile file, string stringPath)
        {
            if (file == null) return StatusCode(StatusCodes.Status409Conflict, "Chưa gửi file!");
            try
            {
                if (!string.IsNullOrEmpty(stringPath))
                {
                    string path = LayDuongDanFile(stringPath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                string tenFile = GetFileName(file.FileName);
                string tenPhanMoRongFile = GetFileExtension(file.FileName);
                string[] TypeImages = { ".png", ".jpg", ".jpeg", ".webp" };
                if (TypeImages.Contains(tenPhanMoRongFile))
                {
                    var fileName = $"{FormatFileName(tenFile)}_{Guid.NewGuid():N}.webp";
                    stringPath = $"Uploads/Image/{DateTime.Now:yyyy/MM}";
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), stringPath, fileName);
                    if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    //Nén ảnh
                    using Image image = Image.Load(fullPath);
                    if (image.Width > 800 || image.Height > 600)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(800, 600),
                            Mode = ResizeMode.Max
                        }));
                    }
                    image.Save(fullPath, new WebpEncoder { Quality = 80 });
                    return Ok(new FileModel { Path = $"/{stringPath}/{fileName}", FileName = file.FileName });
                }
                else
                {
                    var fileName = $"{FormatFileName(tenFile)}_{Guid.NewGuid():N}{tenPhanMoRongFile}";
                    stringPath = $"Uploads/File/{DateTime.Now:yyyy/MM}";
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), stringPath, fileName);
                    if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new FileModel { Path = $"/{stringPath}/{fileName}", FileName = file.FileName });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }
        [HttpPost("Multi")]
        public ActionResult Multi(List<IFormFile> lstFiles)
        {
            if (lstFiles == null || !lstFiles.Any()) return StatusCode(StatusCodes.Status409Conflict, "Chưa gửi file!");
            try
            {
                List<FileModel> lst = new();
                string stringPathFile = $"Uploads/File/{DateTime.Now:yyyy/MM}";
                string stringPathImage = $"Uploads/File/{DateTime.Now:yyyy/MM}";
                string[] TypeImages = { ".png", ".jpg", ".jpeg", ".webp" };
                foreach (var file in lstFiles)
                {
                    string tenFile = GetFileName(file.FileName);
                    string tenPhanMoRongFile = GetFileExtension(file.FileName);
                    if (TypeImages.Contains(tenPhanMoRongFile))
                    {
                        var fileName = $"{FormatFileName(tenFile)}_{Guid.NewGuid():N}.webp";
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), stringPathImage, fileName);
                        if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        }
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        //Nén ảnh
                        using Image image = Image.Load(fullPath);
                        if (image.Width > 800 || image.Height > 600)
                        {
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(800, 600),
                                Mode = ResizeMode.Max
                            }));
                        }
                        image.Save(fullPath, new WebpEncoder { Quality = 80 });
                        lst.Add(new FileModel { Path = "/" + stringPathImage + "/" + fileName, FileName = file.FileName });
                    }
                    else
                    {
                        var fileName = $"{FormatFileName(tenFile)}_{Guid.NewGuid():N}{tenPhanMoRongFile}";
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), stringPathFile, fileName);
                        if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        }
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        lst.Add(new FileModel { Path = "/" + stringPathFile + "/" + fileName, FileName = file.FileName });
                    }
                }
                return Ok(lst);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }
        [HttpPost("Multi/Image")]
        public ActionResult MultiImage(List<IFormFile> lstFiles)
        {
            try
            {
                List<FileModel> lst = new();
                string[] supportedTypes = { ".png", ".jpg", ".jpeg", ".webp" };
                foreach (var file in lstFiles)
                {
                    string tenPhanMoRongFile = GetFileExtension(file.FileName).ToLower();
                    if (!supportedTypes.Contains(tenPhanMoRongFile))
                    {
                        return BadRequest($"Chỉ cho phép ảnh với định dạng: png, jpg, jpeg, webp. File '{file.FileName}' không hợp lệ!");
                    }
                }
                string stringPath = $"Uploads/Image/{DateTime.Now:yyyy/MM}";
                foreach (var file in lstFiles)
                {
                    string tenPhanMoRongFile = GetFileExtension(file.FileName).ToLower();
                    string tenFile = GetFileName(file.FileName);
                    var fileName = $"{FormatFileName(tenFile)}_{Guid.NewGuid():N}{tenPhanMoRongFile}";
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), stringPath, fileName);
                    string directoryPath = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    using Image image = Image.Load(fullPath);
                    if (image.Width > 800 || image.Height > 600)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(800, 600),
                            Mode = ResizeMode.Max
                        }));
                    }
                    image.Save(fullPath, new WebpEncoder { Quality = 80 });
                    lst.Add(new FileModel { Path = "/" + stringPath + "/" + fileName, FileName = fileName });
                }
                return Ok(lst);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPost("delete-image")]
        public ActionResult DeleteImage(string stringPath)
        {
            lock (Commons.LockObjectState)
            {
                try
                {
                    string path = LayDuongDanFile(stringPath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Đã xảy ra lỗi: " + ex.Message);
                }
                return Ok("Xóa hình ảnh thành công!");
            }
        }
        public class MultiFilePut
        {
            public string stringPath { get; set; }
        }
        [HttpPost("RemoveMulti")]
        public ActionResult Multi(List<MultiFilePut> lstFiles)
        {
            lock (Commons.LockObjectState)
            {
                foreach (var data in lstFiles)
                {
                    if (!string.IsNullOrEmpty(data.stringPath))
                    {
                        try
                        {
                            string path = LayDuongDanFile(data.stringPath);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(StatusCodes.Status409Conflict, "Đã xảy ra lỗi: " + ex.Message);
                        }
                    }
                }
                return Ok();
            }
        }
        [HttpPost("apk")]
        public ActionResult UploadAPK(IFormFile file, string stringPath, Guid donVi_Id, string maPhienBan, Guid phanMem_Id)
        {
            lock (Commons.LockObjectState)
            {
                if (string.IsNullOrEmpty(maPhienBan))
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Mã phiên bản bắt buộc nhập!");
                }
                var donVi = uow.DonVis.GetById(donVi_Id);
                if (donVi == null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Đơn vị không tồn tại!");
                }
                var phanMem = uow.phanMems.GetById(phanMem_Id);
                if (phanMem == null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Phần mềm không tồn tại!");
                }
                //Xóa file cũ
                if (stringPath != null)
                {
                    try
                    {
                        string path = LayDuongDanFile(stringPath);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status409Conflict, "Đã xảy ra lỗi: " + ex.Message);
                    }
                }
                string fileName = $"{FormatFileName(phanMem.MaPhanMem)}_{FormatFileName(maPhienBan)}_{DateTime.Now.ToString("yyMMddHHmmss")}.apk";
                stringPath = $"Uploads/APK/{FormatFileName(donVi.MaDonVi)}/{FormatFileName(phanMem.MaPhanMem)}/{FormatFileName(maPhienBan)}";
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), stringPath, fileName);
                string directoryPath = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Ok(new FileModel { Path = "/" + stringPath + "/" + fileName, FileName = file.FileName });
            }
        }
    }
}