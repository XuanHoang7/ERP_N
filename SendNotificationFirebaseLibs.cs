using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace ERP
{
    public class SendNotificationFirebaseLibs
    {
        private readonly FirebaseApp FirebaseAppInstance;
        private readonly string CredentialsFilePath;
        public SendNotificationFirebaseLibs()
        {
            CredentialsFilePath = @"firebase.json";
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
        public void SendNotificationWithCustomData(string registrationToken, string title, string body, string link, string LogoPhanMemUrl)
        {
            try
            {
                var customData = new Dictionary<string, string>
                {
                    { "title", title },
                    { "body", body },
                    { "click_action", link }
                };
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
                    ImageUrl = LogoPhanMemUrl
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
        /*public void SendNotification(string title, string body, string registrationToken)
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
        }*/
    }
}