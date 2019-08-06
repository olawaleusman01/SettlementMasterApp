using Generic.Data.Utilities;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Models
{
    public static class SmartUtil
    {
        public static int GetMenuId(string m)
        {
            int menuId = 0;
            if (!string.IsNullOrEmpty(m))
            {
                var decrypt = SmartObj.Decrypt(m);
                // var m = HttpContext.Current.Request.QueryString["m"];
                if (int.TryParse(decrypt, out menuId))
                {
                    //var urlHelper = new UrlHelper();
                    // urlHelper.Action("Login", "Account");
                    return menuId;
                }
            }
            return 0;
        }
    }
}