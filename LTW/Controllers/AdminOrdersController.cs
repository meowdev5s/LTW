using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;

namespace LTW.Controllers
{
    public class AdminOrdersController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        //TẤT CẢ ĐƠN HÀNG
        public ActionResult Index()
        {
            var orders = db.Orders.OrderByDescending(o => o.OrderID).ToList();

            ViewBag.ActiveMenu = "orders";
            ViewBag.ActiveSub = "all";

            return View(orders);
        }

        //ĐƠN ĐANG CHỜ XỬ LÝ
        public ActionResult Pending()
        {
            var orders = db.Orders
                           .Where(o => o.Status == "pending")
                           .OrderByDescending(o => o.OrderID)
                           .ToList();

            ViewBag.ActiveMenu = "orders";
            ViewBag.ActiveSub = "pending";

            return View("Index", orders);
        }

        //ĐƠN ĐANG GIAO
        public ActionResult Shipping()
        {
            var orders = db.Orders
                           .Where(o => o.Status == "shipping")
                           .OrderByDescending(o => o.OrderID)
                           .ToList();

            ViewBag.ActiveMenu = "orders";
            ViewBag.ActiveSub = "shipping";

            return View("Index", orders);
        }

        //ĐƠN ĐÃ HOÀN THÀNH
        public ActionResult Completed()
        {
            var orders = db.Orders
                           .Where(o => o.Status == "completed")
                           .OrderByDescending(o => o.OrderID)
                           .ToList();

            ViewBag.ActiveMenu = "orders";
            ViewBag.ActiveSub = "completed";

            return View("Index", orders);
        }

        //ĐƠN ĐÃ HỦY
        public ActionResult Cancelled()
        {
            var orders = db.Orders
                           .Where(o => o.Status == "cancelled")
                           .OrderByDescending(o => o.OrderID)
                           .ToList();

            ViewBag.ActiveMenu = "orders";
            ViewBag.ActiveSub = "cancelled";

            return View("Index", orders);
        }


        //CHUYỂN TRẠNG THÁI ĐƠN HÀNG
        public ActionResult UpdateStatus(int id, string status)
        {
            var order = db.Orders.Find(id);
            if (order == null) return HttpNotFound();

            string current = order.Status;

            //RÀNG BUỘC LOGIC 
            if (current == "completed" || current == "cancelled")
            {
                TempData["Error"] = "Đơn hàng đã hoàn thành hoặc đã hủy, không thể thay đổi.";
                return RedirectToAction("Index");
            }

            //pending -> chỉ được: confirmed, cancelled
            if (current == "pending" && (status != "confirmed" && status != "cancelled"))
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction("Index");
            }

            //confirmed -> chỉ shipping hoặc cancelled
            if (current == "confirmed" && (status != "shipping" && status != "cancelled"))
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction("Index");
            }

            //shipping -> chỉ completed
            if (current == "shipping" && status != "completed")
            {
                TempData["Error"] = "Đơn đang giao chỉ có thể chuyển sang 'Hoàn thành'.";
                return RedirectToAction("Index");
            }

            //OK -> cập nhật
            order.Status = status;
            db.SaveChanges();

            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Index");
        }
    }
}