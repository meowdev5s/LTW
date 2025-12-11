using LinhKienDienTu;
using LTW.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

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

        // POST: Xử lý Đăng ký
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

        // POST: Xử lý Đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginSubmit(FormCollection collect)
        {
            // Code cũ của bạn: string email = collect["Email"];
            // Sửa lại để khớp với "name" bên View HTML mình gửi (name="Username" và name="PasswordHash")
            string inputUser = collect["Username"];
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
                    // 3. Đăng nhập thành công -> Lưu Session
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

            // Đăng nhập thất bại thì trả về View cũ
            return View();
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa session
            return RedirectToAction("Index", "Home");
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