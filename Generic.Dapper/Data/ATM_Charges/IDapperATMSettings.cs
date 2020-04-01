using Generic.Dapper.Model;
using Generic.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public interface IDapperATMSettings
    {
        List<DropdownObj> GetATMPartyType();
        List<ATMChargesObj> GetATMCharges_Temp(string batchId,string userId);
        Task<List<ATMChargesObj>> GetATMChargesAsync();
        Task<List<MenuPrivObj>> GetMenuPrivilegeAsync();
        List<MenuPrivObj> GetMenuPrivilegeTemp(string batchId, string userId);
        List<ParentMenu> GetParentMenu(int roleId, string app_key);
        List<ChildMenu> GetChildMenu(int roleId, string app_key);
    }
}
