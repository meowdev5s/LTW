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
        public ActionResult Login()
        {
            return View();
        }

        //POST: Xử lý Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSubmit(Users _user, string regConfirmPass)
        {
            if (ModelState.IsValid)
            {
                //Trùng Username
                if (db.Users.Any(u => u.Username == _user.Username))
                {
                    ViewBag.ErrorRegister = "Tên đăng nhập đã tồn tại!";
                    ViewBag.ActiveTab = "register";
                    return View("Login");
                }

                //Trùng Email
                if (!string.IsNullOrEmpty(_user.Email) &&
                    db.Users.Any(u => u.Email == _user.Email))
                {
                    ViewBag.ErrorRegister = "Email đã được sử dụng!";
                    return View("Login");
                }

                //Trùng SĐT
                if (!string.IsNullOrEmpty(_user.Phone) &&
                    db.Users.Any(u => u.Phone == _user.Phone))
                {
                    ViewBag.ErrorRegister = "Số điện thoại đã được sử dụng!";
                    return View("Login");
                }

                //Xác nhận mật khẩu
                if (_user.PasswordHash != regConfirmPass)
                {
                    ViewBag.ErrorRegister = "Mật khẩu xác nhận không khớp!";
                    return View("Login");
                }

                _user.PasswordHash = GetMD5(_user.PasswordHash);
                _user.VaiTro = "customer";

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Users.Add(_user);
                db.SaveChanges();

                // Tạo giỏ hàng cho user mới
                Cart cart = new Cart
                {
                    UserID = _user.UserID
                };
                db.Cart.Add(cart);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            return View("Login");
        }

        //POST: Xử lý Đăng nhập
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
                    //Mã hóa mật khẩu người dùng nhập vào để so sánh với DB
                    string f_password = GetMD5(inputPass);

                    //Tìm User: Cho phép đăng nhập bằng cả Username HOẶC Email
                    var user = db.Users.FirstOrDefault(u =>
                        (u.Username == inputUser || u.Email == inputUser) &&
                        u.PasswordHash == f_password
                    );
                    if (user != null)
                    {
                        Session["User"] = user;

                        //Nếu là Admin thì chuyển trang Admin, khách thì về trang chủ
                        if (user.VaiTro == "admin")
                        {
                            return RedirectToAction("Index", "Admin");
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
            return View("Login");
        }

        //Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa session
            return RedirectToAction("Index", "Home");
        }

        //GET: Hiển thị trang hồ sơ
        public ActionResult Profile()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login");
            }

            //Lấy thông tin mới nhất từ DB để hiển thị (tránh trường hợp Session cũ)
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
                //Chỉ cập nhật các trường cho phép
                userInDb.FullName = _user.FullName;
                userInDb.Email = _user.Email;
                userInDb.Phone = _user.Phone;
                userInDb.DiaChi = _user.DiaChi;

                db.SaveChanges();

                //Cập nhật lại Session
                Session["User"] = userInDb;
                ViewBag.Message = "Cập nhật thông tin thành công!";
            }
            return View("Profile", userInDb);
        }

        //POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPass, string newPass, string confirmPass)
        {
            if (Session["User"] == null) return RedirectToAction("Login");

            var uSession = (Users)Session["User"];
            var userInDb = db.Users.Find(uSession.UserID);

            //Kiểm tra mật khẩu cũ
            string oldPassHash = GetMD5(currentPass);
            if (userInDb.PasswordHash != oldPassHash)
            {
                ViewBag.ErrorPass = "Mật khẩu hiện tại không đúng!";
                return View("Profile", userInDb);
            }

            //Kiểm tra xác nhận mật khẩu mới
            if (newPass != confirmPass)
            {
                ViewBag.ErrorPass = "Mật khẩu xác nhận không khớp!";
                return View("Profile", userInDb);
            }

            //Cập nhật mật khẩu mới
            userInDb.PasswordHash = GetMD5(newPass);
            db.SaveChanges();

            ViewBag.MessagePass = "Đổi mật khẩu thành công!";
            return View("Profile", userInDb);
        }

        //Hàm mã hóa MD5
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