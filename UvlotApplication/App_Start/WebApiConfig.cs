using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace UvlotApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            var routeUrl = ConfigurationManager.AppSettings["APIRouteURL"];
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                  routeTemplate: routeUrl + "/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                //routeTemplate: "api/{controller}/{id}",
                //defaults: new { id = RouteParameter.Optional }
            );
        }

        
    }
}
