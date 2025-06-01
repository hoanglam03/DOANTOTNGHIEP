using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace Nhom9.Models
{
    public class WeeklyRevenueData
    {
        public int WeekNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
}