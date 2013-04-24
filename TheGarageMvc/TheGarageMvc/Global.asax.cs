using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.IdentityModel.Web;
using TheGarageMvc.App_Start;

namespace TheGarageMvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SignalRConfig.ConfigureSignalR(GlobalHost.DependencyResolver, GlobalHost.HubPipeline);

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();


            FederatedAuthentication.ServiceConfigurationCreated += (s, e) =>
            {
                FederatedAuthentication.SessionAuthenticationModule.SessionSecurityTokenCreated +=
                    SessionAuthenticationModule_SessionSecurityTokenCreated;
                FederatedAuthentication.SessionAuthenticationModule.SessionSecurityTokenReceived +=
                    SessionAuthenticationModule_SessionSecurityTokenReceived;
                FederatedAuthentication.WSFederationAuthenticationModule.SessionSecurityTokenCreated +=
                    WSFederationAuthenticationModule_SessionSecurityTokenCreated;
                FederatedAuthentication.WSFederationAuthenticationModule.SecurityTokenValidated +=
                    WSFederationAuthenticationModule_SecurityTokenValidated;
                FederatedAuthentication.WSFederationAuthenticationModule.SecurityTokenReceived +=
                    WSFederationAuthenticationModule_SecurityTokenReceived;
                FederatedAuthentication.WSFederationAuthenticationModule.SignedIn +=
                    WSFederationAuthenticationModule_SignedIn;
            };
        }


        void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
        {
            //Debugger.Break();
        }

        void SessionAuthenticationModule_SessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e)
        {
            //Debugger.Break();
        }

        void WSFederationAuthenticationModule_SessionSecurityTokenCreated(object sender, SessionSecurityTokenCreatedEventArgs e)
        {
            //Debugger.Break();
        }

        void WSFederationAuthenticationModule_SecurityTokenValidated(object sender, SecurityTokenValidatedEventArgs e)
        {
            //Debugger.Break();
        }

        void WSFederationAuthenticationModule_SecurityTokenReceived(object sender, SecurityTokenReceivedEventArgs e)
        {
            //Debugger.Break();
        }

        void WSFederationAuthenticationModule_SignedIn(object sender, EventArgs e)
        {
            //Debugger.Break();
        }

    }
}