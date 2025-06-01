using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nhom9.Models
{
    public class MonthlyRevenueData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
}