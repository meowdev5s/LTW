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
        public ActionResult Index()
        {
            return View(db.Products.ToList());

        }


    }
}