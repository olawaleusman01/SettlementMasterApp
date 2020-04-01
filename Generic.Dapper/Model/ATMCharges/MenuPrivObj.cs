using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public class MenuPrivObj
    {
            public int MenuId { get; set; }
           // public int RoleAssigId { get; set; }
            public string MenuName { get; set; }
            public string App { get; set; }
            public string USERID { get; set; }
            public string CREATEDBY { get; set; }

        
    }

    public class MenuPrivObj2
    {
        public List<MenuPrivObj> MenuPrivList { get; set; }
    }
}
