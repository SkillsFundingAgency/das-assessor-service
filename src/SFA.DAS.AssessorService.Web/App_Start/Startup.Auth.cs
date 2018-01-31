using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure;
using Microsoft.IdentityModel.Protocols;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.WsFederation;

namespace SFA.DAS.AssessorService.Web
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string authority = aadInstance + tenantId;

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = SecurityTokenValidated
                    }
                });
        }

        private Task SecurityTokenValidated(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            var identity = notification.AuthenticationTicket.Identity;

            var email = identity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Upn)?.Value;

            return Task.CompletedTask;
        }

        //public void ConfigureAuth(IAppBuilder app)
        //{
        //    app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

        //    app.UseCookieAuthentication(new CookieAuthenticationOptions());

        //    var realm = CloudConfigurationManager.GetSetting("IdamsRealm");
        //    var adfsMetadata = CloudConfigurationManager.GetSetting("IdamsADFSMetadata");

        //    var options = new WsFederationAuthenticationOptions
        //    {
        //        Wtrealm = realm,
        //        MetadataAddress = adfsMetadata,
        //        Notifications = new WsFederationAuthenticationNotifications
        //        {
        //            SecurityTokenValidated = SecurityTokenValidated
        //        }
        //    };

        //    app.UseWsFederationAuthentication(options);
        //}

        //private async Task SecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification)
        //{
        //    var identity = notification.AuthenticationTicket.Identity;

        //    var email = identity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;

        //    // At some point get EPAOid from user's email address and add to claims.
        //}
    }
}
