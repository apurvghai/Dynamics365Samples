// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Samples;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;

namespace UniversalModernApp
{
    static class CrmOAuthHelper
    {
        // TODO Set these string values as approppriate for your app registration and organization.
        // For more information, see the SDK topic "Walkthrough: Register an app with Active Directory".
        public const string ServerUrl = "https://duprepro.agcrm13.com:81";
        public const string resourceName = "https://auth.duprepro.com:81/";
        public static string redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
        public const string ClientId = "C8E3F34A-E1A6-455C-A6DB-A1B82E6A6BE0";

        public static string AuthorityUrl;

        static public OrganizationDataWebServiceProxy proxy;

        static CrmOAuthHelper()
        {
            proxy = new OrganizationDataWebServiceProxy();
            proxy.ServiceUrl = ServerUrl;
            proxy.EnableProxyTypes();
        }

        /// <summary>
        /// Method to get authority URL from organization’s SOAP endpoint.
        /// http://msdn.microsoft.com/en-us/library/dn531009.aspx#bkmk_oauth_discovery
        /// </summary>
        /// <param name="result">The Authority Url returned from HttpResponseMessage.</param>
        public static async Task DiscoveryAuthority()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
                // need to specify soap endpoint with client version,.
                HttpResponseMessage httpResponse = await httpClient.GetAsync(ServerUrl + "/XRMServices/2011/Organization.svc/web?SdkClientVersion=6.1.0.533");
                // For phone, we dont need oauth2/authorization part.
                AuthorityUrl = System.Net.WebUtility.UrlDecode(httpResponse.Headers.GetValues("WWW-Authenticate").FirstOrDefault().Split('=')[1]).Replace("oauth2/authorize", "");
            }
        }

        static public AuthenticationContext authContext = null;

        public static async Task<string> GetTokenSilent()
        {
            if (String.IsNullOrEmpty(CrmOAuthHelper.AuthorityUrl))
                await CrmOAuthHelper.DiscoveryAuthority();

            // If authContext is null, then generate it.
            if (authContext == null)
                authContext = new AuthenticationContext(CrmOAuthHelper.AuthorityUrl, false);

            //Clear Cache
            if (authContext.TokenCache.Count > 0)
                authContext.TokenCache.Clear();

            AuthenticationResult result = await authContext.AcquireTokenAsync(
                resource: CrmOAuthHelper.ServerUrl,
                clientId: CrmOAuthHelper.ClientId,
                redirectUri: new Uri(redirectUri));

            if (result != null && result.Status == AuthenticationStatus.Success)
            {
                // A token was successfully retrieved. Then store it.
                StoreToken(result);
            }
            return result.AccessToken;
        }

        //public static string DisplayResultedToken(Windows.UI.Xaml.Controls.TextBlock txt)
        //{
        //    return txt.Text = 
        //}

        public static void StoreToken(AuthenticationResult result)
        {
            if (result.Status == AuthenticationStatus.Success)
            {
                CrmOAuthHelper.proxy.AccessToken = result.AccessToken;
                //DisplayResultedToken
            }
            else
            {
                DisplayErrorWhenAcquireTokenFails(result);
            }
        }

        static private async void DisplayErrorWhenAcquireTokenFails(AuthenticationResult result)
        {
            MessageDialog dialog;

            switch (result.Error)
            {
                case "authentication_canceled":
                    // User cancelled, so no need to display a message.
                    break;
                case "temporarily_unavailable":
                case "server_error":
                    dialog = new MessageDialog("Please retry the operation. If the error continues, please contact your administrator.", "Sorry, an error has occurred.");
                    await dialog.ShowAsync();
                    break;
                default:
                    // An error occurred when acquiring a token. Show the error description in a MessageDialog. 
                    dialog = new MessageDialog(string.Format("If the error continues, please contact your administrator.\n\nError: {0}\n\nError Description:\n\n{1}", result.Error, result.ErrorDescription), "Sorry, an error has occurred.");
                    await dialog.ShowAsync();
                    break;
            }
        }

    }
}
