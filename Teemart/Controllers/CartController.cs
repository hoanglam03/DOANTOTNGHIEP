using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nhom9.Models;

namespace Nhom9.Controllers
{
    public class CartController : Controller
    {
        Nhom9DB db = new Nhom9DB();
        // GET: Cart
        [HttpGet]
        public ActionResult Orders()
        {
            List<SanPhamChiTiet> list = new List<SanPhamChiTiet>();
            if (Session[Nhom9.Session.ConstainCart.CART] != null)
            {
                List<ChiTietHoaDon> ses = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
                foreach (ChiTietHoaDon item in ses)
                {
                    list.Add(db.SanPhamChiTiets.Include("SanPham").Include("KichCo").Where(s => s.IDCTSP == item.IDCTSP).FirstOrDefault());
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].ChiTietHoaDons.Add(ses[i]);
                }
            }
            return View(list);
        }



        [HttpPost]
        public ActionResult CreateOrder(int matk, string hotennguoinhan, string sodienthoainhan, string diachinhan, string ghichu, string paymentMethod)
        {
            try
            {
                // Lấy thông tin người dùng
                TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
                if (tk == null || tk.MaTK != matk)
                {
                    return RedirectToAction("Login", "Home");
                }

                // Lấy thông tin giỏ hàng
                List<ChiTietHoaDon> gioHang = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
                if (gioHang == null || gioHang.Count == 0)
                {
                    return RedirectToAction("Orders", "Cart");
                }

                // Tạo hóa đơn mới
                HoaDon hoaDon = new HoaDon();
                hoaDon.MaTK = matk;
                hoaDon.HoTenNguoiNhan = hotennguoinhan;
                hoaDon.SoDienThoaiNhan = sodienthoainhan;
                hoaDon.DiaChiNhan = diachinhan;
                hoaDon.GhiChu = ghichu;
                hoaDon.NgayDat = DateTime.Now;
                hoaDon.TrangThai = 1;
                //hoaDon.PhuongThucThanhToan = paymentMethod;

                // Lưu hóa đơn vào database
                db.HoaDons.Add(hoaDon);
                db.SaveChanges();

                // Lưu chi tiết hóa đơn
                foreach (var item in gioHang)
                {
                    item.MaHD = hoaDon.MaHD;
                    db.ChiTietHoaDons.Add(item);

                    // Cập nhật số lượng sản phẩm
                    var sanPham = db.SanPhamChiTiets.Find(item.IDCTSP);
                    if (sanPham != null)
                    {
                        sanPham.SoLuong -= item.SoLuongMua;
                        db.Entry(sanPham).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                db.SaveChanges();

                // Xóa giỏ hàng
                Session[Nhom9.Session.ConstainCart.CART] = null;

                // Chuyển hướng đến trang xác nhận đơn hàng
                return RedirectToAction("OrderConfirmation", "Cart", new { id = hoaDon.MaHD });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return RedirectToAction("Orders", "Cart");
            }
        }

        public ActionResult OrderConfirmation(int id)
        {
            var hoaDon = db.HoaDons.Find(id);
            if (hoaDon == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(hoaDon);
        }


        [HttpPost]
        public JsonResult AddToCart(ChiTietHoaDon chiTiet)
        {
            
            var cart = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
            var thisItemIncCart = cart?.FirstOrDefault(x => x.IDCTSP == chiTiet.IDCTSP);
            var numberExistInCart = 0;
            if (thisItemIncCart != null)
            {
                numberExistInCart = thisItemIncCart.SoLuongMua;
            }
            if (chiTiet.SoLuongMua + numberExistInCart > db.SanPhamChiTiets.Where(x => x.IDCTSP == chiTiet.IDCTSP).FirstOrDefault().SoLuong)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            bool isExists = false;
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();
            if (Session[Nhom9.Session.ConstainCart.CART] != null)
            {
                list = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
                foreach (ChiTietHoaDon item in list)
                {
                    if (item.IDCTSP == chiTiet.IDCTSP)
                    {
                        item.SoLuongMua += chiTiet.SoLuongMua;
                        isExists = true;
                    }
                }
                if (!isExists)
                {
                    list.Add(chiTiet);
                }
            }
            else
            {
                list = new List<ChiTietHoaDon>();
                list.Add(chiTiet);
            }
            list.RemoveAll((x) => x.SoLuongMua <= 0);
            foreach (ChiTietHoaDon item in list)
            {
                item.GiaMua = db.SanPhamChiTiets.Include("SanPham").Where(s => s.IDCTSP == item.IDCTSP).FirstOrDefault().SanPham.Gia;
            }
            Session[Nhom9.Session.ConstainCart.CART] = list;
            return Json(new { status = true, cart = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFromCart(int idctsp)
        {
            List<ChiTietHoaDon> list = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
            list.RemoveAll((x) => x.IDCTSP == idctsp);
            Session[Nhom9.Session.ConstainCart.CART] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateQuantity(int idctsp, int quantity)
        {
            
            var cart = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
            var item = cart?.FirstOrDefault(x => x.IDCTSP == idctsp);
            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy sản phẩm trong giỏ hàng!",
                }, JsonRequestBehavior.AllowGet);
            }
            if (quantity > db.SanPhamChiTiets.Where(x => x.IDCTSP == item.IDCTSP).FirstOrDefault().SoLuong)
            {
                return Json(new
                {
                    success = false,
                    message = "Số lượng sản phẩm trong kho không đủ !",
                    originalQuantity = item.SoLuongMua // Trả lại giá trị số lượng gốc
                }, JsonRequestBehavior.AllowGet);
            }
            
            if (quantity < 1)
            {
                return Json(new
                {
                    success = false,
                    message = "Số lượng phải lớn hơn hoặc bằng 1!",
                    originalQuantity = item.SoLuongMua // Trả lại giá trị số lượng gốc
                }, JsonRequestBehavior.AllowGet);
            }

            item.SoLuongMua = quantity;
            return Json(new
            {
                success = true,
                updatedQuantity = item.SoLuongMua
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckOut()
        {
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
            if (tk == null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<SanPhamChiTiet> list = new List<SanPhamChiTiet>();
            List<ChiTietHoaDon> ses = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
            ViewBag.TaiKhoan = tk;
            foreach (ChiTietHoaDon item in ses)
            {
                list.Add(db.SanPhamChiTiets.Include("SanPham").Include("KichCo").Where(s => s.IDCTSP == item.IDCTSP).FirstOrDefault());
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ChiTietHoaDons.Add(ses[i]);
            }
            return View(list);
        }

        [HttpPost]
        public JsonResult CreatePaymentVNPay(int matk, string hotennguoinhan, string sodienthoainhan, string diachinhan, string ghichu, string tongtien)
        {
            try
            {
                // Lấy thông tin người dùng
                TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
                if (tk == null || tk.MaTK != matk)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để tiếp tục!" }, JsonRequestBehavior.AllowGet);
                }

                // Lấy thông tin giỏ hàng
                List<ChiTietHoaDon> gioHang = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
                if (gioHang == null || gioHang.Count == 0)
                {
                    return Json(new { success = false, message = "Giỏ hàng trống!" }, JsonRequestBehavior.AllowGet);
                }

                // Tạo hóa đơn mới
                HoaDon hoaDon = new HoaDon();
                hoaDon.MaTK = matk;
                hoaDon.HoTenNguoiNhan = hotennguoinhan;
                hoaDon.SoDienThoaiNhan = sodienthoainhan;
                hoaDon.DiaChiNhan = diachinhan;
                hoaDon.GhiChu = ghichu;
                hoaDon.NgayDat = DateTime.Now;
                //hoaDon.TrangThai = "Chờ thanh toán";
                //hoaDon.PhuongThucThanhToan = "VNPAY";

                // Lưu hóa đơn vào database
                db.HoaDons.Add(hoaDon);
                db.SaveChanges();

                // Lưu chi tiết hóa đơn
                foreach (var item in gioHang)
                {
                    item.MaHD = hoaDon.MaHD;
                    db.ChiTietHoaDons.Add(item);

                    // Cập nhật số lượng sản phẩm
                    var sanPham = db.SanPhamChiTiets.Find(item.IDCTSP);
                    if (sanPham != null)
                    {
                        sanPham.SoLuong -= item.SoLuongMua;
                        db.Entry(sanPham).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                db.SaveChanges();

                // Chuyển đổi tổng tiền từ string sang decimal
                decimal tongTienDecimal;
                if (!decimal.TryParse(tongtien.Replace(".", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out tongTienDecimal))
                {
                    tongTienDecimal = gioHang.Sum(x => x.GiaMua * x.SoLuongMua);
                }

                // Cấu hình thông tin thanh toán VNPAY
                string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
                string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
                string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

                // Tạo các tham số thanh toán
                VnPayLibrary vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (tongTienDecimal * 100).ToString("0")); // Số tiền * 100 (VND)
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang #" + hoaDon.MaHD);
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", hoaDon.MaHD.ToString()); // Mã tham chiếu đơn hàng

                // Tạo URL thanh toán
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                // Lưu thông tin hóa đơn vào session để xử lý sau khi thanh toán
                Session["HoaDonVNPay"] = hoaDon.MaHD;

                return Json(new { success = true, paymentUrl = paymentUrl }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xử lý thanh toán: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}