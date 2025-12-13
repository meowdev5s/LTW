using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Controllers
{
    public class CartController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        //LẤY / TẠO CART
        private Cart GetOrCreateCart(int userId)
        {
            var cart = db.Cart.FirstOrDefault(c => c.UserID == userId);
            if (cart == null)
            {
                cart = new Cart { UserID = userId };
                db.Cart.Add(cart);
                db.SaveChanges();
            }
            return cart;
        }

        //HIỂN THỊ GIỎ HÀNG
        public ActionResult Index()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var user = (Users)Session["User"];
            var cart = GetOrCreateCart(user.UserID);

            var items = db.CartItems
                          .Where(ci => ci.CartID == cart.CartID)
                          .ToList();

            ViewBag.CartItems = items;
            ViewBag.TotalQuantity = items.Sum(i => i.Quantity);
            ViewBag.TotalPrice = items.Sum(i => i.Quantity * i.Products.Price);

            return View();
        }

        //THÊM SẢN PHẨM VÀO GIỎ
        public ActionResult AddToCart(int id)
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var user = (Users)Session["User"];
            var cart = GetOrCreateCart(user.UserID);

            var item = db.CartItems
                         .FirstOrDefault(ci => ci.CartID == cart.CartID && ci.ProductID == id);

            var product = db.Products.Find(id);

            if (item == null)
            {
                if (product.Stock <= 0)
                    return RedirectToAction("Index", "Home");

                db.CartItems.Add(new CartItems
                {
                    CartID = cart.CartID,
                    ProductID = id,
                    Quantity = 1
                });
            }
            else
            {
                if (item.Quantity < product.Stock)
                    item.Quantity += 1;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //TĂNG / GIẢM SỐ LƯỢNG
        //delta = 1 (tăng), -1 (giảm)
        public ActionResult UpdateQuantity(int id, int delta)
        {

            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var item = db.CartItems.Include("Products").FirstOrDefault(x => x.ItemID == id);

            if (item == null) return RedirectToAction("Index");

            if (delta > 0 && item.Quantity >= item.Products.Stock)
            {
                return RedirectToAction("Index"); //chặn vượt kho
            }

            item.Quantity += delta;

            if (item.Quantity <= 0)
                db.CartItems.Remove(item);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //XÓA SẢN PHẨM KHỎI GIỎ
        public ActionResult Remove(int id)
        {

            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var item = db.CartItems.Find(id);
            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //THANH TOÁN - TẠO ĐƠN HÀNG
        public ActionResult PaymentConfirm()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Account");

            var user = (Users)Session["User"];
            var cart = GetOrCreateCart(user.UserID);

            var items = db.CartItems
                          .Where(ci => ci.CartID == cart.CartID)
                          .ToList();

            if (!items.Any())
                return RedirectToAction("Index");

            Orders order = new Orders
            {
                UserID = user.UserID,
                OrderDate = DateTime.Now,
                Status = "pending",
                ShipName = user.FullName,
                ShipPhone = user.Phone,
                ShipAddress = user.DiaChi,
                TotalAmount = items.Sum(i => i.Quantity * i.Products.Price)
            };

            db.Orders.Add(order);
            db.SaveChanges();

            foreach (var i in items)
            {
                //Trừ tồn kho
                var product = db.Products.Find(i.ProductID);
                if (product.Stock < i.Quantity)
                {
                    //Phòng ngừa trường hợp kho thay đổi
                    return RedirectToAction("Index");
                }

                product.Stock -= i.Quantity;

                db.OrderItems.Add(new OrderItems
                {
                    OrderID = order.OrderID,
                    ProductID = i.ProductID,
                    Quantity = i.Quantity,
                    Price = product.Price
                });
            }

            //Xóa giỏ sau khi đặt hàng
            db.CartItems.RemoveRange(items);
            db.SaveChanges();

            return View("PaymentConfirm");
        }
    }
}