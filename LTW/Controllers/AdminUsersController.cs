using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;

namespace LTW.Controllers
{
    public class AdminUsersController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        //DANH SÁCH KHÁCH HÀNG
        public ActionResult Index()
        {
            var customers = db.Users
                              .Where(u => u.VaiTro == "customer")
                              .ToList();

            return View(customers);
        }

        //THÊM TÀI KHOẢN (ADMIN / CUSTOMER)
        public ActionResult Create()
        {
            ViewBag.Roles = new[] { "admin", "customer" };
            return View();
        }

        [HttpPost]
        public ActionResult Create(Users model)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Roles = new[] { "admin", "customer" };
            return View(model);
        }
    }
}