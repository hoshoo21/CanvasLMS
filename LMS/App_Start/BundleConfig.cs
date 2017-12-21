using System.Web;
using System.Web.Optimization;

namespace LMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            ////bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            ////            "~/Scripts/jquery-{version}.js"));

            ////bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            ////            "~/Scripts/jquery.validate*"));

            ////// Use the development version of Modernizr to develop with and learn from. Then, when you're
            ////// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            ////bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            ////            "~/Scripts/modernizr-*"));

            ////bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            ////          "~/Scripts/bootstrap.js",
            ////          "~/Scripts/respond.js"));

            ////bundles.Add(new StyleBundle("~/Content/css").Include(
            ////          "~/Content/bootstrap.css",
            ////          "~/Content/site.css"));



            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"
                       , "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"
                        , "~/Scripts/jquery.unobtrusive*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                "~/Scripts/DataTables/jquery.dataTables.js"
                , "~/Scripts/DataTables/dataTables.bootstrap.js"
                //, "~/Scripts/DataTables/dataTables.fixedHeader.js"
                , "~/Scripts/DataTables/dataTables.buttons.js"
                , "~/Scripts/DataTables/buttons.flah.js"
                , "~/Scripts/DataTables/buttons.html5.js"
                 , "~/Scripts/DataTables/buttons.print.js"
                , "~/Scripts/DataTables/buttons.colVis.js"
                , "~/Scripts/DataTables/buttons.bootstrap.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/moment.js"
                , "~/Scripts/moment-with-locales.js"
                , "~/Scripts/fastclick.js"
                , "~/Scripts/jquery.mCustomScrollbar.js"
                , "~/Scripts/yukon_all.js"
                , "~/Scripts/countUp.min.js"
                , "~/Scripts/retina.min.js"
                , "~/Scripts/URI.js"
                , "~/Scripts/bootstrap-datepicker.js"
                , "~/Scripts/daterangepicker.js"
                , "~/Scripts/typeahead.jquery.js"
                , "~/Scripts/typeahead.bundle.js"
                , "~/Scripts/d3.js"
                , "~/Scripts/c3.js"
                , "~/Scripts/jquery-jvectormap.js"
                , "~/Scripts/jVectorMaps/world-ISO-A2/jquery-jvectormap-world-mill-en-US.js"
                , "~/Scripts/jquery.matchHeight.js"
                , "~/Scripts/jquery.easypiechart.js"
                , "~/Scripts/jquery-mousewheel.js"
                , "~/Scripts/lou-multi-select/jquery.multi-select.js"
                , "~/Scripts/jquery-chained.js"
                , "~/Scripts/bootstrap-datetimepicker.js"
                , "~/Scripts/sweetalert.min.js"
                , "~/Scripts/site-script.js"
                , "~/scripts/site-common.js"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/application").Include(
            //          "~/Scripts/Application/*.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/*.css"
                      //, "~/Content/themes/base/core.css"
                      , "~/Content/themes/base/jquery-ui.css"
                      , "~/Content/bootstrap-datetimepicker.css"
                      , "~/content/sweetalert.css",
                      "~/content/facebook.css"
                      , "~/content/titatoggle-dist-min.css"
                       //, "~/Content/themes/base/autocomplete.css"
                       //, "~/Content/themes/base/theme.css"
                       ));
            bundles.Add(new StyleBundle("~/Content/matrix/css").Include(
                "~/Content/matrix/css/bootstrap-responsive.min.css"
                , "~/Content/matrix/css/*.css"));

            bundles.Add(new StyleBundle("~/Content/matrix/fontawesome").Include(
                "~/Content/matrix/font-awesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/matrix/css/bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/matrix/js").Include(
                "~/Scripts/matrix/js/*.js"));

            //bundles.Add(new StyleBundle("~/Content/datatablecss").Include(
            //         "~/Content/DataTables/css/datatables.css"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}
