using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTW.Models
{
	public class CartFuntion
	{
        public List<CartItemFuntion> list;
        public CartFuntion()
        {
            list = new List<CartItemFuntion>();
        }
        public CartFuntion(List<CartItemFuntion> listGH)
        {
            list = listGH;
        }
        //Tính số mặt hàng
        public int SoMatHang()
        {
            return list.Count;
        }
        //Tính tổng số lượng đặt hàng
        public int TongSLHang()
        {
            return list.Sum(x => x.Quantity);
        }
        //Tính tổng thành tiền
        public decimal TongThanhTien()
        {
            return list.Sum(x => x.ThanhTien);
        }
        //Thêm sản phẩm vào giỏ hàng
        public int Them(int id)
        {
            CartItemFuntion sanpham = list.Find(x => x.ProductID == id); //Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
            if (sanpham == null) //nếu chưa thì thêm mới vào
            {
                CartItemFuntion sp = new CartItemFuntion(id);
                if (sp == null)
                    return -1;
                list.Add(sp);

            }
            else //đã tồn tại thì tăng số lượng lên 1
            {
                sanpham.Quantity++;
            }
            return 1;
        }
        public int Xoa(int id)
        {
            CartItemFuntion sanpham = list.Find(x => x.ProductID == id); //Kiểm tra xem sản phẩm có tồn tại trong giỏ hàng không
            if (sanpham != null) //giảm đi 1
            {
                list.Remove(sanpham);

            }
            else //không tồn tại sản phẩm
            {
                return -1;
            }
            return 1;
        }
        public int Giam(int id)
        {
            CartItemFuntion sanpham = list.Find(x => x.ProductID == id); //Kiểm tra xem sản phẩm có tồn tại trong giỏ hàng không
            if (sanpham != null) //giảm đi 1
            {
                sanpham.Quantity--;

                if (sanpham.Quantity <= 0) //nếu số lượng về 0 thì xóa đi
                {
                    list.Remove(sanpham);
                }

            }
            else //không tồn tại sản phẩm
            {
                return -1;
            }
            return 1;
        }
    }
}