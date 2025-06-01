using System;
using System.Web;
using System.Web.Mvc;
using Nhom9.Models;
using Nhom9.Session;

namespace Nhom9.Areas.Admin.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var session = (TaiKhoanQuanTri)HttpContext.Current.Session[ConstaintUser.ADMIN_SESSION];

            if (session == null)
            {
                filterContext.Result = new RedirectResult("~/Admin/Login");
                return;
            }

            // Check if user is admin (LoaiTaiKhoan == true)
            if (session.LoaiTaiKhoan != true)
            {
                filterContext.Result = new RedirectResult("~/Admin/Home/AccessDenied");
            }
        }
    }
}
