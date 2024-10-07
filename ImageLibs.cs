using System.Drawing.Imaging;
using System.Drawing;
using System.Text.Json;
using System;
using System.Linq;
using System.IO;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using static ERP.Commons;
namespace NETCORE3
{
    public class ImageLibs
    {
        public class class_ViTriLoi
        {
            public float x { get; set; }
            public float y { get; set; }
            public string maLoi { get; set; }
            public Guid? tits_qtsx_TDSXKiemSoatChatLuongChiTiet_Id { get; set; }
        }
        public static int[] CreateErrorRound(string inputImagePath, string outputImagePath, string[] viTriLois)
        {
            // Đường dẫn của file ảnh đầu vào
            inputImagePath = inputImagePath.TrimStart('/', '\\');
            outputImagePath = outputImagePath.TrimStart('/', '\\');
            if (!File.Exists(inputImagePath))
            {
                inputImagePath = "Uploads/Image/default.png";
            }
            using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputImagePath);
            using MemoryStream outputStream = new();
            image.Save(outputStream, new PngEncoder());
            byte[] pngBytes = outputStream.ToArray();
            // Tạo một đối tượng Bitmap từ file ảnh gốc
            using (MemoryStream ms = new MemoryStream(pngBytes))
            {
                // Tạo một đối tượng Bitmap mới để làm việc
                using (Bitmap newImage = new Bitmap(ms))
                {
                    int width = newImage.Width;
                    int height = newImage.Height;
                    int diameter = (int)(width / 15.0); //Đường kính chấm tròn
                    // Tạo một đối tượng Graphics từ ảnh mới
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        foreach (var viTriLoi in viTriLois)
                        {
                            var loi = JsonSerializer.Deserialize<class_ViTriLoi>(viTriLoi);
                            int x = (int)(width * loi.x / 100);  // 10% của chiều rộng
                            int y = (int)(height * loi.y / 100); // 20% của chiều cao
                            // Tạo một Brush xanh để vẽ hình tròn
                            using (Brush brush = new SolidBrush(Color.Blue))
                            {
                                g.FillEllipse(brush, x - diameter / 2, y - diameter / 2, diameter, diameter);
                            }
                            Font font = new Font("Tahoma", 20); // Font size 20 (ví dụ)
                            Brush textBrush = new SolidBrush(Color.White);

                            // Kiểm tra và điều chỉnh fontsize nếu nó vượt quá kích thước của hình tròn
                            while (g.MeasureString(loi.maLoi, font).Width > diameter || g.MeasureString(loi.maLoi, font).Height > diameter)
                            {
                                font = new Font(font.FontFamily, font.Size - 1); // Giảm fontsize xuống 1
                            }
                            float textX = x - g.MeasureString(loi.maLoi, font).Width / 2;
                            float textY = y - g.MeasureString(loi.maLoi, font).Height / 2;
                            g.DrawString(loi.maLoi, font, textBrush, textX, textY);
                        }
                    }
                    newImage.Save(outputImagePath, ImageFormat.Png);
                    return new int[] { width, height };
                }
            }
        }
        public static void CreateConDau(Guid User_Id, string maMau, string viTri1Dong1, string viTri1Dong2, string viTri2)
        {
            // Đường dẫn của file ảnh đầu vào
            string inputImagePath = "Uploads/Image/ConDau/DauDo.png";
            viTri2 = viTri2.ToUpper();
            viTri1Dong1 = viTri1Dong1.ToUpper();
            Brush textBrush;
            if (maMau.Trim().ToUpper() == "S63")
            {
                inputImagePath = "Uploads/Image/ConDau/DauXanh.png";
                textBrush = new SolidBrush(Color.FromArgb(5, 45, 150));
            }
            else
            {
                textBrush = new SolidBrush(Color.FromArgb(213, 71, 83));
            }
            string outputImagePath = $"Uploads/Image/ConDau/{User_Id.ToString().ToLower()}.png";
            // Tạo một đối tượng Bitmap từ file ảnh gốc
            using (Bitmap originalImage = new Bitmap(inputImagePath))
            {
                // Tạo một đối tượng Bitmap mới để làm việc
                using (Bitmap newImage = new Bitmap(originalImage))
                {
                    int width = newImage.Width;
                    int height = newImage.Height;
                    Font font = new Font("Arial", 0.047f * height);
                    // Tạo một đối tượng Graphics từ ảnh mới
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        float textX = (int)(width * 0.51) - g.MeasureString(viTri1Dong1, font).Width / 2;
                        float textY = (int)(height * 0.262);
                        g.DrawString(viTri1Dong1, font, textBrush, textX, textY);
                        if (!string.IsNullOrEmpty(viTri1Dong2))
                        {
                            viTri1Dong2 = viTri1Dong2.ToUpper();
                            while (g.MeasureString(viTri1Dong2, font).Width > 0.69 * width || g.MeasureString(viTri1Dong2, font).Height > 0.69 * width)
                            {
                                font = new Font(font.FontFamily, font.Size - 1); // Giảm fontsize xuống 1
                            }
                            textX = (int)(width * 0.51) - g.MeasureString(viTri1Dong2, font).Width / 2;
                            textY = (int)(height * 0.346);
                            g.DrawString(viTri1Dong2, font, textBrush, textX, textY);
                        }
                        font = new Font("Arial", 0.047f * height);
                        while (g.MeasureString(viTri2, font).Width > 0.66 * width || g.MeasureString(viTri2, font).Height > 0.66 * width)
                        {
                            font = new Font(font.FontFamily, font.Size - 1); // Giảm fontsize xuống 1
                        }
                        textX = (int)(width * 0.51) - g.MeasureString(viTri2, font).Width / 2;
                        textY = (int)(height * 0.6323);
                        g.DrawString(viTri2, font, textBrush, textX, textY);
                    }
                    newImage.Save(outputImagePath, ImageFormat.Png);
                }
            }
        }
        public class vptq_lms_LopHocChiTietInfo
        {
            public Guid vptq_lms_LopHocChiTiet_Id { get; set; }
            public string FullName { get; set; }
            public string MaNhanVien { get; set; }
            public string TenChuyenDeDaoTao { get; set; }
            public DateTime ThoiGianHoanThanh { get; set; }
        }
        public static void CreateGiayChungNhan(vptq_lms_LopHocChiTietInfo data)
        {
            string outputImagePath = $"Uploads/Image/GiayChungNhan/{data.vptq_lms_LopHocChiTiet_Id.ToString().ToLower()}.png";
            if (System.IO.File.Exists(outputImagePath))
            {
                return;
            }
            if (!Directory.Exists(Path.GetDirectoryName(outputImagePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputImagePath));
            }
            string inputImagePath = "Uploads/Image/GiayChungNhan/GiayChungNhan.png";
            Brush textBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            string tenChuyenDeDaoTao = data.TenChuyenDeDaoTao.Trim();
            using (Bitmap originalImage = new Bitmap(inputImagePath))
            {
                using (Bitmap newImage = new Bitmap(originalImage))
                {
                    int width = newImage.Width;
                    int height = newImage.Height;
                    Font font = new Font("Cambria", 0.037f * height, FontStyle.Bold);
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        string infoCBNV = data.FullName.ToUpper() + " - MSNV: " + data.MaNhanVien;
                        float textX = (int)(width * 0.5) - g.MeasureString(infoCBNV, font).Width / 2;
                        float textY = (int)(height * 0.368);
                        g.DrawString(infoCBNV, font, textBrush, textX, textY);
                        int len = tenChuyenDeDaoTao.Length;
                        if (len > 65)
                        {
                            //Chia đôi dòng
                            string text1 = "", text2 = "";
                            int len2 = len / 2, vitri1, vitri2;
                            string nuaDau = new(tenChuyenDeDaoTao[..len2].Reverse().ToArray()), nuaCuoi = tenChuyenDeDaoTao[len2..];
                            if (tenChuyenDeDaoTao.Contains(','))
                            {
                                vitri1 = nuaDau.LastIndexOf(',');
                                vitri2 = nuaCuoi.IndexOf(',');
                            }
                            else if (tenChuyenDeDaoTao.Contains('&'))
                            {
                                vitri1 = nuaDau.LastIndexOf('&');
                                vitri2 = nuaCuoi.IndexOf('&');
                            }
                            else
                            {
                                vitri1 = nuaDau.LastIndexOf(' ');
                                vitri2 = nuaCuoi.IndexOf(' ');
                            }
                            if (vitri2 != -1 && (vitri1 == -1 || vitri1 > vitri2))
                            {
                                int vitri = len2 + vitri2 + 1;
                                text1 = tenChuyenDeDaoTao[..vitri].Trim();
                                text2 = tenChuyenDeDaoTao[vitri..].Trim();
                            }
                            else
                            {
                                int vitri = len2 - vitri1;
                                text1 = tenChuyenDeDaoTao[..vitri].Trim();
                                text2 = tenChuyenDeDaoTao[vitri..].Trim();
                            }
                            string text = text1.Length > text2.Length ? text1 : text2;
                            font = new Font("Cambria", 0.029f * height);
                            while (g.MeasureString(text, font).Width > 0.94 * width)
                            {
                                font = new Font("Cambria", font.Size - 1); // Giảm fontsize xuống 1
                            }
                            g.DrawString(
                            text1,
                            font,
                            textBrush,
                            (int)(width * 0.5) - g.MeasureString(text1, font).Width / 2,
                            (int)(height * 0.525)
                            );

                            g.DrawString(
                            text2,
                            font,
                            textBrush,
                            (int)(width * 0.5) - g.MeasureString(text2, font).Width / 2,
                            (int)(height * 0.575)
                            );

                            font = new Font("Cambria", 0.026f * height);
                            string ngay = "Ngày hoàn thành: " + data.ThoiGianHoanThanh.ToString("dd/MM/yyyy");
                            g.DrawString(
                                ngay,
                                font,
                                textBrush,
                                (int)(width * 0.5) - g.MeasureString(ngay, font).Width / 2,
                                (int)(height * 0.625)
                                );
                            font = new Font("Cambria", 0.028f * height, FontStyle.Italic);
                        }
                        else
                        {
                            //Một dòng
                            font = new Font("Cambria", 0.031f * height);
                            while (g.MeasureString(tenChuyenDeDaoTao, font).Width > 0.94 * width)
                            {
                                font = new Font("Cambria", font.Size - 1); // Giảm fontsize xuống 1
                            }
                            g.DrawString(
                            tenChuyenDeDaoTao,
                            font,
                            textBrush,
                            (int)(width * 0.5) - g.MeasureString(tenChuyenDeDaoTao, font).Width / 2,
                            (int)(height * 0.525)
                            );
                            font = new Font("Cambria", 0.028f * height);
                            string ngay = "Ngày hoàn thành: " + data.ThoiGianHoanThanh.ToString("dd/MM/yyyy");
                            g.DrawString(
                                ngay,
                                font,
                                textBrush,
                                (int)(width * 0.5) - g.MeasureString(ngay, font).Width / 2,
                                (int)(height * 0.59)
                                );
                            font = new Font("Cambria", 0.028f * height, FontStyle.Italic);
                        }
                        string thongTin = $"Núi Thành, ngày {data.ThoiGianHoanThanh.Day} tháng {data.ThoiGianHoanThanh.ToString("MM")} năm {data.ThoiGianHoanThanh.Year}";
                        g.DrawString(
                            thongTin,
                            font,
                            textBrush,
                            (int)(width * 0.52),
                            (int)(height * 0.7)
                            );
                    }
                    newImage.Save(outputImagePath, ImageFormat.Png);
                }
            }
        }
        public static string FileToBase64(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        public static string CreateGiayChungNhanBase64(vptq_lms_LopHocChiTietInfo data)
        {
            string inputImagePath = "Uploads/Image/GiayChungNhan/GiayChungNhan.png";
            Brush textBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            string tenChuyenDeDaoTao = data.TenChuyenDeDaoTao.Trim();
            using (Bitmap originalImage = new Bitmap(inputImagePath))
            {
                using (Bitmap newImage = new Bitmap(originalImage))
                {
                    int width = newImage.Width;
                    int height = newImage.Height;
                    Font font = new Font("Cambria", 0.037f * height, FontStyle.Bold);
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        string infoCBNV = data.FullName.ToUpper() + " - MSNV: " + data.MaNhanVien;
                        float textX = (int)(width * 0.5) - g.MeasureString(infoCBNV, font).Width / 2;
                        float textY = (int)(height * 0.368);
                        g.DrawString(infoCBNV, font, textBrush, textX, textY);
                        int len = tenChuyenDeDaoTao.Length;
                        if (len > 65)
                        {
                            //Chia đôi dòng
                            string text1 = "", text2 = "";
                            int len2 = len / 2, vitri1, vitri2;
                            string nuaDau = new(tenChuyenDeDaoTao[..len2].Reverse().ToArray()), nuaCuoi = tenChuyenDeDaoTao[len2..];
                            if (tenChuyenDeDaoTao.Contains(','))
                            {
                                vitri1 = nuaDau.LastIndexOf(',');
                                vitri2 = nuaCuoi.IndexOf(',');
                            }
                            else if (tenChuyenDeDaoTao.Contains('&'))
                            {
                                vitri1 = nuaDau.LastIndexOf('&');
                                vitri2 = nuaCuoi.IndexOf('&');
                            }
                            else
                            {
                                vitri1 = nuaDau.LastIndexOf(' ');
                                vitri2 = nuaCuoi.IndexOf(' ');
                            }
                            if (vitri2 != -1 && (vitri1 == -1 || vitri1 > vitri2))
                            {
                                int vitri = len2 + vitri2 + 1;
                                text1 = tenChuyenDeDaoTao[..vitri].Trim();
                                text2 = tenChuyenDeDaoTao[vitri..].Trim();
                            }
                            else
                            {
                                int vitri = len2 - vitri1;
                                text1 = tenChuyenDeDaoTao[..vitri].Trim();
                                text2 = tenChuyenDeDaoTao[vitri..].Trim();
                            }
                            string text = text1.Length > text2.Length ? text1 : text2;
                            font = new Font("Cambria", 0.029f * height);
                            while (g.MeasureString(text, font).Width > 0.94 * width)
                            {
                                font = new Font("Cambria", font.Size - 1); // Giảm fontsize xuống 1
                            }
                            g.DrawString(
                            text1,
                            font,
                            textBrush,
                            (int)(width * 0.5) - g.MeasureString(text1, font).Width / 2,
                            (int)(height * 0.525)
                            );

                            g.DrawString(
                            text2,
                            font,
                            textBrush,
                            (int)(width * 0.5) - g.MeasureString(text2, font).Width / 2,
                            (int)(height * 0.575)
                            );

                            font = new Font("Cambria", 0.026f * height);
                            string ngay = "Ngày hoàn thành: " + data.ThoiGianHoanThanh.ToString("dd/MM/yyyy");
                            g.DrawString(
                                ngay,
                                font,
                                textBrush,
                                (int)(width * 0.5) - g.MeasureString(ngay, font).Width / 2,
                                (int)(height * 0.625)
                                );
                            font = new Font("Cambria", 0.028f * height, FontStyle.Italic);
                        }
                        else
                        {
                            //Một dòng
                            font = new Font("Cambria", 0.031f * height);
                            while (g.MeasureString(tenChuyenDeDaoTao, font).Width > 0.94 * width)
                            {
                                font = new Font("Cambria", font.Size - 1); // Giảm fontsize xuống 1
                            }
                            g.DrawString(
                            tenChuyenDeDaoTao,
                            font,
                            textBrush,
                            (int)(width * 0.5) - g.MeasureString(tenChuyenDeDaoTao, font).Width / 2,
                            (int)(height * 0.525)
                            );
                            font = new Font("Cambria", 0.028f * height);
                            string ngay = "Ngày hoàn thành: " + data.ThoiGianHoanThanh.ToString("dd/MM/yyyy");
                            g.DrawString(
                                ngay,
                                font,
                                textBrush,
                                (int)(width * 0.5) - g.MeasureString(ngay, font).Width / 2,
                                (int)(height * 0.59)
                                );
                            font = new Font("Cambria", 0.028f * height, FontStyle.Italic);
                        }
                        string thongTin = $"Núi Thành, ngày {data.ThoiGianHoanThanh.Day} tháng {data.ThoiGianHoanThanh.ToString("MM")} năm {data.ThoiGianHoanThanh.Year}";
                        g.DrawString(
                            thongTin,
                            font,
                            textBrush,
                            (int)(width * 0.52),
                            (int)(height * 0.7)
                            );
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        newImage.Save(ms, ImageFormat.Png);
                        byte[] imageBytes = ms.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
        }
    }
}