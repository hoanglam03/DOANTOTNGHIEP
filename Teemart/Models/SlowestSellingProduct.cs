using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom9.Models
{
    public class SlowestSellingProduct
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string HinhAnh { get; set; }
        public decimal Gia { get; set; }
        public int TotalSold { get; set; }
        public System.DateTime NgayTao { get; set; }
    }
}