using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using ERP.Controllers;
using ERP.HubConfig;
using ERP.Infrastructure;
using ERP.Models;
using Newtonsoft.Json;
using ERP.Models.Default;

namespace ERP.Filters
{
    public class UserFilterAttribute : IActionFilter
    {
        private readonly IUnitofWork uow;
        private IHubContext<DashboardHub> hub;
        private readonly DashboardController dashboardController;
        public UserFilterAttribute(IUnitofWork _uow, IHubContext<DashboardHub> _hub, DashboardController _dashboardController)
        {
            uow = _uow;
            hub = _hub;
            dashboardController = _dashboardController;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var method = context.HttpContext.Request.Method.ToString();
            if (method == "GET") return;
            var routeData = context.RouteData;
            var action = routeData.Values["action"];
            if (action.ToString() is "PostInfoTest" or "PutThoiDiemVideo" or "PutChonDapAn")
            {
                return;
            }
            var controller = routeData.Values["controller"];
            if (controller.ToString() == "KeySercure") return;
            string data;
            var url = $"{controller}/{action}";
            int Type_Login_ChangePass = 0;
            if (!string.IsNullOrEmpty(context.HttpContext.Request.QueryString.Value))
            {
                data = context.HttpContext.Request.QueryString.Value;
            }
            else
            {
                var arguments = context.ActionArguments;
                data = JsonConvert.SerializeObject(arguments);
                if (action.ToString() == "Authencation")
                {
                    Type_Login_ChangePass = 1;
                }
                else if (action.ToString() == "ChangePassword")
                {
                    Type_Login_ChangePass = 2;
                }
            }
            var user = context.HttpContext.User.Identity.Name;
            var ipAddress = context.HttpContext.Connection.LocalIpAddress.ToString();
            SaveUserActivity(data, url, controller.ToString(), method, user, ipAddress, Type_Login_ChangePass);
        }
        public void SaveUserActivity(string data, string url, string controller, string type, string user, string ipAddress, int Type_Login_ChangePass)
        {
            if (Type_Login_ChangePass == 1)
            {
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                jsonObject.Password = "*****";
                data = JsonConvert.SerializeObject(jsonObject);
            }
            if (Type_Login_ChangePass == 2)
            {
                dynamic jsonObject = JsonConvert.DeserializeObject(data);
                jsonObject.Password = "*****";
                jsonObject.NewPassword = "*****";
                jsonObject.ConfirmNewPassword = "*****";
                data = JsonConvert.SerializeObject(jsonObject);
            }
            Guid? us = null;
            if (user != null)
            {
                us = Guid.Parse(user);
            }
            var userActivity = new Log
            {
                Data = data,
                Type = type,
                Url = url,
                AccessdBy = us,
                IpAddress = ipAddress,
                AccessDate = DateTime.Now
            };
            uow.Logs.Add(userActivity);
            uow.Complete();
        }
    }
}