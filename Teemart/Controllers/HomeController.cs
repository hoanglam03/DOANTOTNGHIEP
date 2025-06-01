using Nhom9.Areas.Admin.Data;
using Nhom9.Models;
using Nhom9.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security; // Thêm dòng này
namespace Nhom9.Controllers
{
    public class HomeController : Controller
    {
        Nhom9DB db = new Nhom9DB();

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.SanPhamMoi = db.SanPhams.Select(p => p).OrderByDescending(p => p.NgayTao).Take(5);
            ViewBag.GiaTot = db.SanPhams.Select(p => p).OrderBy(p => p.Gia).Take(5);
            ViewBag.AoThun = db.SanPhams.Where(s => s.MaDM == 1).OrderByDescending(s => s.NgayTao).Take(10).ToList();
            ViewBag.AoKhoac = db.SanPhams.Where(s => s.MaDM == 2).OrderByDescending(s => s.NgayTao).Take(10).ToList();
            ViewBag.QuanDai = db.SanPhams.Where(s => s.MaDM == 3).OrderByDescending(s => s.NgayTao).Take(10).ToList();
            ViewBag.QuanShort = db.SanPhams.Where(s => s.MaDM == 4).OrderByDescending(s => s.NgayTao).Take(10).ToList();
            ViewBag.Vay = db.SanPhams.Where(s => s.MaDM == 5).OrderByDescending(s => s.NgayTao).Take(10).ToList();
            return View();
        }

        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            IEnumerable<DanhMuc> danhmucs = db.DanhMucs.Select(p => p);
            return PartialView(danhmucs);
        }

        [ChildActionOnly]
        public ActionResult DropdownCategories()
        {
            IEnumerable<DanhMuc> danhmucs = db.DanhMucs.Select(p => p);
            return PartialView(danhmucs);
        }

        [ChildActionOnly]
        public ActionResult SelectOptionSize()
        {
            IEnumerable<KichCo> kichCos = db.KichCoes.Select(p => p);
            return PartialView(kichCos);
        }

        [ChildActionOnly]
        public ActionResult CartCount()
        {
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();
            list = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
            return PartialView(list);
        }

        [HttpGet]
        public ActionResult Login()
        {
            TaiKhoanNguoiDung session = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
            if (session != null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginAccount loginAccount)
        {
            if (ModelState.IsValid)
            {
                TaiKhoanNguoiDung tk = db.TaiKhoanNguoiDungs.Where
                    (a => a.TenDangNhap.Equals(loginAccount.username) && a.MatKhau.Equals(loginAccount.password)).FirstOrDefault();

                if (tk != null)
                {
                    if (tk.TrangThai == false)
                    {
                        ModelState.AddModelError("ErrorLogin", "Tài khoản của bạn đã bị vô hiệu hóa !");
                        return View(loginAccount);
                    }
                    else
                    {
                        // Sử dụng FormsAuthentication
                        FormsAuthentication.SetAuthCookie(tk.TenDangNhap, false);
                        Session.Add(ConstaintUser.USER_SESSION, tk);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("ErrorLogin", "Tài khoản hoặc mật khẩu không đúng!");
                    return View(loginAccount);
                }
            }
            return View(loginAccount);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove(ConstaintUser.USER_SESSION);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            TaiKhoanNguoiDung session = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
            if (session != null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(TaiKhoanNguoiDung tk)
        {
            TaiKhoanNguoiDung check = db.TaiKhoanNguoiDungs.Where
                (a => a.TenDangNhap.Equals(tk.TenDangNhap)).FirstOrDefault();

            if (check != null)
            {
                ModelState.AddModelError("ErrorSignUp", "Tên đăng nhập đã tồn tại");
            }
            else
            {
                try
                {
                    tk.TrangThai = true;
                    db.TaiKhoanNguoiDungs.Add(tk);
                    db.SaveChanges();

                    // Không đăng nhập tự động nữa
                    // Session[Nhom9.Session.ConstaintUser.USER_SESSION] = session;

                    // Chuyển hướng sang trang đăng nhập
                    return RedirectToAction("Login", "Home");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ErrorSignUp", "Đăng ký không thành công. Thử lại sau !");
                }
            }

            return View(tk);
        }
    }
}
