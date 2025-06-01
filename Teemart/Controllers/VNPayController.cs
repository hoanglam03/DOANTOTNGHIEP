using System;
using System.Web.Mvc;
using System.Web;
using Nhom9.Models;

namespace Nhom9.Controllers
{
    public class VNPayController : Controller
    {
        [HttpPost]
        public ActionResult InitiatePayment(string orderId, decimal amount, string orderInfo)
        {
            string ipAddr = Request.UserHostAddress;
            string paymentUrl = VNPayHelper.CreatePaymentUrl(HttpContext, orderId, amount, orderInfo, ipAddr);
            return Redirect(paymentUrl);
        }

        [HttpGet]
        public ActionResult VNPayReturn()
        {
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["vnp_SecretKey"];
            bool isValid = VNPayHelper.ValidateSignature(Request, secretKey);

            if (isValid && Request.QueryString["vnp_ResponseCode"] == "00")
            {
                // Payment successful, update order status
                string orderId = Request.QueryString["vnp_TxnRef"];
                // TODO: Update your database to mark orderId as paid
                TempData["PaymentMessage"] = "Thanh toán thành công!";
                return RedirectToAction("OrderSuccess", "Cart");
            }
            else
            {
                // Payment failed
                TempData["PaymentMessage"] = "Thanh toán thất bại hoặc chữ ký không hợp lệ.";
                return RedirectToAction("CheckOut", "Cart");
            }
        }

        [HttpGet]
        public ActionResult OrderSuccess()
        {
            ViewBag.Message = TempData["PaymentMessage"];
            return View();
        }
    }
}