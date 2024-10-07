using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using QRCoder;
using static QRCoder.QRCodeGenerator;
using System.Drawing;
using FirebaseAdmin.Auth.Multitenancy;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace ERP
{
    public class Commons
    {
        private const string keyCrypt = "+^3#BT4C=NO1Z6N";
        private const string ivCrypt = "W&!BA$-K!-DO49#N";
        public static string UploadBase64(string webRootPath, string File_Base64, string File_Name)
        {
            //Xử lý file base 64 lưu trữ
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            DateTime dt = DateTime.Now;
            string fileName = (long)timeSpan.TotalSeconds + "_" + TiengVietKhongDau(File_Name);
            byte[] fileBytes = Convert.FromBase64String(File_Base64.Split(',')[1]);
            var buffer = Convert.FromBase64String(Convert.ToBase64String(fileBytes));
            // Convert byte[] to file type
            string path = "Uploads/" + dt.Year + "/" + dt.Month + "/" + dt.Day;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }
            string fullPath = Path.Combine(webRootPath, fileName);
            FileStream f = File.Create(fullPath);
            f.Close();
            File.WriteAllBytes(fullPath, buffer);
            return path + "/" + fileName;
        }
        public static string Upload(string webRootPath, IFormFile file)
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            DateTime dt = DateTime.Now;
            // Rename file
            string fileName = (long)timeSpan.TotalSeconds + "_" + Commons.TiengVietKhongDau(file.FileName);
            string fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
            string path = "Uploads/" + dt.Year + "/" + dt.Month + "/" + dt.Day;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }
            string fullPath = Path.Combine(webRootPath, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return path + "/" + fileName;
        }

        public static bool DeleteFile(string filePath)
        {
            try
            {
                // Lấy đường dẫn thư mục hiện tại của dự án
                string projectDirectory = Directory.GetCurrentDirectory().Replace("\\", "/");

                // Kết hợp đường dẫn tệp từ cơ sở dữ liệu với thư mục hiện tại của dự án
                string fullPath = projectDirectory + filePath;
                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xóa tệp: " + ex.Message);
                return false;
            }
        }

        public static string GetFileImg(string filePath)
        {
            try
            {
                // Lấy đường dẫn thư mục hiện tại của dự án
                string projectDirectory = Directory.GetCurrentDirectory().Replace("\\", "/");

                // Kết hợp đường dẫn tệp từ cơ sở dữ liệu với thư mục hiện tại của dự án
                string fullPath = projectDirectory + filePath;
                return fullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xóa tệp: " + ex.Message);
                return null;
            }
        }

        public static string NonUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
            }
            return text;
        }
        public static object LockObjectState = new object();
        public static string TiengVietKhongDau(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace("đ", "d").Replace("Đ", "D").Replace(' ', '_').ToLower();
        }
        public static float ConvertFloat(string number)
        {
            float num = 0;
            float.TryParse(number, out num);
            return num;
        }
        public static float TinhTrungBinh(params float[] array)
        {
            return array.Average();
        }
        public static string ConvertObjectToJson(object ob)
        {
            return JsonConvert.SerializeObject(ob);
        }
        /*    public static Image HinhAnhUrl(string url)
            {
              var base_url = "http://demo1api.thacoindustries.vn/" + url;
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(base_url);
              MemoryStream ms = new MemoryStream(bytes);
              System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
              return img;
            }*/


        //firebase
        public class FirebaseNotificationSender
        {
            private readonly FirebaseApp FirebaseAppInstance;
            private readonly string CredentialsFilePath;

            /*            public FirebaseNotificationSender()
                        {
                            FirebaseAppInstance = InitializeFirebaseAppInstance();
                        }

                        private FirebaseApp InitializeFirebaseAppInstance()
                        {
                            var credential = GoogleCredential.FromFile("D:/Thaco_Dev_QLTB/firebase-admin/qlcntt-bf148-firebase-adminsdk-37kuv-c9f2e3d983.json");
                            return FirebaseApp.Create(new AppOptions
                            {
                                Credential = credential,
                                ProjectId = "qlcntt-bf148"
                            });
                        }*/
            public FirebaseNotificationSender()
            {
                CredentialsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-admin", "qlcntt-bf148-firebase-adminsdk-37kuv-c9f2e3d983.json");
                FirebaseAppInstance = InitializeFirebaseAppInstance();
            }

            private FirebaseApp InitializeFirebaseAppInstance()
            {
                var credential = GoogleCredential.FromFile(CredentialsFilePath);
                return FirebaseApp.Create(new AppOptions
                {
                    Credential = credential,
                    ProjectId = "qlcntt-bf148"
                });
            }

            public void SendNotification(string title, string body, string registrationToken)
            {


                try
                {
                    var messaging = FirebaseMessaging.GetMessaging(FirebaseAppInstance);

                    var message = new Message
                    {
                        Token = registrationToken,
                        Notification = new Notification
                        {
                            Title = title,
                            Body = body
                        }
                    };
                    messaging.SendAsync(message).GetAwaiter().GetResult();
                }
                catch
                {
                    // Handle any exceptions here, log, or throw.
                }
            }
            public void SendNotificationWithCustomData(string registrationToken, Dictionary<string, string> customData, string title, string body, string link)
            {
                try
                {
                    var messaging = FirebaseMessaging.GetMessaging(FirebaseAppInstance);

                    var message = new Message
                    {
                        Token = registrationToken,
                        Data = customData
                    };

                    var notification = new Notification
                    {
                        Title = title,
                        Body = body,
                        //ImageUrl = "https://qlcntt.thacoindustries.com/static/media/logo-industries.04373ce1.jpg"
                    };

                    var webpushOptions = new WebpushFcmOptions
                    {
                        Link = link
                    };

                    message.Notification = notification;
                    message.Webpush = new WebpushConfig
                    {
                        FcmOptions = new WebpushFcmOptions
                        {
                            Link = link
                        }
                    };
                    messaging.SendAsync(message).GetAwaiter().GetResult();
                }
                catch
                {
                    // Handle any exceptions here, log, or throw.
                }
            }

            public void HandleNotificationClick(string link)
            {
                try
                {
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = link,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // Xử lý lỗi nếu cần
                }
            }

        }

        private static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[128];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string HashPassword(string password)
        {
            byte[] salt = GenerateRandomSalt();

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 8;
                argon2.MemorySize = 65536;
                argon2.Iterations = 4;

                byte[] hashBytes = argon2.GetBytes(128);

                string hashedPassword = Convert.ToBase64String(hashBytes);
                string saltString = Convert.ToBase64String(salt);

                return $"{saltString}${hashedPassword}";
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string[] parts = storedHash.Split('$');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hashBytes = Convert.FromBase64String(parts[1]);

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(enteredPassword)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 8;
                argon2.MemorySize = 65536;
                argon2.Iterations = 4;

                byte[] newHashBytes = argon2.GetBytes(128);

                return hashBytes.SequenceEqual(newHashBytes);
            }
        }

        private static byte[] GenerateKey(string password, int keySize)
        {
            using (var sha256 = new SHA256Managed())
            {
                var keyBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(keyBytes);

                // Ensure the key size is correct by taking the first 'keySize' bits
                var key = new byte[keySize / 8];
                Array.Copy(hashBytes, key, key.Length);

                return key;
            }
        }
        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(keyCrypt, aesAlg.KeySize);
                aesAlg.IV = GenerateKey(ivCrypt, aesAlg.BlockSize);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(keyCrypt, aesAlg.KeySize);
                aesAlg.IV = GenerateKey(ivCrypt, aesAlg.BlockSize);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }



        //kiểm tra có chữ hoa chữ, có số, có ký tự đặt biệt
        public class CustomPasswordValidationAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                string password = value as string;

                if (string.IsNullOrEmpty(password))
                    return false;

                bool hasUpperCase = password.Any(char.IsUpper);
                bool hasSpecialCharacter = password.Any(c => !char.IsLetterOrDigit(c));
                bool hasDigit = password.Any(char.IsDigit);

                return hasUpperCase && hasSpecialCharacter && hasDigit;
            }
        }
        public static void Shuffle<T>(List<T> list)
        {
            Random random = new();
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
        public static IEnumerable<T> RandomDeThi<T>(IEnumerable<T> list, int soCauHoi)
        {
            Random random = new();
            return list.OrderBy(x => random.Next()).Take(soCauHoi);
        }
        public static string LayDuongDanFile(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            return url.TrimStart('/', '\\');
        }
        public static string FormatFileName(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";
            str = ToTitleCase(NonUnicode(str.ToLower()));
            str = new string(str.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
            return str.Length > 50 ? str[..50] : str;
        }
        public static string ToTitleCase(string text)
        {
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text);
        }
        public static string VietHoaChuCaiDau(string text)
        {
            text = text?.Trim();
            if (string.IsNullOrWhiteSpace(text)) return null;
            return string.Concat(text[..1].ToUpper(), text.AsSpan(1));
        }
        public static string FormatNameText(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            name = name.Trim();
            string punctuationPattern = @"(?<=\p{L})\s+(?=[.,;!?])";
            string textWithoutSpacesBeforePunctuation = Regex.Replace(name, punctuationPattern, "");
            return textWithoutSpacesBeforePunctuation;
        }
        public static string GetFileName(string name)
        {
            string fileName = Path.GetFileNameWithoutExtension(name);
            return fileName;
        }
        public static string GetFileExtension(string name)
        {
            string fileExtension = Path.GetExtension(name);
            return fileExtension;
        }
        public enum LoaiFormat
        {
            Default = 0,
            ToLower = 1,
            ToUpper = 2,
        }
        private static string GetLoaiForMat(int loaiPhieuYeuCau)
        {
            return loaiPhieuYeuCau switch
            {
                (int)LoaiFormat.Default => "Mặc định",
                (int)LoaiFormat.ToLower => "Tất cả chữ cái thường",
                (int)LoaiFormat.ToUpper => "Tất cả chữ cái in hoa",
                _ => "",
            };
        }
        public static string CleanAndFormatString(string input, LoaiFormat loaiformat)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;
            string cleanedInput = input.Trim().Replace("\n", "").Replace("\r", "").Replace("\t", "");
            return loaiformat switch
            {
                LoaiFormat.ToUpper => cleanedInput.ToUpper(),
                LoaiFormat.ToLower => cleanedInput.ToLower(),
                _ => cleanedInput,
            };
        }
        public enum LoaiNgayGioDinhDang
        {
            Ngay, // "dd/MM/yyyy"
            NgayGioPhutGiay, // "dd/MM/yyyy HH:mm:ss"
            NgayGioPhut // "dd/MM/yyyy HH:mm"
        }
        public static bool TryParseDate(string dateString, LoaiNgayGioDinhDang format, out DateTime date)
        {
            string loaingay = format switch
            {
                LoaiNgayGioDinhDang.Ngay => "dd/MM/yyyy",
                LoaiNgayGioDinhDang.NgayGioPhutGiay => "dd/MM/yyyy HH:mm:ss",
                LoaiNgayGioDinhDang.NgayGioPhut => "dd/MM/yyyy HH:mm",
                _ => "dd/MM/yyyy"
            };
            return DateTime.TryParseExact(dateString, loaingay, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
        public static string CopyFile(string FileUrl)
        {
            string FileKetQua = null;
            if (!string.IsNullOrWhiteSpace(FileUrl))
            {
                string newId = Guid.NewGuid().ToString("N");
                string fileExt = System.IO.Path.GetExtension(FileUrl);
                int lastIndex = FileUrl.LastIndexOf('_');
                if (lastIndex != -1)
                {
                    FileKetQua = string.Concat(FileUrl[..lastIndex], "_", newId, fileExt);
                }
                else
                {
                    lastIndex = FileUrl.LastIndexOf('.');
                    FileKetQua = string.Concat(FileUrl[..lastIndex], "_", newId, fileExt);
                }
                System.IO.File.Copy(LayDuongDanFile(FileUrl), LayDuongDanFile(FileKetQua), true);
            }
            return FileKetQua;
        }
        public static void GenerateQRCodeImage(string text, int doPhanGia = 20, string path = "Uploads/Image/QRCode/QRCode.png")
        {
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, ECCLevel.Q);
            BitmapByteQRCode qrCode = new(qrCodeData);
            using (MemoryStream ms = new MemoryStream(qrCode.GetGraphic(doPhanGia)))
            {
                Bitmap bitmap = new Bitmap(ms);
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
            /*string base64String = Convert.ToBase64String(qrCode.GetGraphic(20));
            return base64String;*/
        }
        public static string GenerateQRCodeBase64(string text, int doPhanGia = 20)
        {
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, ECCLevel.Q);
            BitmapByteQRCode qrCode = new(qrCodeData);
            string base64String = Convert.ToBase64String(qrCode.GetGraphic(20));
            return base64String;
        }
        public static string ToSoLaMa(int number)
        {
            // Mảng chứa các ký tự số La Mã và giá trị tương ứng
            string[] romanNumerals = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            int[] values = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            // Sử dụng StringBuilder để xây dựng chuỗi số La Mã
            var result = new System.Text.StringBuilder();
            for (int i = 0; i < romanNumerals.Length; i++)
            {
                int count = number / values[i];
                number %= values[i];
                // Thêm ký tự số La Mã vào chuỗi kết quả
                result.Append(string.Concat(Enumerable.Repeat(romanNumerals[i], count)));
            }
            return result.ToString();
        }
    }
}
