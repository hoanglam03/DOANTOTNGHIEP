
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace Nhom9.Models
{
    public class VNPayHelper
    {
        public static string CreatePaymentUrl(HttpContextBase context, string orderId, decimal amount, string orderInfo, string ipAddr)
        {
            string vnp_PayUrl = System.Configuration.ConfigurationManager.AppSettings["vnp_PayUrl"];
            string vnp_ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["vnp_ReturnUrl"];
            string vnp_TmnCode = System.Configuration.ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_SecretKey = System.Configuration.ConfigurationManager.AppSettings["vnp_SecretKey"];

            var vnp_Params = new SortedDictionary<string, string>
        {
            {"vnp_Version", "2.1.0"},
            {"vnp_Command", "pay"},
            {"vnp_TmnCode", vnp_TmnCode},
            {"vnp_Amount", ((int)(amount * 100)).ToString()}, // VNPay requires amount * 100
            {"vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
            {"vnp_CurrCode", "VND"},
            {"vnp_IpAddr", ipAddr},
            {"vnp_Locale", "vn"},
            {"vnp_OrderInfo", orderInfo},
            {"vnp_OrderType", "billpayment"},
            {"vnp_ReturnUrl", vnp_ReturnUrl},
            {"vnp_TxnRef", orderId}
        };

            string queryString = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            string checksum = HmacSHA512(vnp_SecretKey, queryString);
            queryString += $"&vnp_SecureHash={checksum}";

            return $"{vnp_PayUrl}?{queryString}";
        }

        public static bool ValidateSignature(HttpRequestBase request, string secretKey)
        {
            string vnp_SecureHash = request.QueryString["vnp_SecureHash"];
            var vnp_Params = request.QueryString.AllKeys
                .Where(k => k != "vnp_SecureHash")
                .OrderBy(k => k)
                .ToDictionary(k => k, k => request.QueryString[k]);

            string queryString = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            string checksum = HmacSHA512(secretKey, queryString);

            return vnp_SecureHash.Equals(checksum, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}