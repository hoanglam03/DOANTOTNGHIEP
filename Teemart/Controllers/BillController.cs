using Nhom9.Models;
using Nhom9.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom9.Controllers
{
    public class BillController : Controller
    {
        private Nhom9DB db = new Nhom9DB();

        [HttpGet]

        public ActionResult PaymentConfirm()
        {
            // Lấy các tham số trả về từ VNPAY
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            VnPayLibrary vnpay = new VnPayLibrary();

            // Lấy tất cả dữ liệu được trả về
            foreach (string s in Request.QueryString)
            {
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s, Request.QueryString[s]);
                }
            }

            // Mã đơn hàng
            string orderId = Convert.ToString(Request.QueryString["vnp_TxnRef"]);
            // Mã giao dịch VNPAY
            string vnpayTranId = Convert.ToString(Request.QueryString["vnp_TransactionNo"]);
            // Mã phản hồi
            string vnp_ResponseCode = Convert.ToString(Request.QueryString["vnp_ResponseCode"]);
            // Mã ngân hàng
            string vnp_BankCode = Convert.ToString(Request.QueryString["vnp_BankCode"]);
            // Số tiền thanh toán
            string vnp_Amount = Convert.ToString(Request.QueryString["vnp_Amount"]);

            bool checkSignature = vnpay.ValidateSignature(Request.QueryString["vnp_SecureHash"], vnp_HashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00")
                {
                    // Thanh toán thành công
                    // Cập nhật trạng thái đơn hàng trong database
                    int maHoaDon = int.Parse(orderId);
                    var hoaDon = db.HoaDons.Find(maHoaDon);

                    if (hoaDon != null)
                    {
                        hoaDon.TrangThai = 1;
                        hoaDon.DiaChiNhan = vnpayTranId;
                        hoaDon.NgayDat = DateTime.Now;
                        //hoaDon.MaNganHang = vnp_BankCode;
                        db.Entry(hoaDon).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        // Xóa giỏ hàng sau khi thanh toán thành công
                        Session[Nhom9.Session.ConstainCart.CART] = null;
                    }

                    ViewBag.ThanhToan = "Thành công";
                    ViewBag.MaGiaoDich = vnpayTranId;
                    ViewBag.MaDonHang = orderId;
                }
                else
                {
                    // Thanh toán thất bại
                    // Cập nhật trạng thái đơn hàng
                    int maHoaDon = int.Parse(orderId);
                    var hoaDon = db.HoaDons.Find(maHoaDon);

                    if (hoaDon != null)
                    {
                        //hoaDon.TrangThai = "Thanh toán thất bại";
                        db.Entry(hoaDon).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    ViewBag.ThanhToan = "Thất bại";
                    ViewBag.MaDonHang = orderId;
                }
            }
            else
            {
                ViewBag.ThanhToan = "Có lỗi xảy ra trong quá trình xử lý";
            }

            return View();
        }
        public ActionResult ListBills()
        {
            if (Session[Nhom9.Session.ConstaintUser.USER_SESSION] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userSession = (Nhom9.Models.TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
            int maTK = userSession.MaTK;

            var danhSachDonHang = db.HoaDons
                                    .Where(h => h.MaTK == maTK)
                                    .OrderByDescending(h => h.NgayDat)
                                    .ToList();

            return View(danhSachDonHang);
        }




        public ActionResult Details(int id)
        {
            var hoaDon = db.HoaDons
                           .Include("ChiTietHoaDons.SanPhamChiTiet.SanPham")
                           .Include("ChiTietHoaDons.SanPhamChiTiet.KichCo")
                           .FirstOrDefault(h => h.MaHD == id);

            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            return View(hoaDon);
        }


    }
}