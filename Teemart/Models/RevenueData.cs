using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom9.Models
{
    public class RevenueData
    {
        public string Date { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

}