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
            //BackgroundThread.Start();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            BackgroundThread.Start();
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