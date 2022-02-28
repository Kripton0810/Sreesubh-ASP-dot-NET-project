using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SreesubhApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Holiday",
                url: "Holiday/deleteHoliday/{id}"               
            );
            routes.MapRoute(
                name: "LeaveAccept",
                url: "Leave/Accept/{id}"
            );
            routes.MapRoute(
                name: "LeaveDecline",
                url: "Leave/Decline/{id}"
            );
            routes.MapRoute(
                name: "LeaveCancelled",
                url: "Leave/Cancelled/{id}"
            );
            
            routes.MapRoute(
                name: "Attandance",
                url: "Attendance/newAttandance"
                );
        }
    }
}
