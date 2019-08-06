using System.Web;
using System.Web.Optimization;

namespace SettlementMaster.App
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/PageJs").Include(
                "~/Content/bower_components/jquery/dist/jquery.js",
                "~/Content/bower_components/bootstrap/dist/js/bootstrap.js",
                 "~/Content/bower_components/metisMenu/dist/metisMenu.min.js",
                 "~/Scripts/jquery.validate.js",
                    "~/ui/jquery.ui.js",
                     "~/Scripts/datepicker/bootstrap-datepicker.js",
                      "~/Scripts/jquery.bootstrap.wizard.min.js",
                    "~/Scripts/prettify.js",
                    "~/source/jquery.fancybox.js",
                  "~/Scripts/jquery.PrintArea.js",
                  "~/Scripts/select2.min.js",
                  "~/Scripts/tooltip.js",
                  "~/Scripts/popover.js",
                  "~/Scripts/app/ajaxhelper.js",
                   "~/Scripts/app/smart_obj.js",
                      "~/Scripts/app/utility.js",
                     "~/TableTool/jquery.dataTables.js",
                "~/Content/dist/js/sb-admin-2.js"));
            bundles.Add(new ScriptBundle("~/bundles/users").Include(
              "~/Scripts/app/users.js"
           ));
            bundles.Add(new ScriptBundle("~/bundles/roles").Include(
            "~/Scripts/app/roles.js"
         ));
            bundles.Add(new ScriptBundle("~/bundles/role_priviledge").Include(
              "~/Scripts/app/role_priviledge.js"
           ));
            bundles.Add(new ScriptBundle("~/bundles/itemtype").Include(
            "~/Scripts/app/itemtype.js"
         ));
            bundles.Add(new ScriptBundle("~/bundles/unit").Include(
            "~/Scripts/app/unit.js"
         ));
            bundles.Add(new ScriptBundle("~/bundles/location").Include(
          "~/Scripts/app/location.js"
       ));
            bundles.Add(new ScriptBundle("~/bundles/reports").Include(
          "~/Scripts/app/reports.js"
       ));
            bundles.Add(new ScriptBundle("~/bundles/item").Include(
       "~/Scripts/app/item.js"
    ));
            bundles.Add(new ScriptBundle("~/bundles/supplier").Include(
 "~/Scripts/app/supplier.js"
));
            bundles.Add(new ScriptBundle("~/bundles/purchase").Include(
"~/Scripts/app/purchase_order.js"
));
            bundles.Add(new ScriptBundle("~/bundles/sales_order").Include(
"~/Scripts/app/sales.js"

));
            bundles.Add(new ScriptBundle("~/bundles/customer").Include(
"~/Scripts/app/customer.js"
));
            bundles.Add(new ScriptBundle("~/bundles/company").Include(
"~/Scripts/app/company_profile.js"
));
            bundles.Add(new ScriptBundle("~/bundles/dashboard").Include(
"~/Scripts/app/dashboard.js",
"~/Content/bower_components/raphael/raphael-min.js",
"~/Content/bower_components/morrisjs/morris.min.js",
"~/Content/js/morris-data.js"
));


            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                 "~/Content/bower_components/jquery/dist/jquery.js",
                "~/Content/bower_components/bootstrap/dist/js/bootstrap.js",
                  "~/Content/bower_components/metisMenu/dist/metisMenu.min.js",
                   "~/Scripts/jquery.validate.js",
                     "~/Scripts/datepicker/bootstrap-datepicker.js",
                       "~/Scripts/select2.min.js",
                     "~/Scripts/app/ajaxhelper.js",
                      "~/Content/dist/js/sb-admin-2.js",
             "~/Scripts/app/login.js"
          ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bower_components/bootstrap/dist/css/bootstrap.css",
                      "~/Content/bower_components/metisMenu/dist/metisMenu.min.css",
                       "~/Content/dist/css/sb-admin-2.css",
                           "~/TableTool/DataTabley.css",
                                  "~/ui/jquery.ui.css",
                                   "~/Scripts/datepicker/datepicker.css",
                                     "~/source/jquery.fancybox.css",
                                     "~/Content/prettify.css",
                                        "~/Content/bootstrap-theme.min.css",
                                   "~/Content/bower_components/morrisjs/morris.css",
                        "~/Content/bower_components/font-awesome/css/font-awesome.min.css",
                         "~/Content/select2.min.css", //C:\Application\vs2016\new\Inventory\Inventory.App\Content\select2.min.css
                        "~/Content/dist/css/custom.css"));
            bundles.Add(new StyleBundle("~/Content/login").Include(
                 // "~/Content/bower_components/bootstrap/dist/css/bootstrap.css",
                 "~/Content/style.css",
                    "~/ Content/reset.css",
                      "~/Content/bower_components/font-awesome/css/font-awesome.min.css"
                     ));
        }
    }
}
