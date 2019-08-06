using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Data;

namespace Generic.Dapper.Model
{
    public class ProductPriceObj 
    {
        public decimal Price { get; set; }
        public string ProductTypeName { get; set; }
        public string CatId { get; set; }
        public int Id { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ProductTypeId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string ProductDescription { get; set; }
        public string Status { get; set; }
        public byte[] ProductImage { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<bool> Display { get; set; }
        public bool IsPickupFlag { get; set; }

    }

    //public class ProductTypeObj : sf_ProductType
    //{

    //    public string CatId { get; set; }
    //}
}
