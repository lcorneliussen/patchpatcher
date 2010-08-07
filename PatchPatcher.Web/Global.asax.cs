using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PatchPatcher.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Github-Url", // Route name
                "github.com/{*path}", // URL with parameters
                new { controller = "ConvertPatch", action = "AnalyzePermaLink" } // Parameter defaults
                );

            routes.MapRoute(
               "Download Patch", // Route name
               "download/github.com/{*path}", // URL with parameters
               new { controller = "ConvertPatch", action = "DownloadPatch" } // Parameter defaults
               );

            routes.MapRoute(
              "View Patch", // Route name
              "view/github.com/{*path}", // URL with parameters
              new { controller = "ConvertPatch", action = "ViewPatch" } // Parameter defaults
              );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new { controller = "Home", action = "Index" } // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}