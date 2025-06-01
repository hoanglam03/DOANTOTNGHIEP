using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Nhom9.Models
{
    public class PaymentConfig
    {
        public static string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
        public static string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
        public static string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
        public static string vnp_ReturnUrl = ConfigurationManager.AppSettings["vnp_ReturnUrl"];
    }
}
