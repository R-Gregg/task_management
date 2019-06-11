using System.Web;
using System.Web.Optimization;

namespace SCG
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Scripts -------------------
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-sortable.js",
                      "~/Scripts/bootstrap-tour.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/js_popup.js",
                        "~/Scripts/grendelfly.js"));

            bundles.Add(new ScriptBundle("~/bundles/forms").Include(
                        "~/Scripts/jquery-ui.min.js",
                        "~/Scripts/js_validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/tinymce").Include(
                      "~/Scripts/tinymce/jquery.tinymce.min.js",
                      "~/Scripts/tinymce/tinymce.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/jquery-ui-1.12.1.js",
                        "~/Scripts/jquery.floatThead.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                              "~/Scripts/jquery.validate.js",
                              "~/Scripts/jquery.validate.unobtrusive.js",
                              "~/Scripts/jquery.validate-vsdoc.js",
                              "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                       "~/Scripts/modernizr-2.8.3.js"));



            // Style Sheets ---------------
            bundles.Add(new StyleBundle("~/Layout/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.structure.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/webfonts.css",
                      "~/Content/css-frm-tbl.css",
                      "~/Content/layout.css",
                      "~/Content/site.css",
                      "~/Content/projects.css",
                      "~/Content/it.css",
                      "~/Content/mjl.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-tour.min.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.structure.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/site.css",
                      "~/Content/nav.css",
                      "~/Content/projects.css",
                      "~/Content/it.css",
                      "~/Content/mjl.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
