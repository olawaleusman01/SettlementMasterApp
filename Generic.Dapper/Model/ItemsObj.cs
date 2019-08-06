using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public class ItemsObj
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public bool IsPickUp { get; set; }
        public int Category { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get { return Quantity * Price; } }
        public int totalCartQty { get; set; }
        public decimal totalCart { get; set; }
        
        //public decimal itemQty { get; set; }
    }
}
public class ItemsObj3
{
    public int ItemId { get; set; } = 0;
    public string ItemName { get; set; }
    public decimal Price { get; set; } = 0;
    public int Quantity { get; set; } = 0;
    public decimal LineTotal { get { return Quantity * Price; } }

    //public decimal itemQty { get; set; }
}
