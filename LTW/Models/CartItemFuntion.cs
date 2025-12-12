using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class CartItemFuntion
	{
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageURL { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal ThanhTien
        {
            get { return Price * Quantity; }
        }
        LinhKienDienTuEntities_ db = new LinhKienDienTuEntities_();
        public CartItemFuntion(int id)
        {
            Products sp = db.Products.FirstOrDefault(x => x.ProductID == id);
            if (sp != null)
            {
                ProductID = sp.ProductID;
                ProductName = sp.ProductName;
                ImageURL = sp.ImageURL;
                Price = sp.Price;
                Quantity = 1;
            }
        }
    }
}