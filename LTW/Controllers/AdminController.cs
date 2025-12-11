using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;

namespace LTW.Controllers
{
    public class AdminController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        public ActionResult Index()
        {
            //Tổng số sản phẩm
            ViewBag.TotalProducts = db.Products.Count();

            //Tổng số danh mục
            ViewBag.TotalCategories = db.Categories.Count();

            //Tổng số đơn hàng
            ViewBag.TotalOrders = db.Orders.Count();

            //Tổng doanh thu
            ViewBag.TotalRevenue = db.Orders
                .Where(o => o.Status == "completed")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            //Đơn hàng theo trạng thái
            ViewBag.Pending = db.Orders.Count(o => o.Status == "pending");
            ViewBag.Shipping = db.Orders.Count(o => o.Status == "shipping");
            ViewBag.Completed = db.Orders.Count(o => o.Status == "completed");
            ViewBag.Cancelled = db.Orders.Count(o => o.Status == "cancelled");

            //Doanh thu theo tháng (12 tháng gần nhất)
            var now = DateTime.Now;
            var fromMonth = now.AddMonths(-11);

            var revenueChart = db.Orders
                .Where(o => o.Status == "completed" && o.OrderDate >= fromMonth)
                .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            //Truyền labels & data cho Chart.js
            ViewBag.Labels = string.Join(",", revenueChart.Select(x => $"'{x.Month}/{x.Year}'"));
            ViewBag.Revenues = string.Join(",", revenueChart.Select(x => x.Total));

            return View();
        }
    }
}