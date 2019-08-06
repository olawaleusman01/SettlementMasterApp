using Generic.Dapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public interface IDapperCart
    {
         List<sf_CartItemObj> GetCartItem(int? ProductId, string CartId, bool @IsAll = false, string conString = null);

         int PostCartItem(sf_CartItemObj cart, string conString = null);
         int UpdateCartItem(sf_CartItemObj cart, string conString = null);
         int ReduceCartItem(sf_CartItemObj cart, string conString = null);
         int RemoveCartItem(string removeCartId, int productId, string conString = null);

         int EmptyCartItem(string CartId, string conString = null);

         int MigrateCartItem(string CartId, string UserName, string conString = null);
        List<ProductPriceObj> GetProductPrice(int productId, bool isAll, string conString = null);
    }
}
