using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Controllers
{
    public class HomeController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();
        //public ActionResult Index()
        //{
        //    return View(db.Products.ToList());
        //}
                
        public ActionResult Detail(int id)
        {
            Products sp = db.Products.FirstOrDefault(x => x.ProductID == id);
            ViewBag.SanPhamLienQuan = db.Products.Where(x => x.CategoryID == sp.CategoryID && x.ProductID != id).Take(4).ToList();
            return View(sp);
        }
        public ActionResult Index(int? categoryId)
        {
            using (var db = new LinhKienDienTuEntities_())
            {
                var products = db.Products.AsQueryable();

                if (categoryId != null && categoryId > 0)
                {
                    // Lấy tất cả danh mục con
                    var allCategories = db.Categories.ToList();
                    var childIds = allCategories
                                    .Where(c => c.ParentID == categoryId)
                                    .Select(c => c.CategoryID)
                                    .ToList();

                    // Thêm chính danh mục cha
                    childIds.Add(categoryId.Value);

                    // Lọc sản phẩm theo tất cả các CategoryID
                    products = products.Where(p => childIds.Contains(p.CategoryID));
                }

                return View(products.ToList());
            }
        }

        public ActionResult TimKiemTheoTuKhoa(string keyword)
        {
            List<Products> list = db.Products.Where(x => x.ProductName.ToLower().Contains(keyword.ToLower())).ToList();
            return View("Index", list);
        }
    }
}