using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;

namespace LTW.Controllers
{
    public class AdminCategoriesController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();
        // GET: AdminCategories
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        //THÊM DANH MỤC
        public ActionResult Create()
        {
            ViewBag.Parents = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Categories model)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Parents = db.Categories.ToList();
            return View(model);
        }

        //SỬA DANH MỤC
        public ActionResult Edit(int id)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return HttpNotFound();

            ViewBag.Parents = db.Categories
                                .Where(c => c.CategoryID != id)
                                .ToList();
            return View(cat);
        }

        [HttpPost]
        public ActionResult Edit(int id, Categories model)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return HttpNotFound();

            cat.CategoryName = model.CategoryName;
            cat.ParentID = model.ParentID;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //XÓA DANH MỤC
        public ActionResult Delete(int id)
        {
            try
            {
                var cat = db.Categories.Find(id);
                if (cat == null) return HttpNotFound();

                db.Categories.Remove(cat);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục này vì đang có sản phẩm liên kết.";
                return RedirectToAction("Index");
            }
        }

    }
}