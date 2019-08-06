using Generic.Dapper.Model;
using Generic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Generic.Dapper.Data
{
    public class ShoppingCartActions 
    {
        private readonly IDapperCart _repo = new DapperCart();
        public string ShoppingCartId { get; set; }
        
        public const string CartSessionKey = "CartId";

        public void AddToCart(sf_CartItemObj item)
        {
            // Retrieve the product from the database.           
            ShoppingCartId = GetCartId();

            //var cartItem = _db.ShoppingCartItems.SingleOrDefault(
            //    c => c.CartId == ShoppingCartId
            //    && c.ProductId == id);
            var cartItem = _repo.GetCartItem(item.ItemId, ShoppingCartId).FirstOrDefault();
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists.                 
                cartItem = new sf_CartItemObj
                {
                    Id = Guid.NewGuid().ToString(),
                    ItemId = item.ItemId,
                    CartId = ShoppingCartId,
                    LineTotal = item.LineTotal,
                    UnitPrice = item.UnitPrice,
                    
                    Quantity = 1,
                    CreatedDate = DateTime.Now
                };

                // _db.ShoppingCartItems.Add(cartItem);
                _repo.PostCartItem(cartItem);
            }
            else
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Quantity++;
                _repo.UpdateCartItem(cartItem);
            }
            //  _db.SaveChanges();
          
        }
        //public void AddAddressToCart(sf_CartAddress add)
        //{
        //    // Retrieve the address from the database.           
        //    ShoppingCartId = GetCartId();

           
        //    var cartAddress = _repo.GetCartAddress(ShoppingCartId);
        //    if (cartAddress == null)
        //    {
        //        // Create a new cart address if no cart item exists.                 
        //        cartAddress = new sf_CartAddress
        //        {
        //            ItemId = Guid.NewGuid().ToString(),
        //            Address = add.Address,
        //            CartId = ShoppingCartId,
        //            CityCode = add.CityCode,
        //            StateCode = add.StateCode,

        //           CountryCode = add.CountryCode,
        //            CreatedDate = DateTime.Now
        //        };

        //        // _db.ShoppingCartItems.Add(cartItem);
        //        _repo.PostUpdateCartAddress(cartAddress);
        //    }
        //    else
        //    {
        //        // If the item does exist in the cart,                  
        //        // then add one to the quantity.                 
        //        //  cartItem.Quantity++;
        //        _repo.PostUpdateCartAddress(cartAddress);
        //    }
        //    //  _db.SaveChanges();

        //}
        //public void AddDateTimeToCart(sf_CartAddress add)
        //{
        //    // Retrieve the address from the database.           
        //    ShoppingCartId = GetCartId();


        //    var cartAddress = _repo.GetCartAddress(ShoppingCartId);
        //    if (cartAddress != null)
        //    {
        //        // Create a new cart address if no cart item exists.                 
        //        cartAddress = new sf_CartAddress
        //        {
        //           DropOffDateTime = add.DropOffDateTime,
        //           PickupDateTime = add.PickupDateTime,
        //            CartId = ShoppingCartId,
        //            DropOffType = add.DropOffType,
        //            PickUpType = add.PickUpType
                   
        //        };

        //        // _db.ShoppingCartItems.Add(cartItem);
        //        _repo.PostCartTimes(cartAddress);
        //    }
        //    else
        //    {
        //        return;
        //    }
        //    //  _db.SaveChanges();

        //}
        //public int AddOrderOnlinePayment(sf_OnlineOrderPaymentLog log)
        //{
        //    // Retrieve the address from the database.           
        //   // ShoppingCartId = GetCartId();


        //    //var cartAddress = _repo.GetCartAddress(ShoppingCartId);
        //    //if (cartAddress != null)
        //    //{
        //        // Create a new cart address if no cart item exists.                 
        //       //var payLog = new sf_OnlineOrderPaymentLog
        //       // {
        //       //     DropOffDateTime = add.DropOffDateTime,
        //       //     PickupDateTime = add.PickupDateTime,
        //       //     CartId = ShoppingCartId,

        //       // };

        //        // _db.ShoppingCartItems.Add(cartItem);
        //     return   _repo.PostOrderPaymentLog(false,log);
        //    //}
        //    //else
        //    //{
        //    //    return;
        //    //}
        //    //  _db.SaveChanges();

        //}
        //public void Dispose()
        //{
        //    if (_db != null)
        //    {
        //        _db.Dispose();
        //        _db = null;
        //    }
        //}

        public string GetCartId()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.     
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<sf_CartItemObj> GetCartItems()
        {
            ShoppingCartId = GetCartId();

            return _repo.GetCartItem(null, ShoppingCartId, true);
            //_db.ShoppingCartItems.Where(
            //    c => c.CartId == ShoppingCartId).ToList();
        }
        //public sf_CartAddress GetCartAddress()
        //{
        //    ShoppingCartId = GetCartId();

        //    return _repo.GetCartAddress(ShoppingCartId);
        //    //_db.ShoppingCartItems.Where(
        //    //    c => c.CartId == ShoppingCartId).ToList();
        //}
        public decimal GetTotal()
        {
            ShoppingCartId = GetCartId();
            // Multiply product price by quantity of that product to get        
            // the current price for each of those products in the cart.  
            // Sum all product price totals to get the cart total.   
            decimal? total = decimal.Zero;
            //total = (decimal?)(from cartItems in _db.ShoppingCartItems
            //                   where cartItems.CartId == ShoppingCartId
            //                   select (int?)cartItems.Quantity *
            //                   cartItems.Product.UnitPrice).Sum();
            total = _repo.GetCartItem(null, ShoppingCartId, true).Sum(e => e.Quantity * e.UnitPrice);
            return total ?? decimal.Zero;
        }

        //public ShoppingCartActions GetCart(HttpContext context)
        //{
        //    //using (var cart = new ShoppingCartActions())
        //    //{
        //    //    cart.ShoppingCartId = cart.GetCartId();
        //    //    return cart;
        //    //}
        //}

        //public void UpdateShoppingCartDatabase(String cartId, ShoppingCartUpdates[] CartItemUpdates)
        //{
        //    using (var db = new WingtipToys.Models.ProductContext())
        //    {
        //        try
        //        {
        //            int CartItemCount = CartItemUpdates.Count();
        //            List<CartItem> myCart = GetCartItems();
        //            foreach (var cartItem in myCart)
        //            {
        //                // Iterate through all rows within shopping cart list
        //                for (int i = 0; i < CartItemCount; i++)
        //                {
        //                    if (cartItem.Product.ProductID == CartItemUpdates[i].ProductId)
        //                    {
        //                        if (CartItemUpdates[i].PurchaseQuantity < 1 || CartItemUpdates[i].RemoveItem == true)
        //                        {
        //                            RemoveItem(cartId, cartItem.ProductId);
        //                        }
        //                        else
        //                        {
        //                            UpdateItem(cartId, cartItem.ProductId, CartItemUpdates[i].PurchaseQuantity);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception exp)
        //        {
        //            throw new Exception("ERROR: Unable to Update Cart Database - " + exp.Message.ToString(), exp);
        //        }
        //    }
        //}

        public void RemoveItem( int removeProductID)
        {
            //using (var _db = new WingtipToys.Models.ProductContext())
            //{
                try
                {
                ShoppingCartId = GetCartId();
                //var myItem = (from c in _db.ShoppingCartItems where c.CartId == removeCartID && c.Product.ProductID == removeProductID select c).FirstOrDefault();
                //if (myItem != null)
                //{
                //    // Remove Item.
                //    _db.ShoppingCartItems.Remove(myItem);
                //    _db.SaveChanges();
                //}
                var rst = _repo.RemoveCartItem(ShoppingCartId, removeProductID);
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Remove Cart Item - " + exp.Message.ToString(), exp);
                }
          //  }
        }

        public void UpdateItem(sf_CartItemObj cart)
        {
            //using (var _db = new WingtipToys.Models.ProductContext())
            //{
            try
            {
                ShoppingCartId = GetCartId();
                cart.CartId = ShoppingCartId;
                //var myItem = (from c in _db.ShoppingCartItems where c.CartId == updateCartID && c.Product.ProductID == updateProductID select c).FirstOrDefault();
                //if (myItem != null)
                //{
                //    myItem.Quantity = quantity;
                //    _db.SaveChanges();
                //}
                var rst = _repo.UpdateCartItem(cart);
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
            }
            // }
        }
        public void ReduceItem(sf_CartItemObj cart)
        {
            //using (var _db = new WingtipToys.Models.ProductContext())
            //{
            try
            {
                ShoppingCartId = GetCartId();
                cart.CartId = ShoppingCartId;
                //var myItem = (from c in _db.ShoppingCartItems where c.CartId == updateCartID && c.Product.ProductID == updateProductID select c).FirstOrDefault();
                //if (myItem != null)
                //{
                //    myItem.Quantity = quantity;
                //    _db.SaveChanges();
                //}
                var rst = _repo.ReduceCartItem(cart);
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
            }
            // }
        }

        public void EmptyCart()
        {
            ShoppingCartId = GetCartId();
            //var cartItems = _db.ShoppingCartItems.Where(
            //    c => c.CartId == ShoppingCartId);
            //foreach (var cartItem in cartItems)
            //{
            //    _db.ShoppingCartItems.Remove(cartItem);
            //}
            //// Save changes.             
            //_db.SaveChanges();
            var rst = _repo.EmptyCartItem(ShoppingCartId);
        }

        public int GetCount()
        {
            ShoppingCartId = GetCartId();

            // Get the count of each item in the cart and sum them up          
            //int? count = (from cartItems in _db.ShoppingCartItems
            //              where cartItems.CartId == ShoppingCartId
            //              select (int?)cartItems.Quantity).Sum();
            // Return 0 if all entries are null         
            return _repo.GetCartItem(null,ShoppingCartId,true).Count();
        }

        public struct ShoppingCartUpdates
        {
            public int ProductId;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }

        public void MigrateCart(string cartId, string userName)
        {
            //var shoppingCart = _db.ShoppingCartItems.Where(c => c.CartId == cartId);
            //foreach (CartItem item in shoppingCart)
            //{
            //    item.CartId = userName;
            //}
            var rst = _repo.MigrateCartItem(cartId, userName);
            HttpContext.Current.Session[CartSessionKey] = userName;
            //_db.SaveChanges();
        }
    }
}
