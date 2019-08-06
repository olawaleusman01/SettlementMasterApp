using Dapper;
using Generic.Dapper.Model;
using Generic.Dapper.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public class DapperCart : RepoBase, IDapperCart
    {
        public List<sf_CartItemObj> GetCartItem(int? ItemId, string CartId, bool @IsAll = false, string conString = null)
        {
            var p = new DynamicParameters();
            p.Add("@ItemId", ItemId, DbType.Int32);
            p.Add("@CartId", CartId, DbType.String);
            p.Add("@IsAll", @IsAll, DbType.Boolean);


            string sql = @"proc_GetCartItem";

            var rec = Fetch(c => c.Query<sf_CartItemObj>(sql, p, commandType: CommandType.StoredProcedure).ToList(), conString);
            return rec;

        }

        public int PostCartItem(sf_CartItemObj cart, string conString = null)
        {
            var p = new DynamicParameters();
            p.Add("@Id", cart.ItemId, DbType.String);
            p.Add("@ItemId", cart.ItemId, DbType.Int32);
            p.Add("@Quantity", cart.Quantity, DbType.Int32);
            p.Add("@UnitPrice", cart.UnitPrice, DbType.Decimal);
            p.Add("@LineTotal", cart.LineTotal, DbType.Decimal);
            p.Add("@TaxValue", cart.TaxValue, DbType.Decimal);
            p.Add("@CartId", cart.CartId, DbType.String);
            p.Add("@Discount", cart.Discount, DbType.Decimal);
            p.Add("@CreatedDate", cart.CreatedDate, DbType.DateTime);
           // p.Add("@IsPickUp", cart.IsPickUp, DbType.Boolean);

            string sql = @"proc_PostCartItem";

            var rec = Execute(c => c.Execute(sql, p, commandType: CommandType.StoredProcedure), conString);
            return rec;

        }
        public int UpdateCartItem(sf_CartItemObj cart, string conString = null)
        {
            var p = new DynamicParameters();
            //  p.Add("@ItemId", cart.ItemId, DbType.String);
            p.Add("@ItemId", cart.ItemId, DbType.Int32);
            p.Add("@Quantity", cart.Quantity, DbType.Int32);
            p.Add("@UnitPrice", cart.UnitPrice, DbType.Decimal);
            p.Add("@LineTotal", cart.LineTotal, DbType.Decimal);
            //p.Add("@IsPickUp", cart.IsPickUp, DbType.Boolean);
            p.Add("@CartId", cart.CartId, DbType.String);



            string sql = @"proc_UpdateCartItem";

            var rec = Execute(c => c.Execute(sql, p, commandType: CommandType.StoredProcedure), conString);
            return rec;

        }
        public int ReduceCartItem(sf_CartItemObj cart, string conString = null)
        {
            var p = new DynamicParameters();
            //  p.Add("@ItemId", cart.ItemId, DbType.String);
            p.Add("@ItemId", cart.ItemId, DbType.Int32);
            p.Add("@Quantity", cart.Quantity, DbType.Int32);
            //p.Add("@UnitPrice", cart.UnitPrice, DbType.Decimal);
            p.Add("@LineTotal", cart.LineTotal, DbType.Decimal);

            p.Add("@CartId", cart.CartId, DbType.String);



            string sql = @"proc_ReduceCartItem";

            var rec = Execute(c => c.Execute(sql, p, commandType: CommandType.StoredProcedure), conString);
            return rec;

        }
        public int RemoveCartItem(string removeCartId, int productId, string conString = null)
        {
            var p = new DynamicParameters();
            //  p.Add("@ItemId", cart.ItemId, DbType.String);
            p.Add("@ProductId", productId, DbType.Int32);

            p.Add("@CartId", removeCartId, DbType.String);

            string sql = @"proc_RemoveCartItem";

            var rec = Execute(c => c.Execute(sql, p, commandType: CommandType.StoredProcedure), conString);
            return rec;

        }

        public int EmptyCartItem(string CartId, string conString = null)
        {
            var p = new DynamicParameters();
            //  p.Add("@ItemId", cart.ItemId, DbType.String);
            //  p.Add("@ProductId", productId, DbType.Int32);

            p.Add("@CartId", CartId, DbType.String);

            string sql = @"proc_EmptyCartItem";

            var rec = Execute(c => c.Execute(sql, p, commandType: CommandType.StoredProcedure), conString);
            return rec;

        }

        public int MigrateCartItem(string CartId, string UserName, string conString = null)
        {
            var p = new DynamicParameters();
            p.Add("@CartId", CartId, DbType.String);
            p.Add("@UserName", UserName, DbType.String);

            string sql = @"proc_MigrateCartItem";

            var rec = Execute(c => c.Execute(sql, p, commandType: CommandType.StoredProcedure), conString);
            return rec;

        }
        public List<ProductPriceObj> GetProductPrice(int productId, bool isAll, string conString = null)
        {
            var p = new DynamicParameters();
            p.Add("@ProductId", productId, DbType.Int32);
            p.Add("@IsAll", isAll, DbType.Boolean);
            //    p.Add("@Status", cust.Status, DbType.String);


            string sql = @"proc_GetProductPrice";

            var rec = Fetch(c => c.Query<ProductPriceObj>(sql, p, commandType: CommandType.StoredProcedure), conString);
            return rec.ToList();

        }

    }
}
