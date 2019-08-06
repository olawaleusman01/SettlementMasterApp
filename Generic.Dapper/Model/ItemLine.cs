using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DahsyApp.Data
{


    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();
        public void AddItem(ItemsObj item, int quantity)
        {
            CartLine line = lineCollection
            .Where(p => p.Item.itemId == item.itemId)
            .FirstOrDefault();
            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Item = item,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }
        public void RemoveLine(int itemId)
        {
            var count = lineCollection.Where(f => f.Item.itemId == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.Item.itemId == itemId);
            }
        }

        public void UpdateItem(int itemId, int quantity)
        {
            CartLine line = lineCollection
            .Where(p => p.Item.itemId == itemId)
            .FirstOrDefault();
            if (line != null)
            {
                line.Quantity -= quantity;
            }
           
        }
        public decimal ComputeTotalValue()
        {
            return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        }
        public void Clear()
        {
            lineCollection.Clear();
        }
        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
        }
    }
    public class CartLine
    {
        public ItemsObj Item { get; set; }
        public int Quantity { get; set; }
    }


}
