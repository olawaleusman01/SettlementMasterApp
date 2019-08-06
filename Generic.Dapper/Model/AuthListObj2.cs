using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Data;
using System.Web;
using Generic.Data.Utilities;

namespace Generic.Dapper.Model
{
    public class AuthListObj2 : SM_AUTHLIST
    {
        public string MenuName { get; set; }
        public string Institution_Name { get; set; }
        public string CREATED_BY { get; set; }
        public string DATESTRING { get { return string.Concat( CREATEDATE.GetValueOrDefault().ToString("dd-MM-yyyy "), CREATEDATE.GetValueOrDefault().ToShortTimeString()); } }
        public string CONTROLLER { get; set; }
        public string ReqUrl { get { return EncryptQueryString(MENUID.GetValueOrDefault().ToString(), ITBID.ToString(), POSTTYPE, CONTROLLER, ""); } }

        public string EncryptQueryString(string m, string ai, string pt, string url, string retUrl)
        {
           // url =string.Format("/{0}/DetailAuth",url);
            var menuId = HttpUtility.UrlEncode(SmartObj.Encrypt(m));
            var a_i = HttpUtility.UrlEncode(SmartObj.Encrypt(ai));
           // var postType = HttpUtility.UrlEncode(SmartObj.Encrypt(pt));
           // var rId = HttpUtility.UrlEncode(SmartObj.Encrypt(r));
            var ret = string.Format("{0}/DetailAuth?m={1}&a_i={2}", url, menuId, a_i); // Eval("URL") + "?m=" + HttpUtility.UrlEncode(Eval("MENUID").ToString()) + "&r=" +HttpUtility.UrlEncode(Eval("RECORDID").ToString()) + "&a_i=" + HttpUtility.UrlEncode(Eval("ITBID").ToString())+ "&pt=" + HttpUtility.UrlEncode(Eval("POSTTYPE").ToString()) + "&ReturnUrl=" + Request.RawUrl %>' runat = "server" > View Detail </ asp:HyperLink >
            return ret;
            // string technology = HttpUtility.UrlEncode(SmartObj.Encrypt(ddlTechnology.SelectedItem.Value));
            // Response.Redirect(string.Format("~/CS2.aspx?name={0}&technology={1}", name, technology));
        }
    }
   
    public class AuthListCountObj 
    {
        public int RECORD_COUNT { get; set; }
    }
    public class RejectedUser : SM_AUTHCHECKER
    {
        public string FULLNAME { get; set; }
      
    }
}
