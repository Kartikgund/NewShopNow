using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NewShopNow2
{
    public class MyActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionUser = filterContext.HttpContext.Session["User"];
            var routeValues = filterContext.RequestContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RequestContext.RouteData.Values["action"].ToString();
            if (sessionUser == null && !actionName.Equals("AuthenticateUser") && !actionName.Equals("Login") &&
                !actionName.Equals("RegisterUser") && !actionName.Equals("SaveUser") && !actionName.Equals("ForgetPassword")
                 && !actionName.Equals("VerifyEmail") && !actionName.Equals("VerifyOTP")
                 && !actionName.Equals("ResetPassword") && !actionName.Equals("SavePassword"))
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}