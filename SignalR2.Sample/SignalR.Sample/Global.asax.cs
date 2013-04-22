using Microsoft.AspNet.SignalR;
using System;
using System.Web.Http;
using System.Web.Routing;
using theGarage;

namespace SignalR.Sample
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            SignalRConfig.ConfigureSignalR(GlobalHost.DependencyResolver, GlobalHost.HubPipeline);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
<<<<<<< HEAD
            //BackgroundThread.Start();
=======

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            BackgroundThread.Start();
>>>>>>> d4c3840cc1d0d1447f37f443c6e9c6d3b392267b
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}