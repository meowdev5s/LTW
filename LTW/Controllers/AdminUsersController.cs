using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                //Kiểm tra trùng Username
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                    ViewBag.Roles = new[] { "admin", "customer" };
                    return View(model);
                }

                //Kiểm tra trùng Email
                if (!string.IsNullOrEmpty(model.Email) &&
                    db.Users.Any(u => u.Email == model.Email))
                {
                    ViewBag.Error = "Email đã được sử dụng!";
                    ViewBag.Roles = new[] { "admin", "customer" };
                    return View(model);
                }

                //Kiểm tra trùng Số điện thoại
                if (!string.IsNullOrEmpty(model.Phone) &&
                    db.Users.Any(u => u.Phone == model.Phone))
                {
                    ViewBag.Error = "Số điện thoại đã được sử dụng!";
                    ViewBag.Roles = new[] { "admin", "customer" };
                    return View(model);
                }

                //Mã hóa mật khẩu
                model.PasswordHash = GetMD5(model.PasswordHash);

                db.Users.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Roles = new[] { "admin", "customer" };
            return View(model);
        }

        // Hàm mã hóa MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}