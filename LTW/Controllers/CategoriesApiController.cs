using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LTW.Models;

namespace LTW.Controllers
{
    public class CategoriesApiController : ApiController
    {
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();

        //Lấy tất cả danh mục
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            var data = db.Categories
                         .Select(c => new
                         {
                             c.CategoryID,
                             c.CategoryName,
                             c.ParentID
                         }).ToList();

            return Ok(data);
        }

        //Lấy danh mục theo ID
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return NotFound();

            return Ok(new
            {
                cat.CategoryID,
                cat.CategoryName,
                cat.ParentID
            });
        }

        //Thêm danh mục
        [HttpPost]
        public IHttpActionResult Post(Categories model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Không hợp lệ");

            db.Categories.Add(model);
            db.SaveChanges();

            return Ok(new { message = "Đã thêm thành công", model.CategoryID });
        }

        //Sửa danh mục
        [HttpPut]
        public IHttpActionResult Put(int id, Categories model)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return NotFound();

            cat.CategoryName = model.CategoryName;
            cat.ParentID = model.ParentID;

            db.SaveChanges();

            return Ok(new { message = "Đã cập nhật thành công" });
        }

        //Xóa danh mục
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return NotFound();

            db.Categories.Remove(cat);
            db.SaveChanges();

            return Ok(new { message = "Đã xóa thành công" });
        }
    }
}
