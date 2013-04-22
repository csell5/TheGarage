using Microsoft.AspNet.SignalR;
using System.Web.Routing;

namespace SignalR.Sample
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Register the default hubs route: ~/signalr/hubs
            routes.MapHubs();
        }
    }
}