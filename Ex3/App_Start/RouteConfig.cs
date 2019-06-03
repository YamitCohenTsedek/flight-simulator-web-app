using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ex3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
          
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
              name: "display",
              url: "display/{ip}/{port}",
              defaults: new { controller = "Display", Action = "DisplayLocation" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "displayLine",
                url: "display/{ip}/{port}/{time}",
                defaults: new { controller = "Display", action = "DisplayLine" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Save",
                url: "save/{ip}/{port}/{time}/{seconds}/{file}",
                defaults: new { controller = "Display", action = "Save" }
            );



            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Display", action = "Index", id = UrlParameter.Optional });
        }

    }
}
