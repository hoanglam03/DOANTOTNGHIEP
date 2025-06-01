using System.Linq;
using System.Web.Mvc;
using Nhom9.Models;

namespace Nhom9.Controllers
{
    public class TinTucController : Controller
    {
        private Nhom9DB db = new Nhom9DB();

        public ActionResult Index()
        {
            var dsTinTuc = db.TinTucs.Where(t => t.IsActive).OrderByDescending(t => t.NgayDang).ToList();
            return View(dsTinTuc);
        }

        public ActionResult Details(int id)
        {
            var tin = db.TinTucs.Find(id);
            if (tin == null || !tin.IsActive)
                return HttpNotFound();

            return View(tin);
        }
    }
}
