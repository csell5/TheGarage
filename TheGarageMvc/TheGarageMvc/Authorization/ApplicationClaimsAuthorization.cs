using System.Diagnostics;
using System.Security.Claims;
using TheGarageMvc.Models;

namespace TheGarageMvc.Authorization
{
    public class ApplicationClaimsAuthorization : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var authorized = false;

            var authorizationClaim = context.Principal.FindFirst(c => c.Type == ClaimTypes.AuthorizationDecision);

            if (authorizationClaim != null)
            {
                var parseResult = bool.TryParse(authorizationClaim.Value, out authorized);

                if (parseResult == false)
                {
                    authorized = false;
                }
            }
            else
            {
                if (context.Resource.Count > 0)
                {
                    // if a name claim is being presented
                    if (context.Resource[0].Type == ClaimTypes.Name)
                    {
                        // and it is a URL with multiple segments
                        if (System.Web.HttpContext.Current.Request.Url.Segments.Length > 1)
                        {
                            var segments = System.Web.HttpContext.Current.Request.Url.Segments;

                            foreach (var segment in segments)
                            {
                                Debug.Write(segment + " - ");
                            }
                            Debug.WriteLine("\n");

                            // and a chat session is being opened
                            // OK, this is a hack but we need to make it work for now
                            if ((segments.Length > 2) && ((segments[1].ToLower().StartsWith("api")) &&
                                                          (segments[2].ToLower().StartsWith("pushnotifications"))))
                            {
                                // Let Push Notifications work with it
                                return true;
                            }

                            if ((segments.Length > 1) && (segments[1].ToLower().StartsWith("signalr")))
                            {
                                // Let SignalR work with it
                                return true;
                            }
                        }
                    }
                }
            }

            return authorized;
        }
    }


    /// <summary>
    /// This allows the transformation of claims as they are presented from the ID Provider
    /// It executes before the Claims Authorization process
    /// </summary>
    public class ApplicationClaimsTransformation : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            var authorize = false;

            if (incomingPrincipal != null && incomingPrincipal.Identity.IsAuthenticated)
            {
                using (var users = new theGarageEntities())
                {
                    const string identityClaim = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";

                    var identityProviderClaim = incomingPrincipal.FindFirst(c => c.Type == identityClaim);
                    var nameClaim = incomingPrincipal.FindFirst(c => c.Type == ClaimTypes.Name);
                    var emailClaim = incomingPrincipal.FindFirst(c => c.Type == ClaimTypes.Email);
                    var nameIdClaim = incomingPrincipal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);

                    string email = null;

                    var identityProvider = identityProviderClaim.Value;

                    if (emailClaim != null)
                    {
                        email = emailClaim.Value;
                    }

                    var nameId = nameIdClaim.Value;

                    if (((identityProvider.ToLower() == "google") || (identityProvider.ToLower().StartsWith("facebook"))) && (email != null))
                    {
                        var user = users.RegisteredUsers.Find(email.ToLower());

                        if ((user != null) && (user.Active))
                        {
                            authorize = true;
                        }
                    }
                    else
                    {
                        var user = users.RegisteredUsers.Find(nameId);

                        if ((user != null) && (user.Active))
                        {
                            if (
                                ((ClaimsIdentity) incomingPrincipal.Identity).HasClaim(
                                    c => c.Type == ClaimTypes.Name))
                            {
                                if (nameClaim != null)
                                {
                                    ((ClaimsIdentity) incomingPrincipal.Identity).RemoveClaim(nameClaim);
                                }

                            }

                            ((ClaimsIdentity) incomingPrincipal.Identity).AddClaim(new Claim(ClaimTypes.Name,
                                                                                             user.Name));

                            authorize = true;
                        }
                    }
                }

                ((ClaimsIdentity)incomingPrincipal.Identity).AddClaim(new Claim(ClaimTypes.AuthorizationDecision, authorize.ToString()));
            }

            return incomingPrincipal;
        }

    }
}