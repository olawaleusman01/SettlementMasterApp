using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Dapper.Model;
using Generic.Data.Model;

namespace Generic.Dapper.Utility
{
    public class Institution
    {
        private List<InstitutionBinUpldObj> lineCollection = new List<InstitutionBinUpldObj>();
        public void AddItem(InstitutionBinUpldObj item)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.Add(item);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void Clear()
        {
            lineCollection.Clear();
        }
        public IEnumerable<InstitutionBinUpldObj> Lines
        {
            get { return lineCollection; }
        }

    }


      public  class InstitutionLine
    {
        public InstitutionBinUpldObj Item { get; set; }
    }
}
