using Nhom9.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace Nhom9.Areas.Admin.Controllers
{
    public class ThongKeController : BaseController
    {
        Nhom9DB db = new Nhom9DB();

        [HttpGet]
        public ActionResult Index(int page = 1, int pageSize = 10, string period = "daily")
        {
            // Lấy dữ liệu sản phẩm
            var sanphams = db.SanPhams.OrderBy(sp => sp.MaSP).ToList();

            // Thêm phân trang
            var pagedSanPhams = sanphams.ToPagedList(page, pageSize);

            // Thống kê sản phẩm theo danh mục
            var categoryData = db.DanhMucs
                .Select(dm => new
                {
                    MaDM = dm.MaDM,
                    TenDM = dm.TenDanhMuc,
                    SoLuongSanPham = db.SanPhams.Count(sp => sp.MaDM == dm.MaDM)
                }).ToList();

            ViewBag.CategoryData = categoryData;

            // Top sản phẩm bán chạy
            var topSellingProducts = (from cthd in db.ChiTietHoaDons
                                      join spct in db.SanPhamChiTiets on cthd.IDCTSP equals spct.IDCTSP
                                      join sp in db.SanPhams on spct.MaSP equals sp.MaSP
                                      join hd in db.HoaDons on cthd.MaHD equals hd.MaHD
                                      where hd != null && (hd.TrangThai == 2 || hd.TrangThai == 3 || hd.TrangThai == 4)
                                      group new { cthd, sp } by new { sp.MaSP, sp.TenSP, sp.Gia } into productGroup
                                      let totalSold = productGroup.Sum(x => x.cthd.SoLuongMua)
                                      let totalRevenue = productGroup.Sum(x => x.cthd.SoLuongMua * x.cthd.GiaMua)
                                      orderby totalSold descending
                                      select new TopSellingProductsItem
                                      {
                                          MaSP = productGroup.Key.MaSP,
                                          TenSP = productGroup.Key.TenSP,
                                          Gia = productGroup.Key.Gia,
                                          TotalSold = totalSold,
                                          TotalRevenue = totalRevenue
                                      })
                .Take(10)
                .ToList();

            ViewBag.TopSellingProducts = topSellingProducts;

            // Sản phẩm bán chậm
            var slowestSellingProducts = (from sp in db.SanPhams
                                          join spct in db.SanPhamChiTiets on sp.MaSP equals spct.MaSP into spctGroup
                                          from spct in spctGroup.DefaultIfEmpty()
                                          join cthd in db.ChiTietHoaDons on spct.IDCTSP equals cthd.IDCTSP into cthdGroup
                                          from cthd in cthdGroup.DefaultIfEmpty()
                                          join hd in db.HoaDons on cthd.MaHD equals hd.MaHD into hdGroup
                                          from hd in hdGroup.DefaultIfEmpty()
                                         
                                          group new { sp, cthd, hd } by new { sp.MaSP, sp.TenSP, sp.Gia, sp.NgayTao } into productGroup
                                          let totalSold = productGroup
                                              .Where(x => x.hd != null && (x.hd.TrangThai == 2 || x.hd.TrangThai == 3 || x.hd.TrangThai == 4))
                                              .Sum(x => (int?)(x.cthd != null ? x.cthd.SoLuongMua : 0)) ?? 0
                                          orderby totalSold ascending, productGroup.Key.NgayTao ascending
                                          select new SlowestSellingProduct
                                          {
                                              MaSP = productGroup.Key.MaSP,
                                              TenSP = productGroup.Key.TenSP,
                                              Gia = productGroup.Key.Gia,
                                              TotalSold = totalSold,
                                              NgayTao = productGroup.Key.NgayTao
                                          })
                .Take(10)
                .ToList();

            ViewBag.SlowestSellingProducts = slowestSellingProducts;

            // Thống kê doanh thu theo ngày (7 ngày gần nhất)
            var dates = Enumerable.Range(0, 7)
                .Select(offset => DateTime.Now.Date.AddDays(-offset))
                .OrderBy(date => date)
                .ToList();

            var dailyRevenue = (from date in dates
                                join hd in db.HoaDons on date equals hd.NgayDat.Date into hdGroup
                                from hd in hdGroup.DefaultIfEmpty()
                                join cthd in db.ChiTietHoaDons on hd != null ? hd.MaHD : 0 equals cthd.MaHD into cthdGroup
                                from cthd in cthdGroup.DefaultIfEmpty()
                                where hd == null || hd.TrangThai == 2 || hd.TrangThai == 3 || hd.TrangThai == 4
                                group cthd by date into dailyGroup
                                select new RevenueData
                                {
                                    Date = dailyGroup.Key.ToString("yyyy-MM-dd"),
                                    Revenue = dailyGroup.Sum(ct => ct != null ? ct.SoLuongMua * ct.GiaMua : 0),
                                    OrderCount = dailyGroup.Count(ct => ct != null)
                                }).ToList();

            ViewBag.DailyRevenue = dailyRevenue;

            // Thống kê doanh thu theo tuần (4 tuần gần nhất)
            var weeks = Enumerable.Range(0, 4)
                .Select(i => new
                {
                    StartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 7 * i),
                    EndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 6 - 7 * i),
                    WeekNumber = i + 1
                })
                .OrderBy(w => w.StartDate)
                .ToList();

            var weeklyRevenue = (from week in weeks
                                 join hd in db.HoaDons on true equals true into hdGroup
                                 from hd in hdGroup.DefaultIfEmpty()
                                 join cthd in db.ChiTietHoaDons on hd != null ? hd.MaHD : 0 equals cthd.MaHD into cthdGroup
                                 from cthd in cthdGroup.DefaultIfEmpty()
                                 where (hd == null || (hd.TrangThai == 2 || hd.TrangThai == 3 || hd.TrangThai == 4)) &&
                                       (hd == null || (hd.NgayDat >= week.StartDate && hd.NgayDat <= week.EndDate))
                                 group cthd by new { week.StartDate, week.EndDate, week.WeekNumber } into weeklyGroup
                                 select new WeeklyRevenueData
                                 {
                                     WeekNumber = weeklyGroup.Key.WeekNumber,
                                     StartDate = weeklyGroup.Key.StartDate.ToString("yyyy-MM-dd"),
                                     EndDate = weeklyGroup.Key.EndDate.ToString("yyyy-MM-dd"),
                                     Revenue = weeklyGroup.Sum(ct => ct != null ? ct.SoLuongMua * ct.GiaMua : 0),
                                     OrderCount = weeklyGroup.Count(ct => ct != null)
                                 }).ToList();

            ViewBag.WeeklyRevenue = weeklyRevenue;

            // Thống kê doanh thu theo tháng (6 tháng gần nhất)
            var months = Enumerable.Range(0, 6)
                .Select(offset => DateTime.Now.AddMonths(-offset))
                .OrderBy(date => date)
                .ToList();

            var monthlyRevenue = (from month in months
                                  join hd in db.HoaDons on true equals true into hdGroup
                                  from hd in hdGroup.DefaultIfEmpty()
                                  join cthd in db.ChiTietHoaDons on hd != null ? hd.MaHD : 0 equals cthd.MaHD into cthdGroup
                                  from cthd in cthdGroup.DefaultIfEmpty()
                                  where (hd == null || (hd.TrangThai == 2 || hd.TrangThai == 3 || hd.TrangThai == 4)) &&
                                        (hd == null || (hd.NgayDat.Year == month.Year && hd.NgayDat.Month == month.Month))
                                  group cthd by new { month.Year, month.Month } into monthlyGroup
                                  select new MonthlyRevenueData
                                  {
                                      Year = monthlyGroup.Key.Year,
                                      Month = monthlyGroup.Key.Month,
                                      MonthName = new DateTime(monthlyGroup.Key.Year, monthlyGroup.Key.Month, 1).ToString("MM/yyyy"),
                                      Revenue = monthlyGroup.Sum(ct => ct != null ? ct.SoLuongMua * ct.GiaMua : 0),
                                      OrderCount = monthlyGroup.Count(ct => ct != null)
                                  }).ToList();

            ViewBag.MonthlyRevenue = monthlyRevenue;

            // Thống kê doanh thu theo năm (1 năm gần nhất)
            var years = Enumerable.Range(0, 1)
                .Select(offset => DateTime.Now.Year - offset)
                .OrderBy(year => year)
                .ToList();

            var yearlyRevenue = (from year in years
                                 join hd in db.HoaDons on true equals true into hdGroup
                                 from hd in hdGroup.DefaultIfEmpty()
                                 join cthd in db.ChiTietHoaDons on hd != null ? hd.MaHD : 0 equals cthd.MaHD into cthdGroup
                                 from cthd in cthdGroup.DefaultIfEmpty()
                                 where (hd == null || (hd.TrangThai == 2 || hd.TrangThai == 3 || hd.TrangThai == 4)) &&
                                       (hd == null || hd.NgayDat.Year == year)
                                 group cthd by year into yearlyGroup
                                 select new YearlyRevenueData
                                 {
                                     Year = yearlyGroup.Key,
                                     Revenue = yearlyGroup.Sum(ct => ct != null ? ct.SoLuongMua * ct.GiaMua : 0),
                                     OrderCount = yearlyGroup.Count(ct => ct != null)
                                 }).ToList();

            ViewBag.YearlyRevenue = yearlyRevenue;

            // Truyền dữ liệu phân trang qua View
            return View(pagedSanPhams);
        }

    }
}