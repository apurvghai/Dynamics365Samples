using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Windows.Security.Authentication.Web;

namespace CRMhelpers
{
    public static class CrmOauthConnector
    {
        private static AuthenticationContext _authenticationContext;

        // TODO Set these string values as approppriate for your app registration and organization.
        // For more information, see the SDK topic "Walkthrough: Register an app with Active Directory".
        private const string _clientID = "C8E3F34A-E1A6-455C-A6DB-A1B82E6A6BE0";
        public const string OauthEndpointUrl = "https://sts.agcrm13.com:81/adfs/ls/";
        public const string CrmServiceUrl = "https://duprepro.agcrm13.com:81/";
        public static bool IsResultTrue { get; set; }
        public static string ReturningUrlToShow { get; set; }
        public static string ErrorMessage { get; set; }

        public static async Task<string> Initialize()
        {

            Uri serviceUrl = new System.Uri(CrmServiceUrl + "XRMServices/2011/Organization.svc/web?SdkClientVersion=6.1.0000.0000");

            // Dyamics CRM Online OAuth URL.
            string _oauthUrl = DiscoveryAuthority(serviceUrl);

            ReturningUrlToShow = _oauthUrl;

            // Obtain the redirect URL for the app. This is only needed for app registration.
            string redirectUrl = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();

            // Obtain an authentication token to access the web service. 
            _authenticationContext = new AuthenticationContext(_oauthUrl, false);
            if (_authenticationContext.TokenCache.Count > 0)
                _authenticationContext.TokenCache.Clear();
            AuthenticationResult result = await _authenticationContext.AcquireTokenAsync(CrmServiceUrl, _clientID, new Uri(redirectUrl));

            // Verify that an access token was successfully acquired.
            if (AuthenticationStatus.Success != result.Status)
            {
                IsResultTrue = true;
                if (result.Error == "authentication_failed")
                {
                    // Clear the token cache and try again.
                    //(AuthenticationContext.TokenCache as DefaultTokenCache).Clear();
                    _authenticationContext = new AuthenticationContext(_oauthUrl, false);
                    result = await _authenticationContext.AcquireTokenAsync(CrmServiceUrl, _clientID, new Uri(redirectUrl));
                }
                else
                {
                    //DisplayErrorWhenAcquireTokenFails(result);
                }
            }
            return result.AccessToken;



        }

        /// <summary>
        /// Method to get authority URL from organization’s SOAP endpoint URL using Microsoft Azure Active Directory Authentication Library (ADAL), 
        /// </summary>
        /// <param name="result">The Authority Url returned from HttpWebResponse.</param>
        public static string DiscoveryAuthority(Uri serviceUrl)
        {
            // Use AuthenticationParameters to send a request to the organization's endpoint and
            // receive tenant information in the 401 challenge. 
            AuthenticationParameters parameters = null;

            HttpWebResponse response = null;
            try
            {
                // Create a web request where the authorization header contains the word "Bearer".
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
                httpWebRequest.Headers[HttpRequestHeader.Authorization.ToString()] = "Bearer";

                // The response is to be encoded.
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                response = (HttpWebResponse)httpWebRequest.GetResponse();
            }

            catch (WebException ex)
            {
                IsResultTrue = true; // good response stays, we don't need to show error on device
                response = (HttpWebResponse)ex.Response;
                ErrorMessage = ex.Message;
                // A good response was returned. Extract any parameters from the response.
                // The response should contain an authorization_uri parameter.
                parameters = AuthenticationParameters.CreateFromResponseAuthenticateHeader((response.Headers)["WWW-Authenticate"]);
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }
            // Return the authority URL.
            return parameters.Authority;

        }

        /// <summary>
        /// Returns a response from an Internet resource. 
        /// </summary>       
        public static WebResponse GetResponse(this WebRequest request)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            IAsyncResult asyncResult = request.BeginGetResponse(r => autoResetEvent.Set(), null);

            // Wait until the call is finished
            autoResetEvent.WaitOne(DefaultRequestTimeout);
            return request.EndGetResponse(asyncResult);
        }

        /// <summary>
        /// Get the DefaultRequestTimeout from the server.
        /// </summary>
        public static TimeSpan DefaultRequestTimeout { get; set; }

        /// <summary>
        /// Display an error message to the user.
        /// </summary>
        /// <param name="result">The authentication result returned from AcquireTokenAsync().</param>
        //private static async void DisplayErrorWhenAcquireTokenFails(AuthenticationResult result)
        //{
        //    switch (result.AccessToken)
        //    {
        //        break;
        //    }
        //}

    }
}