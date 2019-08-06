using Generic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public class sf_CartItemObj 
    {
        public string ProductName { get; set; }
        public string Id { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> LineTotal { get; set; }
        public Nullable<decimal> TaxValue { get; set; }
        public Nullable<decimal> ManualUnitPriceOverride { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string MerchantNote { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CartId { get; set; }
        public Nullable<int> StarchLevel { get; set; }
        public bool IsPickUp { get; set; }
    }
}
