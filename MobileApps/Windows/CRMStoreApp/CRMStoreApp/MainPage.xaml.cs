
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Samples;
using Windows.Security.Authentication.Web;
using System.Net.Http.Headers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CRMStoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // TODO Set these string values as approppriate for your app registration and organization.
        // For more information, see the SDK topic "Walkthrough: Register an app with Active Directory".
        public const string ServerUrl = "https://duprepro.agcrm13.com:81";
        public const string resourceName = "https://auth.duprepro.com:81/";
        public static string redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
        public const string ClientId = "C8E3F34A-E1A6-455C-A6DB-A1B82E6A6BE0";
        public AuthenticationContext authContext = null;

        public static string AuthorityUrl;
        public static string TokenString { get; set; }

        static public OrganizationDataWebServiceProxy proxy;

        public MainPage()
        {
            this.InitializeComponent();
            proxy = new OrganizationDataWebServiceProxy();
            proxy.ServiceUrl = ServerUrl;
            proxy.EnableProxyTypes();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = string.Empty;

            // Disable the button until the operation is complete.
            btnAuthenticate.IsEnabled = false;

            // One-step async call.
            var x = await AccessOAuthTokenAsync();

            WorkingDisplay("Authentication Completed..");

            // Reenable the button in case you want to run the operation again.
            btnAuthenticate.IsEnabled = true;
        }


        public void WorkingDisplay(string txt)
        {
            txtStatus.Text += "\r\n " + txt + "\r\n";
        }


        async Task<String> AccessOAuthTokenAsync()
        {
            WorkingDisplay("Authentication started..");
            String accessToken = string.Empty;
            redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
            try
            {
                if (String.IsNullOrEmpty(AuthorityUrl))
                    await DiscoveryAuthorityAsync();

                // If authContext is null, then generate it.
                if (authContext == null)
                    authContext = new AuthenticationContext(AuthorityUrl, false);

                //Clear Cache
                if (authContext.TokenCache.Count > 0)
                    authContext.TokenCache.Clear();
                redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
                AuthenticationResult result = await authContext.AcquireTokenAsync(
                    resource: ServerUrl,
                    clientId: ClientId,
                    redirectUri: new Uri(redirectUri));
                redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
                if (result != null && result.Status == AuthenticationStatus.Success)
                {
                    // A token was successfully retrieved. Then store it.
                    //StoreToken(result);
                    TokenString = result.AccessToken;
                    WorkingDisplay("Storing Token Completed..");
                    WorkingDisplay("Displayin Token occurred.." + TokenString);
                }
                else
                {
                    WorkingDisplay("unknown occurred..");
                }
            }
            catch (Exception ex)
            {
                WorkingDisplay("Error occurred.." + ex.Message);
            }
            return accessToken;
        }

        /// <summary>
        /// Method to get authority URL from organization’s SOAP endpoint.
        /// http://msdn.microsoft.com/en-us/library/dn531009.aspx#bkmk_oauth_discovery
        /// </summary>
        /// <param name="result">The Authority Url returned from HttpResponseMessage.</param>
        async Task DiscoveryAuthorityAsync()
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

    }
}
