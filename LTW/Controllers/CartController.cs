using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;

namespace LTW.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        public ActionResult Index()
        {
            CartFuntion cart = Session["Cart"] as CartFuntion;
            if (cart == null)
            {
                cart = new CartFuntion();
                Session["Cart"] = cart;   // phải gán lại
            }
            return View(cart);
        }
        public ActionResult AddToCart(int id)
        {
            if (Session["User"] == null) //phải đăng nhập mới cho thêm vào giỏ hàng
            {
                return RedirectToAction("Login", "Account");
            }
            CartFuntion cart = (CartFuntion)Session["Cart"];
            if (cart == null)
            {
                cart = new CartFuntion();
            }
            int result = cart.Them(id);
            if (result == 1) //thêm thành công
            {
                Session["Cart"] = cart; //cập nhật lại giỏ hàng trong session

            }
            else
            {
                return RedirectToAction("Detail", "Home", new { id = id });
            }

            return RedirectToAction("Index", "Home");
        }
        public ActionResult RemoveFromCart(int id)
        {
            if (Session["User"] == null) //phải đăng nhập mới cho thêm vào giỏ hàng
            {
                return RedirectToAction("Login", "User");
            }
            CartFuntion cart = (CartFuntion)Session["Cart"];
            if (cart == null)
            {
                cart = new CartFuntion();
            }
            int result = cart.Xoa(id);
            if (result == 1) //thêm thành công
            {
                Session["Cart"] = cart; //cập nhật lại giỏ hàng trong session

            }
            return RedirectToAction("Index", "Cart");
        }
        public ActionResult UpdateSLCart(int id, int num)
        {
            int result = -1;
            CartFuntion cart = (CartFuntion)Session["Cart"];
            if (num == -1)
            {
                result = cart.Giam(id);
            }
            else
            {
                result = cart.Them(id);
            }

            if (result == 1) // thành công
            {
                Session["Cart"] = cart;

            }

            return RedirectToAction("Index", "Cart");
        }
        public ActionResult PaymentConfirm()
        {
            var khachHang = (Users)Session["User"];
            CartFuntion cart = (CartFuntion)Session["Cart"];
            if (cart == null || cart.list.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            Orders hoaDon = new Orders
            {
                UserID = khachHang.UserID,
                //MaNV = 1,
                OrderDate = DateTime.Now,
                TotalAmount = cart.TongThanhTien(),
                //Status = 1,
                ShipAddress = khachHang.DiaChi,
                //DaThanhToan = false
            };

            db.Orders.Add(hoaDon);
            db.SaveChanges();

            int maHoaDonMoi = hoaDon.OrderID;

            foreach (var item in cart.list)
            {
                OrderItems chiTiet = new OrderItems
                {
                    OrderID = maHoaDonMoi, // Gán MaHD đã lấy được
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price // Giá bán tại thời điểm đặt hàng
                };
                db.OrderItems.Add(chiTiet);
            }

            db.SaveChanges();

            cart = new CartFuntion();
            Session["Cart"] = cart; //cập nhật lại giỏ hàng trong session
            return View();
        }
    }
}