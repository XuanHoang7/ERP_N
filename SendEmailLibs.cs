using System.Collections.Generic;
using System;
using Microsoft.Exchange.WebServices.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ERP
{
    public class SendEmailLibs
    {
        private const string key = "Thaco@Industries";
        private const string iv = "Thaco@2024AbCdEf";
        private readonly IConfiguration configuration;
        public SendEmailLibs(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public static string EncryptPassword(string plainText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using StreamWriter swEncrypt = new(csEncrypt);
                swEncrypt.Write(plainText);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        public static string DecryptPassword(string cipherText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        public void SendEmail(string fromEmail, string password, string subject, string content, IEnumerable<string> toEmails, bool isPasswordHash = false)
        {
            try
            {
                string linkHome = "https://erp.thacoindustries.com";
                if (configuration != null)
                {
                    linkHome = configuration["LinkHome:Link"] ?? linkHome;
                }
                if (isPasswordHash)
                {
                    password = DecryptPassword(DecryptPassword(password));
                }
                ExchangeService service = new(ExchangeVersion.Exchange2013)
                {
                    Credentials = new WebCredentials(fromEmail, password),
                    Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx")
                };
                EmailMessage mail = new(service)
                {
                    Subject = subject
                };
                string htmlBody = @"<!DOCTYPE html>
                  <html lang=""en"">
                     <head>
                        <meta charset=""utf-8"" /> <meta http-equiv=""x-ua-compatible"" content=""ie=edge"" /> <title>Email Confirmation</title> <meta name=""viewport"" content=""width=device-width, initial-scale=1"" /> <style type=""text/css""></style>
                     </head>
                     <body>
                        <table width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#f5f6f7"">
                            <tbody>
                                <tr><td height=""50""></td></tr>
                                <tr> <td align=""center"" valign=""top"">
                                    <table width=""720"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#ffffff"" style=""border:1px solid #f1f2f5"">
                                        <tbody>
                                            <tr>
                                                <td align=""center"" style=""padding-top: 15px"">
                                                    <img width=""151"" height=""111"" src=""cid:MyImageId"" alt=""E-Learning"" title=""E-Learning"" style=""display: flex; align-content: center; width: 150px; height: auto; border: 0; line-height: 100%; outline: none; text-decoration: none;"" data-bit=""iit"" tabindex=""0"" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center; padding:0px 40px;""> <h1 style=""text-align:center;font-family:Cambria,serif;font-weight:bold;font-size:22px;color:#3366CC""> "
                                                    + subject.ToUpper()
                                                + @" </h1> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align: justify; padding:0px 30px;""> <p style=""text-align:justify;font-family:Cambria,serif;font-size:16px;color:#006DB0""> "
                                                    + content
                                                + @" </p> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2""style=""text-align:center; padding:0px 260px;"">
                                                    <a href=""" + linkHome + @""" target=""_blank"" style=""text-decoration:none""> 
                                                        <div width=""200"" height=""40"" style=""width:200px;font-size:15px;line-height:40px;border-radius:5px;background-color:#13aa52;border:1px solid #13aa52;text-align:center;margin:auto;color:#ffffff;"">
                                                            Đến trang Đào tạo
                                                        </div>
                                                    </a>
                                                </td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#999"">THACO INDUSTRIES © 2024</span> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:left; padding:10px 40px 0px 40px; ""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#555"">Đây là email được tạo tự động. Vui lòng không trả lời thư này.</span></td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                        </tbody>
                                    </table> </td>
                                </tr>
                                <tr> <td height=""50""> </td> </tr>
                            </tbody>
                         </table>
                     </body>
                  </html>";
                string imagePath = "Uploads/Image/LogoDaoTao.png";
                FileAttachment fileAttachment = mail.Attachments.AddFileAttachment("LogoDaoTao.png", imagePath);
                fileAttachment.ContentId = "MyImageId";
                mail.Body = new MessageBody(BodyType.HTML, htmlBody);
                mail.ToRecipients.AddRange(toEmails);
                mail.Send();
                //System.Threading.Thread.Sleep(600);
            }
            catch /*(Exception e)*/
            {
                //Gửi lỗi về email
            }
        }
        public void SendEmailCongNgheThongTin(string fromEmail, string password, string subject, string content, IEnumerable<string> toEmails, bool isPasswordHash = false)
        {
            try
            {
                string linkHome = "http://erp.thacoindustries.com";
                if (configuration != null)
                {
                    linkHome = configuration["LinkHome:Link"] ?? linkHome;
                }
                if (isPasswordHash)
                {
                    password = DecryptPassword(DecryptPassword(password));
                }
                ExchangeService service = new(ExchangeVersion.Exchange2013)
                {
                    Credentials = new WebCredentials(fromEmail, password),
                    Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx")
                };
                EmailMessage mail = new(service)
                {
                    Subject = subject
                };
                string htmlBody = @"<!DOCTYPE html>
                  <html lang=""en"">
                     <head>
                        <meta charset=""utf-8"" /> <meta http-equiv=""x-ua-compatible"" content=""ie=edge"" /> <title>Email Confirmation</title> <meta name=""viewport"" content=""width=device-width, initial-scale=1"" /> <style type=""text/css""></style>
                     </head>
                     <body>
                        <table width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#f5f6f7"">
                            <tbody>
                                <tr><td height=""50""></td></tr>
                                <tr> <td align=""center"" valign=""top"">
                                    <table width=""720"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#ffffff"" style=""border:1px solid #f1f2f5"">
                                        <tbody>
                                            <tr>
                                                <td height=""60"" style=""border-bottom:1px solid #eeeeee;padding-left:16px"" align=""left"">
                                                    <img src=""cid:MyImageId"" width=""117"" height=""28"" alt=""Thaco Industries"" title=""Thaco Industries"" style=""display: flex; align-content: center; width: 117px; height: auto; border: 0; line-height: 100%; outline: none; text-decoration: none;"" data-bit=""iit"" tabindex=""0"" />
                                                </td>
                                                <td height=""60"" style=""border-bottom:1px solid #eeeeee;padding-left:16px"" align=""left"">
                                                    <div style=""display: flex; align-content: center; ""> <font face=""Cambria,serif"" size=""2"" color=""#006DB0"" style=""font-family: Cambria, serif, serif, EmojiFont;""> <span style=""font-size:10pt;""> <b>TẬP ĐOÀN CÔNG NGHIỆP TRƯỜNG HẢI</b> </span> </font> </div>
                                                    <div style=""display: flex; align-content: center; ""> <font face=""Cambria,serif"" size=""2"" color=""#006DB0"" style=""font-family: Cambria, serif, serif, EmojiFont;""> <span style=""font-size:10pt;"">KCN THACO Chu Lai, Huyện Núi Thành, Tỉnh Quảng Nam, Việt Nam</span> </font> </div>
                                                </td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""10""></td></tr>
                                            <tr> <td width=""20""></td> <td align=""left""></td> </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center; padding:0px 40px;""> <h1 style=""text-align:center;font-family:Cambria,serif;font-weight:bold;font-size:22px;color:#3366CC""> "
                                                    + subject.ToUpper()
                                                + @" </h1> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align: justify; padding:0px 30px;""> <p style=""text-align:justify;font-family:Cambria,serif;font-size:16px;color:#006DB0""> "
                                                    + content
                                                + @" </p> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2""style=""text-align:center; padding:0px 260px;"">
                                                    <a href=""" + linkHome + @""" target=""_blank"" style=""text-decoration:none""> 
                                                        <div width=""200"" height=""40"" style=""width:200px;font-size:15px;line-height:40px;border-radius:5px;background-color:#13aa52;border:1px solid #13aa52;text-align:center;margin:auto;color:#ffffff;"">
                                                            Đến trang ERP
                                                        </div>
                                                    </a> 
                                                </td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#999"">THACO INDUSTRIES © 2024</span> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:left; padding:10px 40px 0px 40px; ""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#555"">Đây là email được tạo tự động. Vui lòng không trả lời thư này.</span></td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                        </tbody>
                                    </table> </td>
                                </tr>
                                <tr> <td height=""50""> </td> </tr>
                            </tbody>
                         </table>
                     </body>
                  </html>";
                string imagePath = "Uploads/Image/logo.png";
                FileAttachment fileAttachment = mail.Attachments.AddFileAttachment("Logo.png", imagePath);
                fileAttachment.ContentId = "MyImageId";
                mail.Body = new MessageBody(BodyType.HTML, htmlBody);
                mail.ToRecipients.AddRange(toEmails);
                mail.Send();
                //System.Threading.Thread.Sleep(600);
            }
            catch /*(Exception e)*/
            {
                //Gửi lỗi về email
            }
        }
        public void SendEmailThietBiChuyenDung(string fromEmail, string password, string subject, string content, IEnumerable<string> toEmails, bool isPasswordHash = false)
        {
            try
            {
                string linkHome = "http://erp.thacoindustries.com";
                if (configuration != null)
                {
                    linkHome = configuration["LinkHome:Link"] ?? linkHome;
                }
                if (isPasswordHash)
                {
                    password = DecryptPassword(DecryptPassword(password));
                }
                ExchangeService service = new(ExchangeVersion.Exchange2013)
                {
                    Credentials = new WebCredentials(fromEmail, password),
                    Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx")
                };
                EmailMessage mail = new(service)
                {
                    Subject = subject
                };
                string htmlBody = @"<!DOCTYPE html>
                  <html lang=""en"">
                     <head>
                        <meta charset=""utf-8"" /> <meta http-equiv=""x-ua-compatible"" content=""ie=edge"" /> <title>Email Confirmation</title> <meta name=""viewport"" content=""width=device-width, initial-scale=1"" /> <style type=""text/css""></style>
                     </head>
                     <body>
                        <table width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#f5f6f7"">
                            <tbody>
                                <tr><td height=""50""></td></tr>
                                <tr> <td align=""center"" valign=""top"">
                                    <table width=""720"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#ffffff"" style=""border:1px solid #f1f2f5"">
                                        <tbody>
                                            <tr>
                                                <td height=""60"" style=""border-bottom:1px solid #eeeeee;padding-left:16px"" align=""left"">
                                                    <img src=""cid:MyImageId"" width=""117"" height=""28"" alt=""Thaco Industries"" title=""Thaco Industries"" style=""display: flex; align-content: center; width: 117px; height: auto; border: 0; line-height: 100%; outline: none; text-decoration: none;"" data-bit=""iit"" tabindex=""0"" />
                                                </td>
                                                <td height=""60"" style=""border-bottom:1px solid #eeeeee;padding-left:16px"" align=""left"">
                                                    <div style=""display: flex; align-content: center; ""> <font face=""Cambria,serif"" size=""2"" color=""#006DB0"" style=""font-family: Cambria, serif, serif, EmojiFont;""> <span style=""font-size:10pt;""> <b>TỔNG CÔNG TY THIẾT BỊ CHUYÊN DỤNG</b> </span> </font> </div>
                                                    <div style=""display: flex; align-content: center; ""> <font face=""Cambria,serif"" size=""2"" color=""#006DB0"" style=""font-family: Cambria, serif, serif, EmojiFont;""> <span style=""font-size:10pt;"">KCN THACO Chu Lai, Huyện Núi Thành, Tỉnh Quảng Nam, Việt Nam</span> </font> </div>
                                                </td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""10""></td></tr>
                                            <tr> <td width=""20""></td> <td align=""left""></td> </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center; padding:0px 40px;""> <h1 style=""text-align:center;font-family:Cambria,serif;font-weight:bold;font-size:22px;color:#3366CC""> "
                                                    + subject.ToUpper()
                                                + @" </h1> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align: justify; padding:0px 30px;""> <p style=""text-align:justify;font-family:Cambria,serif;font-size:16px;color:#006DB0""> "
                                                    + content
                                                + @" </p> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2""style=""text-align:center; padding:0px 260px;"">
                                                    <a href=""" + linkHome + @""" target=""_blank"" style=""text-decoration:none""> 
                                                        <div width=""200"" height=""40"" style=""width:200px;font-size:15px;line-height:40px;border-radius:5px;background-color:#13aa52;border:1px solid #13aa52;text-align:center;margin:auto;color:#ffffff;"">
                                                            Đến trang ERP
                                                        </div>
                                                    </a> 
                                                </td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#999"">THACO INDUSTRIES © 2024</span> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:left; padding:10px 40px 0px 40px; ""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#555"">Đây là email được tạo tự động. Vui lòng không trả lời thư này.</span></td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                        </tbody>
                                    </table> </td>
                                </tr>
                                <tr> <td height=""50""> </td> </tr>
                            </tbody>
                         </table>
                     </body>
                  </html>";
                string imagePath = "Uploads/Image/logo.png";
                FileAttachment fileAttachment = mail.Attachments.AddFileAttachment("Logo.png", imagePath);
                fileAttachment.ContentId = "MyImageId";
                mail.Body = new MessageBody(BodyType.HTML, htmlBody);
                mail.ToRecipients.AddRange(toEmails);
                mail.Send();
                //System.Threading.Thread.Sleep(600);
            }
            catch /*(Exception e)*/
            {
                //Gửi lỗi về email
            }
        }
        public void SendEmailDonVi(string tenDonVi, string fromEmail, string password, string subject, string content, IEnumerable<string> toEmails, bool isPasswordHash = false, string link = null)
        {
            try
            {
                string linkHome = "https://erp.thacoindustries.com";
                if (configuration != null)
                {
                    link = string.Concat((configuration["LinkHome:Link"] ?? linkHome), link);
                }
                link = link.Replace("//", "/").Replace("https:/", "https://").Replace("http:/", "http://");
                if (isPasswordHash)
                {
                    password = DecryptPassword(DecryptPassword(password));
                }
                ExchangeService service = new(ExchangeVersion.Exchange2013)
                {
                    Credentials = new WebCredentials(fromEmail, password),
                    Url = new Uri("https://mail.thaco.com.vn/ews/exchange.asmx")
                };
                EmailMessage mail = new(service)
                {
                    Subject = subject
                };
                string htmlBody = @"<!DOCTYPE html>
                  <html lang=""en"">
                     <head>
                        <meta charset=""utf-8"" /> <meta http-equiv=""x-ua-compatible"" content=""ie=edge"" /> <title>Email Confirmation</title> <meta name=""viewport"" content=""width=device-width, initial-scale=1"" /> <style type=""text/css""></style>
                     </head>
                     <body>
                        <table width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#f5f6f7"">
                            <tbody>
                                <tr><td height=""50""></td></tr>
                                <tr> <td align=""center"" valign=""top"">
                                    <table width=""720"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#ffffff"" style=""border:1px solid #f1f2f5"">
                                        <tbody>
                                            <tr>
                                                <td height=""60"" style=""border-bottom:1px solid #eeeeee;padding-left:16px"" align=""left"">
                                                    <img src=""cid:MyImageId"" width=""117"" height=""28"" alt=""Thaco Industries"" title=""Thaco Industries"" style=""display: flex; align-content: center; width: 117px; height: auto; border: 0; line-height: 100%; outline: none; text-decoration: none;"" data-bit=""iit"" tabindex=""0"" />
                                                </td>
                                                <td height=""60"" style=""border-bottom:1px solid #eeeeee;padding-left:16px"" align=""left"">
                                                    <div style=""display: flex; align-content: center; ""> <font face=""Cambria,serif"" size=""2"" color=""#006DB0"" style=""font-family: Cambria, serif, serif, EmojiFont;""> <span style=""font-size:10pt;""> <b>" + tenDonVi.ToUpper() + @"</b> </span> </font> </div>
                                                    <div style=""display: flex; align-content: center; ""> <font face=""Cambria,serif"" size=""2"" color=""#006DB0"" style=""font-family: Cambria, serif, serif, EmojiFont;""> <span style=""font-size:10pt;"">KCN THACO Chu Lai, Huyện Núi Thành, Tỉnh Quảng Nam, Việt Nam</span> </font> </div>
                                                </td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""10""></td></tr>
                                            <tr> <td width=""20""></td> <td align=""left""></td> </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center; padding:0px 40px;""> <h1 style=""text-align:center;font-family:Cambria,serif;font-weight:bold;font-size:22px;color:#3366CC""> "
                                                    + subject.ToUpper()
                                                + @" </h1> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align: justify; padding:0px 30px;""> <p style=""text-align:justify;font-family:Cambria,serif;font-size:16px;color:#006DB0""> "
                                                    + content
                                                + @" </p> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2""style=""text-align:center; padding:0px 260px;"">
                                                    <a href=""" + link + @""" target=""_blank"" style=""text-decoration:none""> 
                                                        <div width=""200"" height=""40"" style=""width:200px;font-size:15px;line-height:40px;border-radius:5px;background-color:#13aa52;border:1px solid #13aa52;text-align:center;margin:auto;color:#ffffff;"">
                                                            Đến trang ERP
                                                        </div>
                                                    </a> 
                                                </td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:center""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#999"">THACO INDUSTRIES © 2024</span> </td>
                                            </tr>
                                            <tr>
                                                <td colspan=""2"" style=""text-align:left; padding:10px 40px 0px 40px; ""> <span style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;color:#555"">Đây là email được tạo tự động. Vui lòng không trả lời thư này.</span></td>
                                            </tr>
                                            <tr><td colspan=""2"" height=""20""></td></tr>
                                        </tbody>
                                    </table> </td>
                                </tr>
                                <tr> <td height=""50""> </td> </tr>
                            </tbody>
                         </table>
                     </body>
                  </html>";
                string imagePath = "Uploads/Image/logo.png";
                FileAttachment fileAttachment = mail.Attachments.AddFileAttachment("Logo.png", imagePath);
                fileAttachment.ContentId = "MyImageId";
                mail.Body = new MessageBody(BodyType.HTML, htmlBody);
                mail.ToRecipients.AddRange(toEmails);
                mail.Send();
                //System.Threading.Thread.Sleep(600);
            }
            catch /*(Exception e)*/
            {
                //Gửi lỗi về email
            }
        }
    }
}