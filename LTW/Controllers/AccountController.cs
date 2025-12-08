using System.Linq;
using System.Web.Mvc;
using LinhKienDienTu;
using System.Security.Cryptography;
using System.Text;
using LTW.Models;
using System.Runtime.Serialization;
using Microsoft.Ajax.Utilities;

namespace LinhKienDienTu.Controllers
{
    public class AccountController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        // GET: Account/Login
        // Trang này chứa cả form Đăng nhập và Đăng ký (dạng Tabs)
        public ActionResult Login()
        {
            return View();
        }

        // POST: Xử lý Đăng Nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginSubmit(FormCollection collect)
        {
            if (ModelState.IsValid)
            {
                string inputUser = collect["UserName"];
                string inputPass = collect["PasswordHash"];
                if (!string.IsNullOrEmpty(inputUser) && !string.IsNullOrEmpty(inputPass))
                {
                    // 1. Mã hóa mật khẩu người dùng nhập vào để so sánh với DB
                    string f_password = GetMD5(inputPass);

                    // 2. Tìm User: Cho phép đăng nhập bằng cả Username HOẶC Email
                    var user = db.Users.FirstOrDefault(u =>
                        (u.Username == inputUser || u.Email == inputUser) &&
                        u.PasswordHash == f_password
                    );
                    if (user != null)
                    {
                        Session["User"] = user;

                        // Nếu là Admin thì chuyển trang Admin, khách thì về trang chủ
                        if (user.VaiTro == "admin")
                        {
                            // return RedirectToAction("Index", "Admin"); // Ví dụ sau này có trang Admin
                            return RedirectToAction("Index", "Home");
                        }
                        return RedirectToAction("Index", "Home");
  


                    }
                    else
                    {
                        ViewBag.ErrorLogin = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    }
                }
                else
                {
                    ViewBag.ErrorLogin = "Vui lòng nhập đầy đủ thông tin!";
                }

            }
            return View();
        }

        // POST: Xử lý Đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSubmit(Users _user, string regConfirmPass)
        {
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(s => s.Username == _user.Username);
                if (check == null)
                {
                    if (_user.PasswordHash != regConfirmPass)
                    {
                        ViewBag.ErrorRegister = "Mật khẩu xác nhận không khớp!";
                        return View("Login");
                    }

                    _user.PasswordHash = GetMD5(_user.PasswordHash); // Mã hóa pass
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.ErrorRegister = "Tên đăng nhập đã tồn tại!";
                    return View("Login");
                }
            }
            return View("Login");
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa session
            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // QUẢN LÝ HỒ SƠ CÁ NHÂN
        // ==========================================

        // GET: Hiển thị trang hồ sơ
        public ActionResult Profile()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login");
            }

            // Lấy thông tin mới nhất từ DB để hiển thị (tránh trường hợp Session cũ)
            var uSession = (Users)Session["User"];
            var user = db.Users.Find(uSession.UserID);

            return View(user);
        }

        // POST: Cập nhật thông tin cá nhân
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Users _user)
        {
            if (Session["User"] == null) return RedirectToAction("Login");

            var userInDb = db.Users.Find(_user.UserID);
            if (userInDb != null)
            {
                // Chỉ cập nhật các trường cho phép
                userInDb.FullName = _user.FullName;
                userInDb.Email = _user.Email;
                userInDb.Phone = _user.Phone;
                userInDb.DiaChi = _user.DiaChi;

                db.SaveChanges();

                // Cập nhật lại Session
                Session["User"] = userInDb;
                ViewBag.Message = "Cập nhật thông tin thành công!";
            }
            return View("Profile", userInDb);
        }

        // POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPass, string newPass, string confirmPass)
        {
            if (Session["User"] == null) return RedirectToAction("Login");

            var uSession = (Users)Session["User"];
            var userInDb = db.Users.Find(uSession.UserID);

            // 1. Kiểm tra mật khẩu cũ
            string oldPassHash = GetMD5(currentPass);
            if (userInDb.PasswordHash != oldPassHash)
            {
                ViewBag.ErrorPass = "Mật khẩu hiện tại không đúng!";
                return View("Profile", userInDb);
            }

            // 2. Kiểm tra xác nhận mật khẩu mới
            if (newPass != confirmPass)
            {
                ViewBag.ErrorPass = "Mật khẩu xác nhận không khớp!";
                return View("Profile", userInDb);
            }

            // 3. Cập nhật mật khẩu mới
            userInDb.PasswordHash = GetMD5(newPass);
            db.SaveChanges();

            ViewBag.MessagePass = "Đổi mật khẩu thành công!";
            return View("Profile", userInDb);
        }

        // Hàm mã hóa MD5 (Dùng cho demo đồ án)
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