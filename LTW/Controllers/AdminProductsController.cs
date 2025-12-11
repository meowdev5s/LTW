using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW.Models;

namespace LTW.Controllers
{
    public class AdminProductsController : Controller
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        // GET: AdminProducts
        //DANH SÁCH SẢN PHẨM + TÌM KIẾM
        public ActionResult Index(string keyword, int? categoryId)
        {
            string keywordNoAccent = RemoveUnicode(keyword ?? "").ToLower();

            var products = db.Products.ToList();

            //LỌC DANH MỤC 
            if (categoryId != null)
            {
                //Lấy danh mục con
                var childIds = db.Categories
                                 .Where(c => c.ParentID == categoryId)
                                 .Select(c => c.CategoryID)
                                 .ToList();

                //Bao gồm cả danh mục cha
                childIds.Add(categoryId.Value);

                products = products
                    .Where(p => childIds.Contains(p.CategoryID))
                    .ToList();
            }

            //TÌM KIẾM KHÔNG DẤU 
            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p =>
                    RemoveUnicode(p.ProductName).ToLower().Contains(keywordNoAccent) ||
                    RemoveUnicode(p.SKU ?? "").ToLower().Contains(keywordNoAccent)
                ).ToList();
            }

            //Truyền danh mục để hiển thị lọc
            ViewBag.Categories = db.Categories
                                   .Where(c => c.ParentID == null)
                                   .ToList();

            ViewBag.SelectedCategory = categoryId;

            return View(products);
        }

        //HÀM BỎ DẤU TIẾNG VIỆT
        private string RemoveUnicode(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            string[] arr1 = new string[] {
                "á","à","ả","ã","ạ","ă","ắ","ằ","ẳ","ẵ","ặ","â","ấ","ầ","ẩ","ẫ","ậ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ"
            };

            string[] arr2 = new string[] {
                "a","a","a","a","a","a","a","a","a","a","a","a","a","a","a","a","a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y"
            };

            for (int i = 0; i < arr1.Length; i++)
                text = text.Replace(arr1[i], arr2[i]).Replace(arr1[i].ToUpper(), arr2[i].ToUpper());

            return text;
        }

        //THÊM SẢN PHẨM
        public ActionResult Create()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Products model)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0]; // file hình đầu tiên

                if (file != null && file.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string path = Server.MapPath("~/Images/Products/" + fileName);

                    file.SaveAs(path);

                    model.ImageURL = "Products/" + fileName;
                }
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }

        //SỬA SẢN PHẨM
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.Categories = db.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Products model)
        {
            var product = db.Products.Find(model.ProductID);

            //Upload hình mới nếu có
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string path = Server.MapPath("~/Images/Products/" + fileName);

                    file.SaveAs(path);

                    product.ImageURL = "Products/" + fileName;
                }
            }

            // Cập nhật thông tin khác
            product.ProductName = model.ProductName;
            product.SKU = model.SKU;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Unit = model.Unit;
            product.CategoryID = model.CategoryID;

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //XÓA SẢN PHẨM
        public ActionResult Delete(int id)
        {
            var p = db.Products.Find(id);
            if (p == null) return HttpNotFound();

            db.Products.Remove(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //XÓA NHIỀU SẢN PHẨM
        [HttpPost]
        public ActionResult DeleteMultiple(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return RedirectToAction("Index");

            foreach (var id in ids)
            {
                var p = db.Products.Find(id);
                if (p != null)
                {
                    db.Products.Remove(p);
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}