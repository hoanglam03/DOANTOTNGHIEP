using Nhom9.Models;
using Nhom9.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nhom9.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = Session[ConstaintUser.ADMIN_SESSION];
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    Controller = "Login",
                    Action = "Index",
                    Area = "Admin"
                }));
            }
            base.OnActionExecuting(filterContext);
        }

        // Helper method to check if user is admin
        protected bool IsAdmin()
        {
            var session = (TaiKhoanQuanTri)Session[ConstaintUser.ADMIN_SESSION];
            return session != null && session.LoaiTaiKhoan == true;
        }

        // Helper method to check if user is staff
        protected bool IsStaff()
        {
            var session = (TaiKhoanQuanTri)Session[ConstaintUser.ADMIN_SESSION];
            return session != null && session.LoaiTaiKhoan == false;
        }
    }
}