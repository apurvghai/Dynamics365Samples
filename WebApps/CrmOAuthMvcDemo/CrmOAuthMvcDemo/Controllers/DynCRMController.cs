using CrmOAuthMvcDemo.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CrmOAuthMvcDemo.Controllers
{
    public class DynCRMController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        DynCrmModel modelCtx = new DynCrmModel();

        private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private string graphResourceID = "https://graph.windows.net";
        private string crmResourceId = ConfigurationManager.AppSettings["ida:CrmResourceUrl"];


        // GET: DynCRM
        public async Task<ActionResult> Index()
        {
            await ReadCrmLoggedUserInfo();
            return View(modelCtx);
        }

        private async Task ReadCrmLoggedUserInfo()
        {
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                OrganizationWebProxyClient proxyClient = new OrganizationWebProxyClient(new Uri("https://<orgname>.crm.dynamics.com/xrmservices/2011/organization.svc/web"), false);
                proxyClient.SdkClientVersion = "8.0";
                proxyClient.HeaderToken = await GetTokenForApplication();

                WhoAmIRequest request = new WhoAmIRequest();
                WhoAmIResponse response = (WhoAmIResponse)proxyClient.Execute(request);
                
                modelCtx.LoggedInUserId = response.UserId;

                Entity systemEntity = new Entity("systemuser");
                systemEntity.Id = response.UserId;

                string[] colParam = { "firstname", "lastname", "fullname"};
                systemEntity = proxyClient.Retrieve("systemuser", response.UserId, new ColumnSet(colParam));
                modelCtx.LoggedInUserName = systemEntity.GetAttributeValue<string>("firstname");

            }
            catch (Exception ex)
            {

            }

        }


        public async Task<string> GetTokenForApplication()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(Microsoft.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
            AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance + tenantID, new ADALTokenCache(signedInUserID));
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(crmResourceId, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}