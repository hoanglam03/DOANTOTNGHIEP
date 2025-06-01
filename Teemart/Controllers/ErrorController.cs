using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom9.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {
        // GET: Error
        [HttpGet]
        public ActionResult PageNotFound()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }
    }
}