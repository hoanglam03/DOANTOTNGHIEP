using Nhom9.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Nhom9.Controllers
{
    public class ProductController : Controller
    {
        Nhom9DB db = new Nhom9DB();

        public ActionResult Shop(string searchString, int? madm, int page = 1, int pageSize = 9, string[] priceRange = null, string sortOrder = null)
        {
            ViewBag.searchString = searchString;
            ViewBag.madm = madm;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SelectedPriceRange = priceRange;

            var sanphams = db.SanPhams.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                sanphams = sanphams.Where(sp => sp.TenSP.Contains(searchString));
            }

            if (madm != null && madm != 0)
            {
                sanphams = sanphams.Where(s => s.MaDM == madm);
                ViewBag.DanhMuc = db.DanhMucs.Where(d => d.MaDM == madm).FirstOrDefault();
            }

            if (priceRange != null && priceRange.Length > 0)
            {
                var filteredSanPhams = new List<Nhom9.Models.SanPham>();

                foreach (var range in priceRange)
                {
                    if (range.Contains("+"))
                    {
                        if (decimal.TryParse(range.Replace("+", ""), out decimal min))
                        {
                            filteredSanPhams.AddRange(sanphams.Where(sp => sp.Gia >= min));
                        }
                    }
                    else
                    {
                        var parts = range.Split('-');
                        if (parts.Length == 2 && decimal.TryParse(parts[0], out decimal min) && decimal.TryParse(parts[1], out decimal max))
                        {
                            filteredSanPhams.AddRange(sanphams.Where(sp => sp.Gia >= min && sp.Gia <= max));
                        }
                    }
                }

                // Lấy danh sách không trùng lặp (nếu nhiều khoảng trùng nhau)
                sanphams = filteredSanPhams.Distinct().AsQueryable();
            }

            // Sắp xếp (tùy chọn)
            switch (sortOrder)
            {
                case "price_asc":
                    sanphams = sanphams.OrderBy(s => s.Gia);
                    break;
                case "price_desc":
                    sanphams = sanphams.OrderByDescending(s => s.Gia);
                    break;
                case "name_asc":
                    sanphams = sanphams.OrderBy(s => s.TenSP);
                    break;
                case "name_desc":
                    sanphams = sanphams.OrderByDescending(s => s.TenSP);
                    break;
                default:
                    sanphams = sanphams.OrderBy(s => s.MaSP);
                    break;
            }

            var paged = sanphams.ToPagedList(page, pageSize);
            ViewBag.IsEmpty = !paged.Any(); // Gửi cờ để View xử lý

            return View(paged);
        }
        public ActionResult ProductDetail(int id)
        {
            SanPham sp = db.SanPhams.Include("DanhMuc").Where(s => s.MaSP == id).FirstOrDefault();
            List<SanPhamChiTiet> list = db.SanPhamChiTiets.Include("KichCo").Where(s => s.MaSP == id).ToList();
            ViewBag.SPCT = list;
            ViewBag.Exitst = list.FirstOrDefault();

            var danhGiaList = db.DanhGias.Where(dg => dg.MaSP == id)
                                         .OrderByDescending(dg => dg.NgayDanhGia)
                                         .ToList();
            ViewBag.DanhGias = danhGiaList;

            return View(sp);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult AddDanhGia(int MaSP, string HoTen, string NoiDung, int SoSao)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Người dùng chưa đăng nhập." });
            }
            try
            {
                if (SoSao < 1 || SoSao > 5)
                {
                    return Json(new { success = false, message = "Số sao phải từ 1 đến 5." });
                }

                if (string.IsNullOrWhiteSpace(HoTen) || HoTen.Length > 100)
                {
                    return Json(new { success = false, message = "Tên không được rỗng và tối đa 100 ký tự." });
                }

                if (string.IsNullOrWhiteSpace(NoiDung))
                {
                    return Json(new { success = false, message = "Nội dung đánh giá không được để trống." });
                }

                var danhGia = new DanhGia
                {
                    MaSP = MaSP,
                    HoTen = HoTen,
                    NoiDung = NoiDung,
                    SoSao = SoSao,
                    NgayDanhGia = DateTime.Now
                };

                db.DanhGias.Add(danhGia);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Cảm ơn bạn đã đánh giá sản phẩm.",
                    danhGia = new
                    {
                        HoTen = danhGia.HoTen,
                        NoiDung = danhGia.NoiDung,
                        SoSao = danhGia.SoSao,
                        NgayDanhGia = danhGia.NgayDanhGia.ToString("dd/MM/yyyy HH:mm")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lưu đánh giá: " + ex.Message });
            }
        }


        [HttpPost]
        public JsonResult Index(int id)
        {
            SanPham sp = db.SanPhams.Include("DanhMuc").Include("SanPhamChiTiets").Where(s => s.MaSP.Equals(id)).FirstOrDefault();
            return Json(sp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Detail(int id)
        {
            SanPhamChiTiet spct = db.SanPhamChiTiets.Where(sp => sp.IDCTSP == id).FirstOrDefault();
            return Json(spct, JsonRequestBehavior.AllowGet);
        }

        private static Expression<Func<T, bool>> OrElse<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }

    }
}
