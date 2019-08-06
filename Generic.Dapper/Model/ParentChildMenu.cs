using Generic.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Generic.Dapper.Model
{
    public class ParentMenu
    {
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string Url { get; set; }
        public string ResourceKey { get; set; }
        public string Active { get; set; }
      
    }

    public class ChildMenu : ParentMenu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string HasNode { get; set; }
        public int Priority { get; set; }
        public string MenuIdEncrypt { get { return HttpUtility.UrlEncode(SmartObj.Encrypt(MenuId.ToString())); } }
    }

    public class MenuViewModel 
    {
        public List<ParentMenu> ParNode { get; set; }
        public List<ChildMenu> ChildNode { get; set; }
    }
}
